
using System;
using System.Reflection.Emit;
using System.Reflection;
using My.Helpers;
using My.IoC;

namespace My.Emit
{
    //class ConditionalBranch
    //{
    //    Label _ifLabel;
    //    public ConditionalBranch(EmitGenerator condition, EmitGenerator body)
    //    {
    //    }
    //}

    /// <summary>
    /// A helper class for Reflection.Emit
    /// </summary>
    /// <remarks>
    /// There are many '.s' version of instructions (for example OpCodes.Brfalse and OpCodes.Brfalse_S), 
    /// these instructions are the short form of their equivalent instructions.
    /// </remarks>
    public class EmitGenerator
    {
        readonly ILGenerator _gen;

        public EmitGenerator(ILGenerator gen)
        {
            Requires.NotNull(gen, "gen");
            _gen = gen;
        }

        public ILGenerator IlGenerator
        {
            get { return _gen; }
        }

        /// <summary>
        /// Declares a local variable.
        /// </summary>
        /// <param name="localType">The Type of the local variable.</param>
        /// <returns>The declared local variable.</returns>
        public LocalBuilder DeclareLocal(Type localType)
        {
            Requires.NotNull(localType, "localType");
            return _gen.DeclareLocal(localType);
        }

        /// <summary>
        /// Declares a local variable, optionally pinning the object referred to by the variable.
        /// </summary>
        /// <param name="localType">The Type of the local variable.</param>
        /// <param name="pinned"><b>true</b> to pin the object in memory; otherwise, <b>false</b>.</param>
        /// <returns>The declared local variable.</returns>
        public LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            Requires.NotNull(localType, "localType");
            return _gen.DeclareLocal(localType, pinned);
        }

        /// <summary>
        /// Declares a new gotoLabel.
        /// </summary>
        /// <returns>Returns a new label that can be used as a token for branching.</returns>
        public Label DefineLabel()
        {
            return _gen.DefineLabel();
        }

        /// <summary>
        /// Marks the Microsoft intermediate language (MSIL) stream's current position 
        /// with the given gotoLabel.
        /// </summary>
        /// <param name="gotoLabel">The label for which to set an index.</param>
        /// <returns>Current instance of the <see cref="EmitGenerator"/>.</returns>
        public void MarkLabel(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.MarkLabel(gotoLabel); 
        }

        #region Load Methods

        public void LoadThis()
        {
            _gen.Emit(OpCodes.Ldarg_0);
        }

        public void LoadArgumentAddress(int index)
        {
            if (index <= byte.MaxValue)
                _gen.Emit(OpCodes.Ldarga_S, (byte)index);
            else
                _gen.Emit(OpCodes.Ldarga, (short)index);
        }

        public void LoadArgument(int index)
        {
            switch (index)
            {
                case 0:
                    throw new InvalidOperationException(Resources.UseLoadThisMethodInstead);
                case 1:
                    _gen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    _gen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    _gen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                        _gen.Emit(OpCodes.Ldarg_S, (byte)index); //将参数（由指定的短格式索引引用）加载到计算堆栈上。
                    else
                        _gen.Emit(OpCodes.Ldarg, (short)index);
                    break;
            }
        }

        public void LoadString(string content)
        {
            Requires.NotNull(content, "content");
            _gen.Emit(OpCodes.Ldstr, content);
        }

        public void LoadBoolean(bool boolVar)
        {
            if (boolVar)
                _gen.Emit(OpCodes.Ldc_I4_1);
            else
                _gen.Emit(OpCodes.Ldc_I4_0);
        }

        public void LoadInt32(int intVar)
        {
            switch (intVar)
            {
                case -1:
                    _gen.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    _gen.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    _gen.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    _gen.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    _gen.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    _gen.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    _gen.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    _gen.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    _gen.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    _gen.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            _gen.Emit(OpCodes.Ldc_I4, intVar);
        }

        public void LoadDouble(float num)
        {
            _gen.Emit(OpCodes.Ldc_R4, num); 
        }

        public void LoadNull()
        {
            _gen.Emit(OpCodes.Ldnull);
        }

        public void GetArrayLength()
        {
            _gen.Emit(OpCodes.Ldlen);
        }

        public void LoadArrayElement(int index)
        {
            LoadInt32(index);
            _gen.Emit(OpCodes.Ldelem_Ref);
        }

        //public void LoadArrayElement(int index, Type elementType)
        //{
        //    LoadInt32(index);
        //    _gen.EmitGenerator(OpCodes.Ldelem, elementType);
        //}

        public void LoadField(FieldInfo fieldInfo)
        {
            Requires.NotNull(fieldInfo, "fieldInfo");
            _gen.Emit(OpCodes.Ldfld, fieldInfo);
        }

        public void LoadField(FieldBuilder fieldBuilder)
        {
            Requires.NotNull(fieldBuilder, "fieldBuilder");
            _gen.Emit(OpCodes.Ldfld, fieldBuilder);
        }

        public void LoadStaticField(FieldInfo fieldInfo)
        {
            Requires.NotNull(fieldInfo, "fieldInfo");
            _gen.Emit(OpCodes.Ldsfld, fieldInfo); 
        }

        public void LoadLocal(LocalBuilder localBuilder)
        {
            Requires.NotNull(localBuilder, "localBuilder");
            _gen.Emit(OpCodes.Ldloc, localBuilder);
        }

        public void LoadLocalAddress(LocalBuilder localBuilder)
        {
            Requires.NotNull(localBuilder, "localBuilder");
            //var index = localBuilder.LocalIndex;
            //if (index <= byte.MaxValue)
            //    _gen.Emit(OpCodes.Ldloca_S, index);
            //else
            //    _gen.Emit(OpCodes.Ldloca, index);
            _gen.Emit(OpCodes.Ldloca, localBuilder);
        }

        public void InitLocal(LocalBuilder localBuilder)
        {
            Requires.NotNull(localBuilder, "localBuilder");
            var localType = localBuilder.LocalType;
            if (localType == null)
                throw new InvalidOperationException();

            if (!localType.IsValueType)
            {
                LoadNull();
                StoreLocal(localBuilder);
            }
            else
            {
                LoadLocalAddress(localBuilder);
                _gen.Emit(OpCodes.Initobj, localType);
            }
        }

        //public void LoadDefaultValue(Type type)
        //{
        //    if (type == null) throw new ArgumentNullException("type");

        //    switch (Type.GetTypeCode(type))
        //    {
        //        case TypeCode.Boolean:
        //        case TypeCode.Char:
        //        case TypeCode.SByte:
        //        case TypeCode.Int16:
        //        case TypeCode.Int32:
        //        case TypeCode.Byte:
        //        case TypeCode.UInt16:
        //        case TypeCode.UInt32:
        //            return LoadInt32(0);

        //        case TypeCode.Int64:
        //        case TypeCode.UInt64:
        //            return LoadInt32(0).ConvertToInt64;

        //        case TypeCode.Single:
        //        case TypeCode.Double:
        //            return LoadDouble(0);

        //        case TypeCode.String:
        //            return LoadStaticField(typeof(string).GetField("Empty"));

        //        default:
        //            if (type.IsClass || type.IsInterface)
        //                return LoadNull;
        //            else
        //                throw new ArgumentException(string.Format("Unsupported type {0}!", type.ToFullTypeName()));
        //    }
        //}

        static readonly MethodInfo GetTypeFromHandle = typeof (Type).GetMethod("GetTypeFromHandle", new Type[] {typeof (RuntimeTypeHandle)});
        /// <summary>
        /// Loads a type instance at runtime.
        /// </summary>
        /// <param name="type">A type</param>
        public void LoadType(Type type)
        {
            Requires.NotNull(type, "type");
            LoadToken(type);
            CallMethod(GetTypeFromHandle);
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<seealso cref="OpCodes.Ldtoken"/>, methodInfo) that
        /// converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public void LoadToken(MethodInfo methodInfo)
        {
            Requires.NotNull(methodInfo, "methodInfo");
            _gen.Emit(OpCodes.Ldtoken, methodInfo); 
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, fieldInfo) that
        /// converts a metadata token to its runtime representation, 
        /// pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public void LoadToken(FieldInfo fieldInfo)
        {
            Requires.NotNull(fieldInfo, "fieldInfo");
            _gen.Emit(OpCodes.Ldtoken, fieldInfo); 
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, type) that
        /// converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public void LoadToken(Type type)
        {
            Requires.NotNull(type, "type");
            _gen.Emit(OpCodes.Ldtoken, type); 
        }

        #endregion

        #region Store Methods

        //public void StoreObject(Type type)
        //{
        //    Requires.NotNull(type, "type");
        //    _gen.Emit(OpCodes.Stobj, type);
        //}

        public void StoreLocal(LocalBuilder localBuilder)
        {
            Requires.NotNull(localBuilder, "localBuilder");
            _gen.Emit(OpCodes.Stloc, localBuilder);
        }

        public void StoreArgument(int slot)
        {
            if (slot <= byte.MaxValue)
                _gen.Emit(OpCodes.Starg_S, (byte)slot);
            else
                _gen.Emit(OpCodes.Starg, (short)slot);
        }

        public void StoreField(FieldInfo fieldInfo)
        {
            Requires.NotNull(fieldInfo, "fieldInfo");
            _gen.Emit(OpCodes.Stfld, fieldInfo);
        }

        public void StoreField(FieldBuilder fieldBuilder)
        {
            Requires.NotNull(fieldBuilder, "fieldBuilder");
            _gen.Emit(OpCodes.Stfld, fieldBuilder);
        } 

        #endregion

        public void CallMethod(MethodInfo methodInfo)
        {
            Requires.NotNull(methodInfo, "methodInfo");
            if (methodInfo.IsVirtual)
                _gen.Emit(OpCodes.Callvirt, methodInfo);
            else if (methodInfo.IsStatic)
                _gen.Emit(OpCodes.Call, methodInfo);
            else
                _gen.Emit(OpCodes.Call, methodInfo);
        }

        public void CallBaseConstructor(ConstructorInfo constructor)
        {
            Requires.NotNull(constructor, "constructor");
            _gen.Emit(OpCodes.Call, constructor);
        }

        public void New(ConstructorInfo constructor)
        {
            Requires.NotNull(constructor, "constructor");
            _gen.Emit(OpCodes.Newobj, constructor);
        }

        #region Convert

        public void CastAny(Type toType)
        {
            Requires.NotNull(toType, "toType");
            if (toType.IsValueType)
                _gen.Emit(OpCodes.Unbox_Any, toType);
            else
                _gen.Emit(OpCodes.Castclass, toType);
        }

        public void ConvertToInt32()
        {
            _gen.Emit(OpCodes.Conv_I4);
        }

        public void ConvertToInt64()
        {
            _gen.Emit(OpCodes.Conv_I8);
        }

        // The targetType is a metadata token indicating the desired class. 
        // If the class of the object on the top of the stack implements targetType (if class is an interface) 
        // or is a derived class of targetType (if targetType is a regular class) then it is cast to type class 
        // and the result is pushed on the stack, exactly as though Castclass had been called. Otherwise, a null 
        // reference is pushed on the stack. If the object reference itself is a null reference, then isinst 
        // likewise returns a null reference.
        public void Is(Type targetType)
        {
            Requires.NotNull(targetType, "targetType");
            _gen.Emit(OpCodes.Isinst, targetType);
        }

        public void As(Type targetType)
        {
            Is(targetType);
        } 

        #endregion

        #region Goto

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Br"/>, gotoLabel) to
        /// unconditionally transfers control to a target instruction. 
        /// </summary>
        /// <param name="gotoLabel">The label to go to.</param>
        /// <seealso cref="OpCodes.Br">OpCodes.Br</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public void Goto(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.Emit(OpCodes.Br, gotoLabel);
        }

        ///// <summary>
        ///// If the condition is false, null or zero.
        ///// </summary>
        //public void IfFalse()
        //{
        //    var label = _gen.DefineLabel();
        //    if (_ifElseLabels == null)
        //        _ifElseLabels = new Stack<Label>();
        //    _ifElseLabels.Push(gotoLabel);
        //    _gen.Emit(OpCodes.Brfalse, gotoLabel);
        //}

        ///// <summary>
        ///// If the condition is true, not null or non-zero.
        ///// </summary>
        //public void IfTrue()
        //{
        //    if (_ifElseLabels == null)
        //        _ifElseLabels = new Stack<Label>();
        //    var label = _gen.DefineLabel();
        //    _ifElseLabels.Push(gotoLabel);
        //    _gen.Emit(OpCodes.Brtrue, gotoLabel);
        //}

        //public void ElseIf()
        //{
        //    if (_ifElseLabels == null)
        //        throw new InvalidOperationException("Define [else if] statement without any [if] statement!");
        //    _ifElseLabels.Pop();

        //    var label = _gen.DefineLabel();
        //    _ifElseLabels.Push(gotoLabel);
        //    _gen.Emit(OpCodes.Brtrue, gotoLabel);
        //}

        //public void EndIf()
        //{
        //    if (_ifElseLabels == null)
        //        throw new InvalidOperationException("Define [end if] statement without any [if] statement!");
        //    var label = _ifElseLabels.Pop();
        //    _gen.Emit(OpCodes.Brtrue, gotoLabel);
        //}

        public void IfTrueGoto(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.Emit(OpCodes.Brtrue, gotoLabel);
        }

        public void IfFalseGoto(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.Emit(OpCodes.Brfalse, gotoLabel);
        }

        public void IfEqualsGoto(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.Emit(OpCodes.Beq, gotoLabel);
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Bne_Un"/>, gotoLabel) that
        /// transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.
        /// </summary>
        /// <param name="gotoLabel">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bne_Un">OpCodes.Bne_Un</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public void IfNotEqualsGoto(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.Emit(OpCodes.Bne_Un, gotoLabel);
        }

        public void IfLessThanOrEqualsGoto(Label gotoLabel)
        {
            Requires.NotNull(gotoLabel, "gotoLabel");
            _gen.Emit(OpCodes.Ble, gotoLabel);
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Switch"/>, label[]) that
        /// implements a jump table.
        /// </summary>
        /// <param name="jumpTable">The array of labels to which to branch from this location.</param>
        /// <seealso cref="OpCodes.Switch">OpCodes.Switch</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode,Label[])">ILGenerator.Emit</seealso>
        public void Switch(Label[] jumpTable)
        {
            Requires.NotNull(jumpTable, "jumpTable");
            _gen.Emit(OpCodes.Switch, jumpTable);
        } 

        #endregion

        public void Duplicate()
        {
            _gen.Emit(OpCodes.Dup);
        }

        public void Return()
        {
            _gen.Emit(OpCodes.Ret);
        }

        public void Throw(ConstructorInfo exceptionConstructor)
        {
            Requires.NotNull(exceptionConstructor, "exceptionConstructor");
            var declType = exceptionConstructor.DeclaringType;
            if (!typeof(Exception).IsAssignableFrom(declType))
                throw new ArgumentException(Resources.DeclaringTypeIsNotExceptionOrDerivedFromException);

            _gen.Emit(OpCodes.Newobj, exceptionConstructor);
            _gen.Emit(OpCodes.Throw);
        }

        public void Throw()
        {
            _gen.Emit(OpCodes.Throw);
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Volatile"/>) that
        /// specifies that an address currently atop the evaluation stack might be volatile, 
        /// and the results of reading that location cannot be cached or that multiple stores 
        /// to that location cannot be suppressed.
        /// </summary>
        /// <seealso cref="OpCodes.Volatile">OpCodes.Volatile</seealso>
        /// <seealso cref="ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public void Volatile()
        {
            _gen.Emit(OpCodes.Volatile); 
        }

        public void Substrate()
        {
            _gen.Emit(OpCodes.Sub);
        }
    }
}
