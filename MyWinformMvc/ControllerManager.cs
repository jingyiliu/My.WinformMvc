using System;
using System.Collections.Generic;
using My.WinformMvc.Core;
using My.WinformMvc.Properties;

namespace My.WinformMvc
{
    public class ControllerManager : IControllerManager
    {
        readonly IIocWrapper _iocWrapper;
        readonly IPairProvider _pairProvider;
        // No lock is needed here, because the UI is a single thread apartment
        readonly Dictionary<string, BaseController> _openedControllers = new Dictionary<string, BaseController>();

        internal ControllerManager(IIocWrapper iocWrapper, IPairProvider pairProvider)
        {
            _iocWrapper = iocWrapper;
            _pairProvider = pairProvider;
        }

        public IPairProvider PairProvider
        {
            get { return _pairProvider; }
        }

        public BaseController GetOrCreateController(IController sourceController, string targetPairName)
        {
            BaseController controller;
            if (!_openedControllers.TryGetValue(targetPairName, out controller))
                controller = CreateController(sourceController, targetPairName, true);
            return controller;
        }

        public void RemoveController(IController controller, string pairName)
        {
            // TODO: Can not remove opened controller with children
            //if (_current == null || _current.Parent == controller)
            //    throw new Exception();
            BaseController openedController;
            if (!_openedControllers.TryGetValue(pairName, out openedController))
                throw new Exception("The controller has not been created yet!");
            if (!ReferenceEquals(controller, openedController))
                throw new Exception("");
            _openedControllers.Remove(pairName);
        }

        //public BaseController CreateController(string pairName)
        //{
        //    return CreateController(pairName, true);
        //}

        //public bool TryGetController(string pairName, out BaseController controller)
        //{
        //    return _openedControllers.TryGetValue(pairName, out controller);
        //}

        BaseController CreateController(IController sourceController, string targetPairName, bool throwIfFailed)
        {
            ViewControllerPair viewControllerPair;
            if (!_pairProvider.TryGetViewControllerPair(targetPairName, out viewControllerPair)
                || viewControllerPair.ControllerType == null
                || viewControllerPair.ViewConcreteType == null)
            {
                // If the binding is incompleted, throw
                if (!throwIfFailed)
                    return null;
                Logger.Error(string.Format(Resources.ControllerMissingException, targetPairName));
                throw new Exception(string.Format(Resources.ControllerMissingException, targetPairName));
            }

            IDisposable lifetimeScope;
            var objController = _iocWrapper.GetInstance(viewControllerPair.ControllerType, out lifetimeScope);
            var controller = objController as BaseController;
            if (controller == null)
                throw new Exception("");
            _openedControllers.Add(targetPairName, controller);

            // Initialize the Controller
            // Since there is no controller existed for the specified controller name, so we know
            // that this is a child of current controller, if there is a current controller.
            controller.Initialize(sourceController, new ControllerCloser(this, lifetimeScope, targetPairName));

            return controller;
        }
    }
}
