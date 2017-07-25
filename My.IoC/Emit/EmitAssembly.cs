
using System;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;
using My.Helpers;
using My.IoC;

namespace My.Emit
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EmitAssembly
    {
        const string DefaultAssemblyFileName = "EmitAssembly.dll";
        readonly string _assemblyDirectory;
        readonly string _assemblyFileName;
        readonly AssemblyBuilder _asmBuilder;
        readonly ModuleBuilder _modBuilder;

        public EmitAssembly()
        {
            var defaultAssemblyName = Path.GetFileNameWithoutExtension(DefaultAssemblyFileName);
            var asmName = new AssemblyName { Name = defaultAssemblyName };
            _asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            _modBuilder = _asmBuilder.DefineDynamicModule(defaultAssemblyName);
        }

        public EmitAssembly(string assemblyDirectoryName, string assemblyFileName)
        {
            Requires.NotNull(assemblyDirectoryName, "assemblyDirectoryName");
            VerifyDirectoryPath(assemblyDirectoryName);
            _assemblyDirectory = GetAssemblyDirectory(assemblyDirectoryName);

            string assemblyName;
            if (assemblyFileName != null)
            {
                VerifyFileName(assemblyFileName);
                if (assemblyFileName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)
                    || assemblyFileName.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    _assemblyFileName = assemblyFileName;
                    assemblyName = Path.GetFileNameWithoutExtension(assemblyFileName);
                }
                else
                {
                    throw new IOException(Resources.InvalidAssemblyFileName + assemblyFileName);
                }
            }
            else
            {
                GetDefaultFileName(out assemblyName, out _assemblyFileName);
            }

            var asmName = new AssemblyName { Name = assemblyName };
            _asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave, _assemblyDirectory);
            _modBuilder = _asmBuilder.DefineDynamicModule(assemblyName, _assemblyFileName);
        }

        string GetAssemblyDirectory(string assemblyDirectoryName)
        {
            string directory;
            if (Path.IsPathRooted(assemblyDirectoryName))
            {
                directory = assemblyDirectoryName;
            }
            else
            {
                var thisAssemblyLocation = GetType().Assembly.Location;
                var thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyLocation);
                directory = Path.Combine(thisAssemblyDirectory, assemblyDirectoryName);
            }
            return directory;
        }

        public string AssemblyDirectory
        {
            get { return _assemblyDirectory; }
        }

        public string AssemblyFileName
        {
            get { return _assemblyFileName; }
        }

        public AssemblyBuilder Assembly
        {
            get { return _asmBuilder; }
        }

        public ModuleBuilder Module
        {
            get { return _modBuilder; }
        }

        static void VerifyDirectoryPath(string assemblyDirectory)
        {
            foreach (var @char in Path.GetInvalidPathChars())
            {
                foreach (var dirChar in assemblyDirectory)
                {
                    if (dirChar == @char)
                        throw new IOException(Resources.InvalidDirectoryPath + assemblyDirectory);
                }
            }
        }

        static void VerifyFileName(string assemblyFileName)
        {
            foreach (var @char in Path.GetInvalidFileNameChars())
            {
                foreach (var dirChar in assemblyFileName)
                {
                    if (dirChar == @char)
                        throw new IOException(Resources.InvalidFileName + assemblyFileName);
                }
            }
        }

        void GetDefaultFileName(out string assemblyName, out string assemblyFileName)
        {
            assemblyFileName = DefaultAssemblyFileName + ".dll";
            var filePath = Path.Combine(_assemblyDirectory, assemblyFileName);
            if (!File.Exists(filePath))
            {
                assemblyName = DefaultAssemblyFileName;
                return;
            }

            for (var i = 0; i < int.MaxValue; i++)
            {
                assemblyFileName = DefaultAssemblyFileName + i + ".dll";
                filePath = Path.Combine(_assemblyDirectory, assemblyFileName);
                if (!File.Exists(filePath))
                {
                    assemblyName = DefaultAssemblyFileName + i;
                    return;
                }
            }

            throw new IOException(Resources.CanNotGetValidAssemblyFileName);
        }

        public TypeBuilder DefineType(string className, Type baseType, params Type[] interfaceTypes)
        {
            var typeBuilder = _modBuilder.DefineType(className, TypeAttributes.Public, baseType, interfaceTypes);
            return typeBuilder;
        }

        public void Save()
        {
            CreateDirectoryIfNeccessary();
            _asmBuilder.Save(_assemblyFileName);
        }

        void CreateDirectoryIfNeccessary()
        {
            if (!Directory.Exists(_assemblyDirectory))
                Directory.CreateDirectory(_assemblyDirectory);
        }
    }
}
