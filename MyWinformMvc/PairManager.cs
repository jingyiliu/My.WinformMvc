using System;
using System.Collections.Generic;
using System.Reflection;

namespace My.WinformMvc
{
    public class ViewControllerPair
    {
        public string PairName { get; internal set; }
        public Type ControllerType { get; internal set; }
        public Type ViewConcreteType { get; internal set; }
        public Type ViewContractType { get; internal set; }

        public bool Verify()
        {
            return PairName != null && ControllerType != null && ViewConcreteType != null && ViewContractType != null;
        }
    }

    public interface IPairRule
    {
        string GetNameByView(string viewName);
        string GetNameByController(string controllerName);
    }

    class DefaultPairRule : IPairRule
    {
        public string GetNameByView(string viewName)
        {
            return viewName.EndsWith("View", StringComparison.InvariantCultureIgnoreCase)
                ? viewName.Substring(0, viewName.Length - "View".Length) 
                : viewName;
        }

        public string GetNameByController(string controllerName)
        {
            return controllerName.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase)
                ? controllerName.Substring(0, controllerName.Length - "Controller".Length)
                : controllerName;
        }
    }

    public class PairManager : IPairManager
    {
        readonly Type _baseControllerType = typeof(BaseController);
        readonly Type _controllerAttrib = typeof(MvcControllerAttribute);
        readonly Type _baseViewType = typeof(BaseView);
        readonly Type _viewInterfaceType = typeof(IView);
        readonly Type _viewAttrib = typeof(MvcViewAttribute);

        readonly IPairRule _pairRule;
        readonly Dictionary<string, ViewControllerPair> _ctrlName2Pairs
            = new Dictionary<string, ViewControllerPair>();

        public PairManager() : this (new DefaultPairRule())
        {
        }
        public PairManager(IPairRule pairRule)
        {
            _pairRule = pairRule;
        }

        public IPairRule PairRule
        {
            get { return _pairRule; }
        }

        public IEnumerable<ViewControllerPair> ViewControllerPairs
        {
            get { return _ctrlName2Pairs.Values; }
        }

        public void RegisterController(Type controllerType)
        {
            if (!VerifyType(_baseControllerType, controllerType))
                throw new Exception();

            var attribs = controllerType.GetCustomAttributes(_controllerAttrib, false);
            if (attribs.Length == 0)
                throw new Exception();

            var attrib = attribs[0] as MvcControllerAttribute;
            RegisterControllerType(controllerType, attrib);
        }

        public void RegisterView(Type viewType)
        {
            if (!VerifyType(_baseViewType, viewType))
                throw new Exception();

            var attribs = viewType.GetCustomAttributes(_viewAttrib, false);
            if (attribs.Length == 0)
                throw new Exception();

            var attrib = attribs[0] as MvcViewAttribute;
            RegisterViewType(viewType, attrib);
        }

        public void RegisterAssembly(Assembly assembly)
        {
            // Get all public types defined in the assembly
            var publicTypes = assembly.GetTypes();
            foreach (var type in publicTypes)
            {
                if (!IsConcreteType(type))
                    continue;

                if (_baseControllerType.IsAssignableFrom(type))
                {
                    var attribs = type.GetCustomAttributes(_controllerAttrib, false);
                    if (attribs.Length == 1)
                    {
                        var attrib = attribs[0] as MvcControllerAttribute;
                        RegisterControllerType(type, attrib);
                    }
                }
                else if (_baseViewType.IsAssignableFrom(type))
                {
                    var attribs = type.GetCustomAttributes(_viewAttrib, false);
                    if (attribs.Length == 1)
                    {
                        var attrib = attribs[0] as MvcViewAttribute;
                        RegisterViewType(type, attrib);
                    }
                }
            }
        }

        void RegisterControllerType(Type controllerType, MvcControllerAttribute attrib)
        {
            var pairName = _pairRule.GetNameByController(attrib.Name);
            ViewControllerPair pair;
            if (_ctrlName2Pairs.TryGetValue(pairName, out pair))
            {
                if (pair.ControllerType != null || pair.PairName != null || pair.ViewContractType != null)
                    throw new Exception("Register duplicatedly!");
            }
            else
            {
                pair = new ViewControllerPair();
                _ctrlName2Pairs.Add(pairName, pair);
            }
            pair.ViewContractType = GetViewContractType(controllerType);
            pair.ControllerType = controllerType;
            pair.PairName = pairName;
        }

        void RegisterViewType(Type viewType, MvcViewAttribute attrib)
        {
            // View to controller mapping
            var pairName = _pairRule.GetNameByView(attrib.Name);
            ViewControllerPair pair;
            if (_ctrlName2Pairs.TryGetValue(pairName, out pair))
            {
                if (pair.ViewConcreteType != null)
                    throw new Exception("Register duplicatedly!");
            }
            else
            {
                pair = new ViewControllerPair();
                _ctrlName2Pairs.Add(pairName, pair);
            }
            pair.ViewConcreteType = viewType;
        }

        Type GetViewContractType(Type controllerType)
        {
            var ctors = controllerType.GetConstructors();
            if (ctors.Length > 1)
                throw new Exception(string.Format("Multiple constructors exists for the controller [{0}], which is not supported!", controllerType.FullName));
            var parameters = ctors[0].GetParameters();

            Type viewContract = null;
            foreach (var parameter in parameters)
            {
                if (!_viewInterfaceType.IsAssignableFrom(parameter.ParameterType))
                    continue;
                if (viewContract != null)
                    throw new Exception("Multiple views needed for the constructor!");
                viewContract = parameter.ParameterType;
            }
            return viewContract;
        }

        static bool VerifyType(Type baseType, Type targetType)
        {
            return targetType.IsClass && !targetType.IsInterface && !targetType.IsAbstract 
                && baseType.IsAssignableFrom(targetType);
        }

        static bool IsConcreteType(Type targetType)
        {
            return targetType.IsClass && !targetType.IsInterface && !targetType.IsAbstract;
        }

        public void VerifyPairs()
        {
            var bindings = _ctrlName2Pairs.Values;
            foreach (var binding in bindings)
            {
                if (!binding.Verify())
                    throw new Exception("");
            }
        }

        public bool TryGetViewControllerPair(string pairName, out ViewControllerPair viewControllerPair)
        {
            return _ctrlName2Pairs.TryGetValue(pairName, out viewControllerPair);
        }
    }
}
