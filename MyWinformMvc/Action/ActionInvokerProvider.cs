using System;
using System.Collections.Generic;
using System.Reflection;

namespace My.WinformMvc.Action
{
    public class ActionInvokerProvider
    {
        abstract class MethodMetadata
        {
            readonly MethodInfo _methodInfo;
            readonly Type[] _parameterTypes;

            protected MethodMetadata(MethodInfo methodInfo, Type[] parameterTypes)
            {
                _methodInfo = methodInfo;
                _parameterTypes = parameterTypes;
            }

            internal MethodInfo MethodInfo { get { return _methodInfo; } }
            internal Type[] ParameterTypes { get { return _parameterTypes; } }

            internal abstract bool IsGeneric { get; }
            internal abstract bool Match(Type[] parameterTypes);
            // Get non-open-generic method, i.e, close generic method or non-generic method
            internal abstract MethodInfo GetCallableMethod(Type[] parameterTypes);

            protected bool MatchParameterLength(Type[] parameterTypes)
            {
                if ((ParameterTypes != null && parameterTypes == null)
                    || (ParameterTypes == null && parameterTypes != null))
                    return false;
                return parameterTypes == null || ParameterTypes.Length == parameterTypes.Length;
            }
        }

        class NormalMethodMetadata : MethodMetadata
        {
            internal NormalMethodMetadata(MethodInfo methodInfo, Type[] parameterTypes)
                : base(methodInfo, parameterTypes)
            { }

            internal override bool IsGeneric
            {
                get { return false; }
            }

            internal override bool Match(Type[] parameterTypes)
            {
                if (!MatchParameterLength(parameterTypes))
                    return false;
                if (parameterTypes == null)
                    return true;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    var methodParamType = ParameterTypes[i];
                    if (methodParamType.IsAssignableFrom(parameterTypes[i]))
                        continue;
                    return false;
                }
                return true;
            }

            internal override MethodInfo GetCallableMethod(Type[] parameterTypes)
            {
                return MethodInfo;
            }
        }

        class GenericMethodMetadata : MethodMetadata
        {
            readonly List<int> _genericPositions = new List<int>();

            internal GenericMethodMetadata(MethodInfo methodInfo, Type[] parameterTypes)
                : base(methodInfo, parameterTypes)
            {
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameterTypes[i].IsGenericParameter)
                        _genericPositions.Add(i);
                }
            }

            internal override bool IsGeneric
            {
                get { return true; }
            }

            internal override bool Match(Type[] parameterTypes)
            {
                if (!MatchParameterLength(parameterTypes))
                    return false;
                if (parameterTypes == null)
                    return true;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    var methodParamType = ParameterTypes[i];
                    // Check generic constraint here
                    if (methodParamType.IsGenericParameter || methodParamType.IsAssignableFrom(parameterTypes[i]))
                        continue;
                    return false;
                }
                return true;
            }

            internal override MethodInfo GetCallableMethod(Type[] parameterTypes)
            {
                var genericTypes = new Type[_genericPositions.Count];
                for (int i = 0; i < _genericPositions.Count; i++)
                    genericTypes[i] = parameterTypes[_genericPositions[i]];
                return MethodInfo.MakeGenericMethod(genericTypes);
            }
        }

        abstract class ActionMethodSelector
        {
            /// <summary>
            /// Gets the action method.
            /// Do not support the [out], [ref] parameter and param array.
            /// </summary>
            /// <param name="parameterTypes">The parameter types.</param>
            /// <returns></returns>
            internal abstract MethodInfo SelectActionMethod(Type[] parameterTypes);

            static protected MethodMetadata GetMethodMetadata(MethodInfo namedMethod)
            {
                var parameters = namedMethod.GetParameters();
                Type[] parameterTypes;
                if (parameters.Length == 0)
                {
                    parameterTypes = null;
                }
                else
                {
                    parameterTypes = new Type[parameters.Length];
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        var parameter = parameters[j];
                        if (parameter.ParameterType.IsByRef)
                            throw new Exception("The out or ref parameter is invalid for controller action method!");
                        parameterTypes[j] = parameter.ParameterType;
                    }
                }

                MethodMetadata metadata;
                if (namedMethod.IsGenericMethod)
                    metadata = new GenericMethodMetadata(namedMethod, parameterTypes);
                else
                    metadata = new NormalMethodMetadata(namedMethod, parameterTypes);
                return metadata;
            }
        }

        class NormalActionMethodSelector : ActionMethodSelector
        {
            readonly MethodMetadata _metadata;

            internal NormalActionMethodSelector(MethodInfo namedMethod)
            {
                _metadata = GetMethodMetadata(namedMethod);
            }

            internal override MethodInfo SelectActionMethod(Type[] parameterTypes)
            {
                if (!_metadata.Match(parameterTypes))
                    throw new Exception("Can not find a matching method for the provided parameters!");
                return _metadata.GetCallableMethod(parameterTypes);
            }
        }

        class OverrideActionMethodSelector : ActionMethodSelector
        {
            readonly MethodMetadata[] _metadataList;

            internal OverrideActionMethodSelector(List<MethodInfo> namedMethods)
            {
                _metadataList = new MethodMetadata[namedMethods.Count];
                for (int i = 0; i < namedMethods.Count; i++)
                {
                    var namedMethod = namedMethods[i];
                    MethodMetadata metadata = GetMethodMetadata(namedMethod);
                    _metadataList[i] = metadata;
                }
            }

            internal override MethodInfo SelectActionMethod(Type[] parameterTypes)
            {
                MethodMetadata target = null;
                foreach (var metadata in _metadataList)
                {
                    if (!metadata.Match(parameterTypes))
                        continue;
                    if (!metadata.IsGeneric)
                        return metadata.MethodInfo;
                    target = metadata;
                }
                if (target != null)
                    // 代码运行到此，如果能找到，找到的一定是泛型方法重载，而且是包含参数的泛型方法重载
                    return target.GetCallableMethod(parameterTypes);
                throw new Exception("Can not find an appropriate override method with the parameters provided for the controller!");
            }
        }

        struct Key
        {
            public int HashCode { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public Type[] ParameterTypes { get; set; }
        }

        class KeyComparer : IEqualityComparer<Key>
        {
            public bool Equals(Key x, Key y)
            {
                if ((x.ParameterTypes != null && y.ParameterTypes == null)
                    || (x.ParameterTypes == null && y.ParameterTypes != null))
                    return false;
                if (x.ParameterTypes == null)
                    return x.ControllerName.Equals(y.ControllerName) && x.ActionName.Equals(y.ActionName);

                if (x.ParameterTypes.Length != y.ParameterTypes.Length)
                    return false;
                for (int i = 0; i < x.ParameterTypes.Length; i++)
                {
                    if (x.ParameterTypes[i] != y.ParameterTypes[i])
                        return false;
                }
                return x.ControllerName.Equals(y.ControllerName) && x.ActionName.Equals(y.ActionName);
            }

            public int GetHashCode(Key obj)
            {
                return obj.HashCode;
            }
        }

        readonly object _syncObj = new object();
        readonly Dictionary<Key, IActionInvoker> _actionInvokers
            = new Dictionary<Key, IActionInvoker>(new KeyComparer());

        static Type[] GetParameterTypes(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return null;
            var paramTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                paramTypes[i] = parameters[i].GetType();
            return paramTypes;
        }

        static Key CreateKey(string controllerName, string actionName, Type[] parameterTypes)
        {
            var hashCode = controllerName.GetHashCode() ^ actionName.GetHashCode();
            if (parameterTypes != null)
            {
                for (int i = 0; i < parameterTypes.Length; i++)
                    hashCode ^= parameterTypes[i].GetHashCode();
            }
            return new Key
            {
                HashCode = hashCode,
                ControllerName = controllerName,
                ActionName = actionName,
                ParameterTypes = parameterTypes
            };
        }

        static MethodInfo SelectActionMethod(IController controller, string actionName, Type[] parameterTypes)
        {
            var controllerType = controller.GetType();
            var publicMethods = controllerType.GetMethods();
            if (publicMethods.Length == 0)
                throw new Exception(string.Format("No public methods found for the controller [{0}]!", controller.GetType().FullName));

            var namedMethods = new List<MethodInfo>();
            foreach (var publicMethod in publicMethods)
            {
                if (actionName.Equals(publicMethod.Name))
                    namedMethods.Add(publicMethod);
            }

            if (namedMethods.Count == 0)
                throw new Exception(string.Format("No public methods named [{0}] found for the controller [{1}]!", actionName, controller.GetType().FullName));

            ActionMethodSelector selector;
            if (namedMethods.Count == 1)
                selector = new NormalActionMethodSelector(namedMethods[0]);
            else
                selector = new OverrideActionMethodSelector(namedMethods);

            var actionMethod = selector.SelectActionMethod(parameterTypes);
            if (!typeof(IActionResult).IsAssignableFrom(actionMethod.ReturnType))
                throw new Exception(string.Format("Invalid action method [{0}] (does not return IActionResult) found for the controller [{1}]!", actionName, controller.GetType().FullName));

            return actionMethod;
        }

        public IActionInvoker GetOrCreate(IController controller, string actionName, object[] parameters)
        {
            var paramTypes = GetParameterTypes(parameters);
            var key = CreateKey(controller.GetType().FullName, actionName, paramTypes);
            lock (_syncObj)
            {
                IActionInvoker actionInvoker;
                if (_actionInvokers.TryGetValue(key, out actionInvoker))
                    return actionInvoker;

                var actionMethod = SelectActionMethod(controller, actionName, paramTypes);
                actionInvoker = new DefaultActionInvoker(actionMethod);

                _actionInvokers.Add(key, actionInvoker);
                return actionInvoker;
            }
        }
    }
}
