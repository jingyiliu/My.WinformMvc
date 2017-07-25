
using System;
using System.Text;
using My.Emit;

namespace My.IoC.Injection.Emit
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class EmitInjectorProvider
    {
        readonly EmitAssembly _assembly;
        readonly EmitInjectorBuilder _injectorBuilder;
        readonly EmitParameterMergerBuilder _mergerBuilder;
        static readonly Type BaseMergerType = typeof(EmitParameterMerger);

        public EmitInjectorProvider()
        {
#if DEBUG
            _assembly = new EmitAssembly("Debug", "EmitAssembly.dll");
#else
            _assembly = new EmitAssembly();
#endif
            _injectorBuilder = new EmitInjectorBuilder();
            _mergerBuilder = new EmitParameterMergerBuilder();
        }

        public Type CreateInjectorType(InjectorEmitBody data, string uniqueTypeName)
        {
            var baseType = typeof(Injector<>).MakeGenericType(data.ContractType);
            var typeBuilder = _assembly.DefineType(uniqueTypeName, baseType);
            return _injectorBuilder.BuildType(typeBuilder, data);
        }

        public Type CreateParameterMergerType(int genParamLength)
        {
            //var typeName = GetParameterMergerTypeName(genParamLength);
            var typeBuilder = _assembly.DefineType("EmitParameterMerger", BaseMergerType);
            return _mergerBuilder.BuildType(typeBuilder, genParamLength);
        }

        //static string GetParameterMergerTypeName(int genParamLength)
        //{
        //    var typeName = new StringBuilder("EmitParameterMerger<");
        //    for (int i = 0; i < genParamLength; i++)
        //    {
        //        typeName.Append('T');
        //        typeName.Append(i);
        //        typeName.Append(", ");
        //    }
        //    typeName.Remove(typeName.Length - 2, 2);
        //    typeName.Append('>');
        //    return typeName.ToString();
        //}

#if DEBUG
        public void SaveDynamicAssembly()
        {
            _assembly.Save();
        }
#endif
    }
}
