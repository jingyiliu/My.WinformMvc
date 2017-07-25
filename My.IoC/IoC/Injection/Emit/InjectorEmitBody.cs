
using System;
using System.Reflection;
using System.Reflection.Emit;
using My.Emit;
using My.Exceptions;
using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Injection.Emit
{
    class ConstructorEmitBody
    {
        readonly EmitInjectorManager _manager;
        readonly ConstructorInfo _injectedCtor;
        readonly DependencyProvider[] _dependencyProviders;
        FieldBuilder _paramMergerField;

        public ConstructorEmitBody(EmitInjectorManager manager, ConstructorInfo injectedCtor, DependencyProvider[] dependencyProviders)
        {
            _manager = manager;
            _injectedCtor = injectedCtor;
            _dependencyProviders = dependencyProviders;
        }

        public ConstructorInfo InjectedConstructor 
        {
            get { return _injectedCtor; }
        }

        public DependencyProvider[] DependencyProviders
        {
            get { return _dependencyProviders; }
        }

        public FieldBuilder ParameterMergerField
        {
            get { return _paramMergerField; }
        }

        public void EmitConstructorBody(TypeBuilder typeBuilder, EmitGenerator gen)
        {
            var mergerType = DefineParameterMergerField(typeBuilder);
            var mergerCtor = GetParameterMergerConstructor(mergerType);

            gen.LoadThis();
            gen.LoadArgument(1);
            gen.New(mergerCtor);
            gen.StoreField(_paramMergerField);
        }

        Type DefineParameterMergerField(TypeBuilder typeBuilder)
        {
            var mergerType = GetParameterMergerType();
            if (mergerType == null)
                throw new ImpossibleException();

            _paramMergerField = typeBuilder.DefineField("paramMerger", mergerType, FieldAttributes.Private);
            return mergerType;
        }

        static ConstructorInfo GetParameterMergerConstructor(Type mergerType)
        {
            var ctor = mergerType.GetConstructor(new Type[] { typeof (DependencyProvider[]) });
            if (ctor == null)
                throw new ImpossibleException();
            return ctor;
        }

        Type GetParameterMergerType()
        {
            var paramTypes = GetParameterTypes();
            var genParamMergerType = _manager.GetOrCreateParameterMergerType(_dependencyProviders.Length);
            return genParamMergerType.MakeGenericType(paramTypes);
        }

        Type[] GetParameterTypes()
        {
            var types = new Type[_dependencyProviders.Length];
            for (int i = 0; i < _dependencyProviders.Length; i++)
                types[i] = _dependencyProviders[i].TargetType;
            return types;
        }

        public void EmitExecuteMethodBody(EmitGenerator gen, LocalBuilder instance)
        {
            if (_dependencyProviders == null || _dependencyProviders.Length == 0)
            {
                //var dummyClass = new DummyClass();
                gen.New(_injectedCtor);
                gen.StoreLocal(instance);
            }
            else
            {
                //IObjectRegistration p1;
                //IConfigurationModule p2;
                //ILifetimeScope p3;
                //_parameterMerger.Merge(context, out p1, out p2, out p3);
                //instance = new DummyClass(p1, p2, p3);

                var mergeMethod = _paramMergerField.FieldType.GetMethod("Merge");
                if (mergeMethod == null)
                    throw new ImpossibleException();

                #region Define Locals

                //IObjectRegistration p1;
                //IConfigurationModule p2;
                //ILifetimeScope p3;
                var ctorParamBuilders = new LocalBuilder[_dependencyProviders.Length];
                for (int i = 0; i < _dependencyProviders.Length; i++)
                    ctorParamBuilders[i] = gen.DeclareLocal(_dependencyProviders[i].TargetType);

                #endregion

                #region _parameterMerger.Merge(context, out p1, out p2, out p3);

                gen.LoadThis();
                gen.LoadField(_paramMergerField);
                gen.LoadArgument(1);
                for (int i = 0; i < ctorParamBuilders.Length; i++)
                    gen.LoadLocalAddress(ctorParamBuilders[i]);

                gen.CallMethod(mergeMethod);

                #endregion

                #region instance = new DummyClass(p1, p2, p3);

                for (int i = 0; i < ctorParamBuilders.Length; i++)
                    gen.LoadLocal(ctorParamBuilders[i]);

                gen.New(_injectedCtor);
                gen.StoreLocal(instance);

                #endregion
            }
        }
    }

    class ParameterEmitBody
    {
        readonly DependencyProvider _dependencyProvider;
        FieldBuilder _paramField;
        LocalBuilder _paramValue;

        public ParameterEmitBody(DependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
        }

        public DependencyProvider DependencyProvider
        {
            get { return _dependencyProvider; }
        }

        public FieldBuilder ParameterField
        {
            get { return _paramField; }
        }

        public LocalBuilder ParameterValue
        {
            get { return _paramValue; }
        }

        public void EmitConstructorBody(TypeBuilder typeBuilder, EmitGenerator gen, int ctorArgumentIndex, ref int memberParameterIndex)
        {
            var fieldType = DefineParameterField(typeBuilder, memberParameterIndex);
            
            gen.LoadThis();
            gen.LoadArgument(ctorArgumentIndex);
            gen.LoadArrayElement(memberParameterIndex);
            gen.CastAny(fieldType);
            gen.StoreField(_paramField);

            memberParameterIndex += 1;
        }

        Type DefineParameterField(TypeBuilder typeBuilder, int memberParameterIndex)
        {
            var fieldType = typeof(DependencyProvider<>).MakeGenericType(_dependencyProvider.TargetType);
            _paramField = typeBuilder.DefineField(memberParameterIndex.ToString(), fieldType, FieldAttributes.Private);
            return fieldType;
        }

        public void EmitExecuteMethodBody(EmitGenerator gen, LocalBuilder instance)
        {
            // Use MakeByRefType(), because this is an out (ref) parameter in the method signature.
            var paramType = _dependencyProvider.TargetType.MakeByRefType(); 
            var paramTypes = new Type[] { typeof(InjectionContext), paramType };

            var createObjectMethod = _paramField.FieldType.GetMethod("CreateObject", paramTypes);
            if (createObjectMethod == null)
                throw new ImpossibleException();

            _paramValue = gen.DeclareLocal(_dependencyProvider.TargetType);
            gen.LoadThis();
            gen.LoadField(_paramField);
            gen.LoadArgument(1);
            gen.LoadLocalAddress(_paramValue);
            gen.CallMethod(createObjectMethod);
        }
    }

    class MethodEmitBody
    {
        readonly MethodInfo _injectedMethod;
        readonly ParameterEmitBody[] _paramEmitBodySet;

        public MethodEmitBody(MethodInfo injectedMethod, ParameterEmitBody[] paramEmitBodySet)
        {
            _injectedMethod = injectedMethod;
            _paramEmitBodySet = paramEmitBodySet;
        }

        public MethodInfo InjectedMethod
        {
            get { return _injectedMethod; }
        }

        public ParameterEmitBody[] ParameterEmitBodySet
        {
            get { return _paramEmitBodySet; }
        }

        public void EmitConstructorBody(TypeBuilder typeBuilder, EmitGenerator gen, int ctorArgumentIndex, ref int memberParameterIndex)
        {
            if (_paramEmitBodySet == null)
                return;
            for (int i = 0; i < _paramEmitBodySet.Length; i++)
                _paramEmitBodySet[i].EmitConstructorBody(typeBuilder, gen, ctorArgumentIndex, ref memberParameterIndex);
        }

        public void EmitExecuteMethodBody(EmitGenerator gen, LocalBuilder instance)
        {
            if (_paramEmitBodySet == null)
            {
                gen.LoadLocal(instance);
                gen.CallMethod(_injectedMethod);
            }
            else
            {
                for (int i = 0; i < _paramEmitBodySet.Length; i++)
                    _paramEmitBodySet[i].EmitExecuteMethodBody(gen, instance);

                gen.LoadLocal(instance);

                for (int i = 0; i < _paramEmitBodySet.Length; i++)
                    gen.LoadLocal(_paramEmitBodySet[i].ParameterValue);

                gen.CallMethod(_injectedMethod);
            }
        }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class InjectorEmitBody
    {
        readonly Type _contractType;
        readonly ConstructorEmitBody _ctorEmitBody;
        readonly MethodEmitBody[] _methodEmitBodySet;

        public InjectorEmitBody(Type contractType, ConstructorEmitBody ctorEmitBody, MethodEmitBody[] methodEmitBodySet)
        {
            _contractType = contractType;
            _ctorEmitBody = ctorEmitBody;
            _methodEmitBodySet = methodEmitBodySet;
        }

        public Type ContractType { get { return _contractType; } }
        public ConstructorEmitBody ConstructorEmitBody { get { return _ctorEmitBody; } }
        public MethodEmitBody[] MethodEmitBodySet { get { return _methodEmitBodySet; } }
    }
}
