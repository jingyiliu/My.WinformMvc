
using System;
using System.Collections.Generic;
using System.Reflection;
using My.IoC.Helpers;
using My.Threading;

namespace My.IoC.Injection.Emit
{
	class EmitInjectorManager
	{
	    int _typeIndex = 0;
        readonly ILock _injectorLock;
        readonly ILock _mergerLock;
        readonly EmitInjectorProvider _provider;
        readonly Dictionary<EmitInjectorKey, Type> _key2Injector;
        readonly Dictionary<int, Type> _length2Merger;

	    public EmitInjectorManager()
	    {
	        if (SystemHelper.MultiProcessors)
	        {
	            _injectorLock = new SpinLockSlim();
                _mergerLock = new SpinLockSlim();
	        }
	        else
	        {
	            _injectorLock = new MonitorLock();
                _mergerLock = new MonitorLock();
	        }
            
            _provider = new EmitInjectorProvider();
            _key2Injector = new Dictionary<EmitInjectorKey, Type>();
            _length2Merger = new Dictionary<int, Type>();
	    }

	    string GetUniqueDynamicTypeName()
	    {
            _typeIndex++;
            //return "Class" + _typeIndex.ToString();
            return _typeIndex.ToString();
            //return new Guid().ToString();
	    }

        public Type GetOrCreateInjectorType(InjectorEmitBody emitBody)
        {
            EmitInjectorKey key;
            var methodEmitBodySet = emitBody.MethodEmitBodySet;
            if (methodEmitBodySet != null)
            {
                var injectedMethods = new MethodInfo[methodEmitBodySet.Length];
                for (int i = 0; i < methodEmitBodySet.Length; i++)
                {
                    var methodEmitBody = methodEmitBodySet[i];
                    injectedMethods[i] = methodEmitBody.InjectedMethod;
                }
                key = new EmitConstructorAndMemberInjectorKey(emitBody.ConstructorEmitBody.InjectedConstructor, injectedMethods);
            }
            else
            {
                key = new EmitInjectorKey(emitBody.ConstructorEmitBody.InjectedConstructor);
            }

            _injectorLock.Enter();
            try
            {
                Type injectorType;
                if (_key2Injector.TryGetValue(key, out injectorType))
                    return injectorType;

                injectorType = _provider.CreateInjectorType(emitBody, GetUniqueDynamicTypeName());
                _key2Injector.Add(key, injectorType);
                return injectorType;
            }
            finally
            {
                _injectorLock.Exit();
            }
		}

        public Type GetOrCreateParameterMergerType(int paramLength)
	    {
            //if (paramLength > 8)
            //    throw new ArgumentOutOfRangeException("Can not build injectors for constructor with more than 8 parameters right now. This function is coming soon...");

            if (paramLength == 0)
                throw new InvalidOperationException();

            var result = GetDefaultParameterMergerType(paramLength);
            if (result != null)
                return result;

            _mergerLock.Enter();
            try
            {
                if (_length2Merger.TryGetValue(paramLength, out result))
                    return result;

                result = _provider.CreateParameterMergerType(paramLength);
                _length2Merger.Add(paramLength, result);
                return result;
            }
            finally
            {
                _mergerLock.Exit();
            }
	    }

        static Type GetDefaultParameterMergerType(int parameterLength)
        {
            switch (parameterLength)
            {
                case 1:
                    return typeof(EmitParameterMerger<>);
                case 2:
                    return typeof(EmitParameterMerger<,>);
                case 3:
                    return typeof(EmitParameterMerger<,,>);
                case 4:
                    return typeof(EmitParameterMerger<,,,>);
                case 5:
                    return typeof(EmitParameterMerger<,,,,>);
                case 6:
                    return typeof(EmitParameterMerger<,,,,,>);
                case 7:
                    return typeof(EmitParameterMerger<,,,,,,>);
                case 8:
                    return typeof(EmitParameterMerger<,,,,,,,>);
                //case 9:
                //    return typeof(EmitParameterBuilder<,,,,,,,,>);
                //case 10:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,>);
                //case 11:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,,>);
                //case 12:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,,,>);
                //case 13:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,,,,>);
                //case 14:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,,,,,>);
                //case 15:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,,,,,,>);
                //case 16:
                //    return typeof(EmitParameterBuilder<,,,,,,,,,,,,,,,>);
                default:
                    return null;
            }
        }

#if DEBUG
        public void SaveDynamicAssembly()
        {
            _provider.SaveDynamicAssembly();
        }
#endif
	}
}
