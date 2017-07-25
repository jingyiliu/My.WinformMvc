using System;
using System.Windows.Forms;
using My.WinformMvc.Action;
using My.WinformMvc.Core;
using My.WinformMvc.DataBinding;
using My.WinformMvc.Navigation;

namespace My.WinformMvc
{
    public class Coordinator : Disposable, ICoordinator
    {
        enum ApplicationState
        {
            Unstarted,
            Starting,
            Started
        }

        ApplicationState _state = ApplicationState.Unstarted;
	    readonly Session _session;
        readonly DataBindingManager _dataBindingManager;
        readonly ActionInvokerProvider _actionInvokerProvider;
        readonly IPairManager _pairManager;
        readonly IControllerManager _controllerManager;

        public Coordinator(IPairManager pairManager, IIocWrapper iocWrapper)
	    {
            pairManager.VerifyPairs();
            _pairManager = pairManager;

            _session = new Session();
            _dataBindingManager = new DataBindingManager();
            _actionInvokerProvider = new ActionInvokerProvider();

            iocWrapper.RegisterTypes(this); // Register the ICoordinator itself and all controller and view types
            _controllerManager = new ControllerManager(iocWrapper, pairManager);
	    }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <remarks>Stores application-wide data, eg., the user principal</remarks>
        public Session Session
        {
            get { return _session; }
        }

        public DataBindingManager DataBindingManager
        {
            get { return _dataBindingManager; }
        }

        public ActionInvokerProvider ActionInvokerProvider
        {
            get { return _actionInvokerProvider; }
        }

        public IPairManager PairManager
        {
            get { return _pairManager; }
        }

        public void InvokeControllerAction(IController sourceController, string targetControllerName, string targetActionName, object[] parameters)
        {
            if (_state == ApplicationState.Unstarted)
                throw new Exception("The application has not started, please start it first (call StartApplication method)!");

            var targetPairName = GetPairNameByController(targetControllerName);
            var targetController = GetTargetController(sourceController, targetPairName);
            targetController.InvokeAction(targetActionName, parameters);
		}

        public void StartApplication(string defaultControllerName)
        {
            if (_state == ApplicationState.Starting || _state == ApplicationState.Started)
                throw new Exception("The application has been started already and should not restart again!");

            _state = ApplicationState.Starting;
            var targetPairName = GetPairNameByController(defaultControllerName);
            var targetController = GetTargetController(null, targetPairName);
            targetController.InvokeAction(ActionNames.DisplayView, null);
            Application.Run((BaseView)targetController.View);
            _state = ApplicationState.Started;
        }

        string GetPairNameByController(string controllerName)
        {
            return _pairManager.PairRule.GetNameByController(controllerName);
        }

        //string GetPairNameByView(string viewName)
        //{
        //    return _pairManager.PairRule.GetNameByView(viewName);
        //}

        //public void RedirectToView(IController sourceController, string targetPairName)
        //{
        //    ShowView(sourceController, targetPairName);
        //}

        //public void RedirectToView<TModel>(IController sourceController, string targetPairName, TModel model)
        //{
        //    ShowView(sourceController, targetPairName, model);
        //}

        //public void CloseView(IController sourceController, string targetPairName)
        //{
        //    var targetController = GetTargetController(sourceController, targetPairName);
        //    targetController.InvokeAction(ActionNames.CloseView, null);
        //}

        //public void HideView(IController sourceController, string targetPairName)
        //{
        //    var targetController = GetTargetController(sourceController, targetPairName);
        //    targetController.InvokeAction(ActionNames.HideView, null);
        //}

        //void ShowView<TModel>(IController sourceController, string targetPairName, TModel model)
        //{
        //    var targetController = GetTargetController(sourceController, targetPairName);
        //    targetController.InvokeAction(ActionNames.DisplayView, new object[] { model });
        //    if (!_inMessageLoop)
        //    {
        //        _inMessageLoop = true;
        //        Application.Run((BaseView)targetController.View);
        //    }
        //}

        //void ShowView(IController sourceController, string targetPairName)
        //{
        //    var targetController = GetTargetController(sourceController, targetPairName);
        //    targetController.InvokeAction(ActionNames.DisplayView, null);
        //    if (!_inMessageLoop)
        //    {
        //        _inMessageLoop = true;
        //        Application.Run((BaseView)targetController.View);
        //    }
        //}

        IController GetTargetController(IController sourceController, string targetPairName)
        {
            return _controllerManager.GetOrCreateController(sourceController, targetPairName);
        }
    }
}