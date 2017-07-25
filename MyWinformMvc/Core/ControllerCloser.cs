using System;

namespace My.WinformMvc.Core
{
    class ControllerCloser
    {
        readonly string _pairName;
        readonly IControllerManager _controllerManager;
        readonly IDisposable _iocLifetimeScope;

        internal ControllerCloser(IControllerManager controllerManager, IDisposable iocLifetimeScope, string pairName)
        {
            _iocLifetimeScope = iocLifetimeScope;
            _controllerManager = controllerManager;
            _pairName = pairName;
        }

        internal void Close(IController controller)
        {
            _iocLifetimeScope.Dispose();
            _controllerManager.RemoveController(controller, _pairName);
        }
    }
}