
using System;
using System.Reflection;
using System.Reflection.Emit;
using My.Emit;
using My.Exceptions;
using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Injection.Emit
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class EmitParameterMergerBuilder
    {
        const MethodAttributes PublicConstructorAttributes = MethodAttributes.Public;
        const BindingFlags InstancePublicDeclaredOnlyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
        const BindingFlags InstancePublicNonPublicFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags StaticPublicNonPublicFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        const MethodAttributes MergeMethodAttributes = MethodAttributes.Public;

        static readonly ConstructorInfo BaseConstructor;
        static readonly MethodInfo VerfiyConstructorParameterLengthMethod;
        static readonly MethodInfo GetInjectionContextParameters;
        static readonly MethodInfo GetParameterLength;
        static readonly MethodInfo GetParameterKind;
        static readonly MethodInfo GetItem;
        static readonly MethodInfo OpenGetPositionalDependencyObjectMethod;
        static readonly MethodInfo OpenGetNamedDependencyObjectMethod;
        static readonly MethodInfo ParameterNumberExceedsMethod;
        static readonly ConstructorInfo ImpossibleExceptionConstructor;


        static readonly Type EmitParameterMergerType = typeof (EmitParameterMerger);
        static readonly Type ParameterSetType = typeof(ParameterSet);
        static readonly Type InjectionContextType = typeof(InjectionContext);
        static readonly Type[] ConstructorParameterType = new Type[] { typeof(DependencyProvider[]) };
        static readonly Type GenericDependencyProviderType = typeof(DependencyProvider<>);

        static EmitParameterMergerBuilder()
        {
            BaseConstructor = EmitParameterMergerType.GetConstructor(InstancePublicNonPublicFlags, null, Type.EmptyTypes, null);
            VerfiyConstructorParameterLengthMethod = EmitParameterMergerType.GetMethod("VerfiyConstructorParameterLength",
                StaticPublicNonPublicFlags, null, new Type[] { typeof(DependencyProvider[]), typeof(Int32) }, null);
            OpenGetPositionalDependencyObjectMethod = EmitParameterMergerType.GetMethod(
                "GetPositionalDependencyObject", StaticPublicNonPublicFlags);
            OpenGetNamedDependencyObjectMethod = EmitParameterMergerType.GetMethod(
                "GetNamedDependencyObject", StaticPublicNonPublicFlags);
            ParameterNumberExceedsMethod = EmitParameterMergerType.GetMethod("ParameterNumberExceeds",
                StaticPublicNonPublicFlags, null, new Type[] { typeof(InjectionContext), typeof(Int32), typeof(Int32) }, null);

            GetParameterLength = ParameterSetType.GetMethod("get_Length",
                InstancePublicNonPublicFlags, null, Type.EmptyTypes, null);
            GetParameterKind = ParameterSetType.GetMethod("get_ParameterKind",
                InstancePublicNonPublicFlags, null, Type.EmptyTypes, null);
            GetItem = ParameterSetType.GetMethod("get_Item",
                InstancePublicNonPublicFlags, null, new Type[] { typeof(Int32) }, null);

            GetInjectionContextParameters = InjectionContextType.GetMethod("get_Parameters",
                InstancePublicNonPublicFlags, null, Type.EmptyTypes, null);
            ImpossibleExceptionConstructor = typeof(ImpossibleException).GetConstructor(Type.EmptyTypes);

            if (BaseConstructor == null || VerfiyConstructorParameterLengthMethod == null || OpenGetPositionalDependencyObjectMethod == null
                || GetParameterLength == null || GetParameterKind == null || GetItem == null
                || ParameterNumberExceedsMethod == null || GetInjectionContextParameters == null
                || ImpossibleExceptionConstructor == null)
                throw new ImpossibleException();
        }

        public Type BuildType(TypeBuilder typeBuilder, int genParamLength)
        {
            var genParamBuilders = DefineGenericParameters(typeBuilder, genParamLength);

            var depProviderFields = EmitConstructor(typeBuilder, genParamBuilders);
            EmitMergeMethod(typeBuilder, genParamBuilders, depProviderFields);
            return typeBuilder.CreateType();
        }

        static GenericTypeParameterBuilder[] DefineGenericParameters(TypeBuilder typeBuilder, int genParamLength)
        {
            var genTypeNames = new string[genParamLength];
            for (int i = 0; i < genParamLength; i++)
                genTypeNames[i] = "T" + i;
            return typeBuilder.DefineGenericParameters(genTypeNames);
        }

        #region Constructor

        static FieldBuilder[] EmitConstructor(TypeBuilder typeBuilder, GenericTypeParameterBuilder[] genParamBuilders)
        {
            var depProviderFields = new FieldBuilder[genParamBuilders.Length];
            for (int i = 0; i < genParamBuilders.Length; i++)
                depProviderFields[i] = DefineDependencyProviderField(typeBuilder, i.ToString(), genParamBuilders[i]);

            var ctorBuilder = DefineConstructorBuilder(typeBuilder);
            var gen = ctorBuilder.GetEmitGenerator();

            #region CallBaseConstructor

            gen.LoadThis();
            gen.CallBaseConstructor(BaseConstructor);

            #endregion

            #region VerfiyConstructorParameterLength(dependencyProviders, length);

            gen.LoadArgument(1);
            gen.LoadInt32(genParamBuilders.Length);
            gen.CallMethod(VerfiyConstructorParameterLengthMethod);
            
            #endregion

            for (int i = 0; i < genParamBuilders.Length; i++)
            {
                var fieldBuilder = depProviderFields[i];
                gen.LoadThis();
                gen.LoadArgument(1);
                gen.LoadArrayElement(i);
                gen.CastAny(fieldBuilder.FieldType);
                gen.StoreField(fieldBuilder);
            }

            gen.Return();
            return depProviderFields;
        }

        static FieldBuilder DefineDependencyProviderField(TypeBuilder typeBuilder, string fieldName, GenericTypeParameterBuilder paramBuilder)
        {
            var closeDeproviderType = GenericDependencyProviderType.MakeGenericType(paramBuilder);
            return typeBuilder.DefineField(fieldName, closeDeproviderType, FieldAttributes.Private);
        }

        static ConstructorBuilder DefineConstructorBuilder(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineConstructor(PublicConstructorAttributes, CallingConventions.Standard, ConstructorParameterType);
        }

        #endregion

        static void EmitMergeMethod(TypeBuilder typeBuilder, GenericTypeParameterBuilder[] genParamBuilders, FieldBuilder[] depProviderFields)
        {
            var methodBuilder = DefineMergeMethodBuilder(typeBuilder, genParamBuilders);
            var gen = methodBuilder.GetEmitGenerator();
            EmitMergeMethodBody(gen, genParamBuilders, depProviderFields);
            gen.Return();
        }

        static MethodBuilder DefineMergeMethodBuilder(TypeBuilder typeBuilder, GenericTypeParameterBuilder[] genParamBuilders)
        {
            var methodBuilder = typeBuilder.DefineMethod("Merge", MergeMethodAttributes);
            methodBuilder.SetReturnType(typeof(void));

            var paramTypes = new Type[genParamBuilders.Length + 1];
            paramTypes[0] = InjectionContextType;
            for (int i = 0; i < genParamBuilders.Length; i++)
                paramTypes[i + 1] = genParamBuilders[i].MakeByRefType();
            methodBuilder.SetParameters(paramTypes);

            methodBuilder.DefineParameter(1, ParameterAttributes.None, "context");
            for (int i = 0; i < genParamBuilders.Length; i++)
                methodBuilder.DefineParameter(i + 2, ParameterAttributes.Out, "p" + i);

            return methodBuilder;
        }

        static void EmitMergeMethodBody(EmitGenerator gen, GenericTypeParameterBuilder[] genParamBuilders,
            FieldBuilder[] depProviderFields)
        {
            // var myParams = context.Parameters;
            var myParams = gen.DeclareLocal(ParameterSetType);
            gen.LoadArgument(1);
            gen.CallMethod(GetInjectionContextParameters);
            gen.StoreLocal(myParams);

            #region If (myParams == null || myParams.Length == 0) condition

            // if (myParams == null || myParams.Length == 0) Goto myParamsIsNullLabel Else Goto myParamsIsNotNullAndLengthNotEqualTo0Label
            var myParamsIsNullLabel = gen.DefineLabel();
            var myParamsIsNotNullAndLengthNotEqualTo0Label = gen.DefineLabel();

            gen.LoadLocal(myParams);
            gen.IfFalseGoto(myParamsIsNullLabel);

            gen.LoadLocal(myParams);
            gen.CallMethod(GetParameterLength);
            gen.IfTrueGoto(myParamsIsNotNullAndLengthNotEqualTo0Label);

            #endregion

            var createObjectMethods = new MethodInfo[depProviderFields.Length];
                 
            #region if (myParams == null || myParams.Length == 0) block

            gen.MarkLabel(myParamsIsNullLabel);
            for (int i = 0; i < depProviderFields.Length; i++)
            {
                // _dependency0.CreateObject(context, out param0);
                // _dependency1.CreateObject(context, out param1);
                // ...
                var depProviderField = depProviderFields[i];
                gen.LoadThis();
                gen.LoadField(depProviderField);
                gen.LoadArgument(1); // load parameter [context]
                gen.LoadArgument(i + 2); // load parameter [param0], [param1], [param2]...
                var createObjectMethod = GetCreateObjectOfDependencyProviderMethod(depProviderField.FieldType);
                createObjectMethods[i] = createObjectMethod;
                gen.CallMethod(createObjectMethod);
            }

            gen.Return();

            #endregion

            #region if (myParams != null && myParams.Length != 0) block

            gen.MarkLabel(myParamsIsNotNullAndLengthNotEqualTo0Label);

            // var paramKind = myParams.ParameterKind;
            var paramKind = gen.DeclareLocal(typeof(ParameterKind));
            gen.LoadLocal(myParams);
            gen.CallMethod(GetParameterKind);
            gen.StoreLocal(paramKind);

            // (Ok)=====================================================

            var paramKindDefaultCase = gen.DefineLabel();
            var paramKindTable = new Label[] { gen.DefineLabel(), gen.DefineLabel() };

            gen.LoadLocal(paramKind);
            gen.Switch(paramKindTable); // switch (paramKind)
            // case default Goto paramKindDefaultCase
            gen.Goto(paramKindDefaultCase);

            // (Ok)=====================================================

            #region case paramKind = ParameterKind.Positional (Ok)

            gen.MarkLabel(paramKindTable[0]);

            // var myParamLength = myParams.Length;
            var myParamLength = gen.DeclareLocal(typeof(int));
            gen.LoadLocal(myParams);
            gen.CallMethod(GetParameterLength);
            gen.StoreLocal(myParamLength);

            // switch (myParamLength)
            gen.LoadLocal(myParamLength);
            gen.LoadInt32(1);
            gen.Substrate(); // myParamLength - 1, this is because the switch branch is starting from 1, instead of 0

            var paramLengthDefaultCase = gen.DefineLabel();
            var paramLengthTable = new Label[depProviderFields.Length];
            for (int i = 0; i < depProviderFields.Length; i++)
                paramLengthTable[i] = gen.DefineLabel();

            gen.Switch(paramLengthTable);
            // case default Goto paramLengthDefaultCase
            gen.Goto(paramLengthDefaultCase);

            #region case i for myParamLength

            for (int i = 0; i < depProviderFields.Length; i++)
            {
                gen.MarkLabel(paramLengthTable[i]);
                for (int j = 0; j < depProviderFields.Length; j++)
                {
                    if (j <= i)
                    {
                        // something like: param0 = GetPositionalDependencyObject(_depProvider0, myParams[0], context);
                        // load _depProvider0
                        gen.LoadThis();
                        gen.LoadField(depProviderFields[j]);

                        // load myParams[0]
                        gen.LoadLocal(myParams);
                        gen.LoadInt32(j);
                        gen.CallMethod(GetItem);

                        // load context
                        gen.LoadArgument(1);

                        var genParamType = genParamBuilders[j];
                        gen.CallMethod(OpenGetPositionalDependencyObjectMethod.MakeGenericMethod(genParamType));
                        //gen.StoreObject(genParamType);
                        gen.StoreArgument(j + 2);
                    }
                    else
                    {
                        // something like: _depProvider1.CreateObject(context, out param1);
                        // load _depProvider1
                        gen.LoadThis();
                        gen.LoadField(depProviderFields[j]);
                        // load context
                        gen.LoadArgument(1);
                        // load param1, its index is j + 2
                        gen.LoadArgument(j + 2);
                        gen.CallMethod(createObjectMethods[j]);
                    }
                }
                gen.Return();
            }

            #endregion

            #region default case for myParamLength

            gen.MarkLabel(paramLengthDefaultCase);
            gen.LoadArgument(1);
            gen.LoadInt32(depProviderFields.Length);
            gen.LoadLocal(myParams);
            gen.CallMethod(GetParameterLength);
            gen.CallMethod(ParameterNumberExceedsMethod);
            gen.Throw();

            #endregion

            gen.Return();

            #endregion

            #region case paramKind = ParameterKind.Named (Ok)

            gen.MarkLabel(paramKindTable[1]);
            for (int i = 0; i < depProviderFields.Length; i++)
            {
                // something like: param0 = GetNamedDependencyObject(_depProvider0, myParams, context);

                // load _depProvider0
                gen.LoadThis();
                //gen.LoadThis(); // why do we need this?
                gen.LoadField(depProviderFields[i]);

                // load myParams
                gen.LoadLocal(myParams);

                // load context
                gen.LoadArgument(1);

                var genParamType = genParamBuilders[i];
                gen.CallMethod(OpenGetNamedDependencyObjectMethod.MakeGenericMethod(genParamType));

                // store to param0, its parameter index is [i + 2]
                gen.StoreArgument(i + 2);
            }
            gen.Return();

            #endregion

            #region default case for ParameterKind (Ok)

            gen.MarkLabel(paramKindDefaultCase);
            gen.Throw(ImpossibleExceptionConstructor);

            #endregion

            gen.Return();

            #endregion
        }

        static MethodInfo GetCreateObjectOfDependencyProviderMethod(Type closeDepProviderType)
        {
            var openDepProviderType = closeDepProviderType.GetGenericTypeDefinition();
            if (openDepProviderType == null)
                throw new ImpossibleException();
            var createObjectMethods = openDepProviderType.GetMethods(InstancePublicDeclaredOnlyFlags);
            //foreach (var objectMethod in createObjectMethods)
            //{
            //    if (objectMethod.Name != "CreateObject")
            //        continue;
            //    var parameters = objectMethod.GetParameters();
            //    if (parameters.Length != 2)
            //        throw new ImpossibleException();
            //    if (!ReferenceEquals(parameters[1].ParameterType, genParamBuilders[i]))
            //        continue;
            //    createObjectMethod = objectMethod;
            //    break;
            //}
            //if (createObjectMethod == null)
            //    throw new ImpossibleException();
            if (createObjectMethods.Length != 1)
                throw new ImpossibleException();
            return createObjectMethods[0];
        }
    }
}
