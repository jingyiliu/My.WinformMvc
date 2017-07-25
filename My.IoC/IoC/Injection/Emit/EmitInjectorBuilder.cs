using System;
using System.Reflection.Emit;
using System.Reflection;
using My.Emit;
using My.Exceptions;
using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Injection.Emit
{
    class EmitInjectorBuilder
    {
        const MethodAttributes PublicConstructorAttributes = MethodAttributes.Public;
        const MethodAttributes MethodOverrideAttributes =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig;
        const BindingFlags BaseConstructorFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags PrivateInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        //const BindingFlags PublicStaticFlags = BindingFlags.Static | BindingFlags.Public;

        //static readonly ConstructorInfo ObjectCtor;
        //static readonly MethodInfo GetParametersProperty;
        //static readonly MethodInfo RedundantParametersProvided;

        //static EmitInjectorBuilder()
        //{
        //    //ObjectCtor = typeof(object).GetConstructor(Type.EmptyTypes);

        //    //GetParametersProperty = typeof(InjectionContext).GetMethod("get_Parameters", PublicInstanceFlags, null, Type.EmptyTypes, null);
        //    //if (GetParametersProperty == null)
        //    //    throw new MissingMethodException("The InjectionContext.Parameters property can not be found!");

        //    //RedundantParametersProvided = typeof(ParameterException).GetMethod(
        //    //    "RedundantParametersProvided", PublicStaticFlags, null, new Type[] { typeof(InjectionContext) }, null);
        //    //if (RedundantParametersProvided == null)
        //    //    throw new MissingMethodException("The ParameterException.RedundantParametersProvided method can not be found!");
        //}

        public Type BuildType(TypeBuilder typeBuilder, InjectorEmitBody emitBody)
        {
            EmitConstructor(typeBuilder, emitBody);
            EmitExecuteMethod(typeBuilder, emitBody);
            return typeBuilder.CreateType();
        }

        #region EmitConstructor

        static void EmitConstructor(TypeBuilder typeBuilder, InjectorEmitBody emitBody)
        {
            ConstructorBuilder ctorBuilder;
            EmitGenerator gen;
            var ctorEmitBody = emitBody.ConstructorEmitBody;
            var methodEmitBodySet = emitBody.MethodEmitBodySet;

            var baseType = typeBuilder.BaseType;
            if (baseType == null)
                throw new ImpossibleException();
            var baseCtor = baseType.GetConstructor(BaseConstructorFlags, null, Type.EmptyTypes, null);
            if (baseCtor == null)
                throw new ImpossibleException();

            if (ctorEmitBody.DependencyProviders != null && ctorEmitBody.DependencyProviders.Length > 0)
            {
                ctorBuilder = methodEmitBodySet != null
                    ? DefineConstructorBuilder(typeBuilder, true, true)
                    : DefineConstructorBuilder(typeBuilder, true, false);

                gen = ctorBuilder.GetEmitGenerator();
                gen.LoadThis();
                gen.CallBaseConstructor(baseCtor);

                ctorEmitBody.EmitConstructorBody(typeBuilder, gen);

                if (methodEmitBodySet != null)
                {
                    var memberParameterIndex = 0;
                    for (int i = 0; i < methodEmitBodySet.Length; i++)
                        methodEmitBodySet[i].EmitConstructorBody(typeBuilder, gen, 2, ref memberParameterIndex);
                }

                gen.Return();
            }
            else
            {
                if (methodEmitBodySet != null)
                {
                    ctorBuilder = DefineConstructorBuilder(typeBuilder, false, true);
                    
                    gen = ctorBuilder.GetEmitGenerator();
                    gen.LoadThis();
                    gen.CallBaseConstructor(baseCtor);

                    var memberParameterIndex = 0;
                    for (int i = 0; i < methodEmitBodySet.Length; i++)
                        methodEmitBodySet[i].EmitConstructorBody(typeBuilder, gen, 1, ref memberParameterIndex);

                    gen.Return();
                }
                //else
                //{
                //    //ctorBuilder = DefineConstructorBuilder(typeBuilder, false, false);
                //    //ctorGenerator = ctorBuilder.GetEmitGenerator();
                //    //ctorGenerator.LoadThis();
                //    //ctorGenerator.CallBaseConstructor(baseCtor);
                //}
            }
        }

//        static MethodBuilder DefineConstructorBuilder(TypeBuilder typeBuilder, bool hasCtorParameters, bool hasMemberParameters)
//        {
//            MethodBuilder ctorBuilder = null;
//            if (hasCtorParameters)
//            {
//                var attributes = MethodAttributes.HideBySig | MethodAttributes.Public;
//                ctorBuilder = typeBuilder.DefineMethod(".ctor", attributes);
//                ctorBuilder.SetReturnType(typeof(void));
//
//                var paramTypes = hasMemberParameters
//                    ? new Type[] { typeof(DependencyProvider[]), typeof(DependencyProvider[]) }
//                    : new Type[] { typeof(DependencyProvider[]) };
//                ctorBuilder.SetParameters(paramTypes);
//                //ctorBuilder.DefineParameter(1, ParameterAttributes.None, "adnContext");
//            }
//            else
//            {
//                if (hasMemberParameters)
//                {
//                    var attributes = MethodAttributes.HideBySig | MethodAttributes.Public;
//                    ctorBuilder = typeBuilder.DefineMethod(".ctor", attributes);
//                    ctorBuilder.SetReturnType(typeof(void));
//                    var paramTypes = new Type[] { typeof(DependencyProvider[]) };
//                    ctorBuilder.SetParameters(paramTypes);
//                }
//                //else
//                //{
//                //    var attributes = MethodAttributes.HideBySig | MethodAttributes.Public;
//                //    ctorBuilder = typeBuilder.DefineMethod(".ctor", attributes);
//                //    ctorBuilder.SetReturnType(typeof(void));
//                //    var paramTypes = Type.EmptyTypes;
//                //    ctorBuilder.SetParameters(paramTypes);
//                //}
//            }
//
//            return ctorBuilder;
//        }

        static ConstructorBuilder DefineConstructorBuilder(TypeBuilder typeBuilder, bool hasCtorParameters, bool hasMemberParameters)
        {
            ConstructorBuilder ctorBuilder = null;
            if (hasCtorParameters)
            {
                var paramTypes = hasMemberParameters
                    ? new Type[] { typeof(DependencyProvider[]), typeof(DependencyProvider[]) }
                    : new Type[] { typeof(DependencyProvider[]) };

                ctorBuilder = typeBuilder.DefineConstructor(PublicConstructorAttributes, CallingConventions.Standard, paramTypes);
            }
            else
            {
                if (hasMemberParameters)
                {
                    var paramTypes = new Type[] { typeof(DependencyProvider[]) };
                    ctorBuilder = typeBuilder.DefineConstructor(PublicConstructorAttributes, CallingConventions.Standard, paramTypes);
                }
//                else
//                {
//                    ctorBuilder = typeBuilder.DefineDefaultConstructor(PublicConstructorAttributes);
//                }
            }

            return ctorBuilder;
        }

        #endregion

        #region EmitExecuteMethod

        static void EmitExecuteMethod(TypeBuilder typeBuilder, InjectorEmitBody emitBody)
        {
            var methodBuilder = DefineExecuteMethodBuilder(typeBuilder, emitBody.ContractType);
            var gen = methodBuilder.GetEmitGenerator();
            var ctorEmitBody = emitBody.ConstructorEmitBody;
            var methodEmitBodySet = emitBody.MethodEmitBodySet;

            var baseType = typeBuilder.BaseType;
            if (baseType == null)
                throw new ImpossibleException();
            var injectInstanceIntoContextMethod = baseType.GetMethod("InjectInstanceIntoContext", PrivateInstanceFlags);
            if (injectInstanceIntoContextMethod == null)
                throw new ImpossibleException();

            var instance = gen.DeclareLocal(ctorEmitBody.InjectedConstructor.DeclaringType);

            #region Create instance

            //IObjectRegistration p1;
            //IConfigurationModule p2;
            //ILifetimeScope p3;
            //_parameterMerger.Merge(context, out p1, out p2, out p3);
            //instance = new DummyClass(p1, p2, p3);
            ctorEmitBody.EmitExecuteMethodBody(gen, instance);

            #endregion

            #region InjectInstanceIntoContext(context, instance);

            gen.LoadThis();
            gen.LoadArgument(1);
            gen.LoadLocal(instance);
            gen.CallMethod(injectInstanceIntoContextMethod);

            #endregion

            #region Property/Method Injection

            //Parameter parameter;
            //_f0.CreateObject(context, out parameter);
            //instance.Parameter = parameter;

            //ContainerOption option;
            //_f1.CreateObject(context, out option);

            //IObjectContainer container;
            //_f2.CreateObject(context, out container);

            //instance.SetProperties(option, container);

            if (methodEmitBodySet != null)
            {
				for (int i = 0; i < methodEmitBodySet.Length; i++)
                	methodEmitBodySet[i].EmitExecuteMethodBody(gen, instance);
            }
                
            #endregion
            
            gen.Return();
        }

        static MethodBuilder DefineExecuteMethodBuilder(TypeBuilder typeBuilder, Type contractType)
        {
            var methodBuilder = typeBuilder.DefineMethod("Execute", MethodOverrideAttributes);
            methodBuilder.SetReturnType(typeof(void));
            methodBuilder.SetParameters(typeof(InjectionContext<>).MakeGenericType(contractType));
            var context = methodBuilder.DefineParameter(1, ParameterAttributes.None, "context");
            return methodBuilder;
        }

        #endregion
    }
}
