using System;
using My.Helpers;
using My.WinformMvc.Core;
using My.WinformMvc.Extensions;
using My.WinformMvc.Navigation;

namespace My.WinformMvc
{
    public abstract class BaseController : Disposable, IController
    {
        ControllerCloser _closer;
        ICoordinator _coordinator;
        IController _parent;
        readonly IView _view;
        protected readonly ViewNavigation ViewHelper = ViewNavigation.Instance;

        protected BaseController(IView view)
        {
            _view = view;
        }

        /// <summary>
        /// Sets the closer and the controller name, and injected itself into view.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="closer">The closer.</param>
        internal void Initialize(IController parent, ControllerCloser closer)
        {
            var realView = View as BaseView;
            if (realView == null)
                throw new Exception("");
            realView.Initialize(this);

            _parent = parent;
            _closer = closer;
        }

        #region Implement the interface

        /// <summary>
        /// Gets the view.
        /// </summary>
        public IView View
        {
            get { return _view; }
        }

        public IController Parent
        {
            get
            {
                if (_parent == null)
                    throw new Exception("The controller is still initializing. Try not to call this property in the constructor!");
                return _parent;
            }
        }

        public ICoordinator Coordinator
        {
            get
            {
                if (_coordinator == null)
                    throw new Exception("The controller is still initializing. Try not to call this property in the constructor!");
                return _coordinator;
            }
            set
            {
                if (value == null)
                    throw new Exception("");
                if (_coordinator != null)
                    throw new Exception("");
                _coordinator = value;
            }
        }

        public Session Session
        {
            get { return Coordinator.Session; }
        }

        public virtual void InvokeAction(string actionName, object[] parameters)
        {
            Requires.NotNullOrWhiteSpace(actionName, "actionName");
            var actionInvoker = Coordinator.ActionInvokerProvider.GetOrCreate(this, actionName, parameters);
            actionInvoker.InvokeAction(this, parameters);
        }

        #endregion

        #region View Navigation

        /// <summary>
        /// Display this view (Activate or Show), but do not pass a model to it.
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult DisplayView()
        {
            return ViewHelper.DisplayView();
        }

        /// <summary>
        /// Display this view (Activate or Show), and provide a model to populate it.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public virtual IActionResult DisplayView<TModel>(TModel model)
        {
            Requires.NotNull(model, "model");
            return ViewHelper.DisplayView<TModel>(model);
        }

        /// <summary>
        /// Hides this view.
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult HideView()
        {
            return ViewHelper.HideView();
        }

        /// <summary>
        /// Closes this view.
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult CloseView()
        {
            return ViewHelper.CloseView();
        }



        /// <summary>
        /// Invoke specified action of another controller, but do not pass a model to
        /// it, and then close this view.
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        public virtual IActionResult RedirectToAction(string controllerName, string actionName)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            Requires.NotNullOrWhiteSpace(actionName, "actionName");
            return ViewHelper.RedirectToAction(controllerName, actionName);
        }

        /// <summary>
        /// Invoke specified action of another controller, passing the model to it, 
        /// and then close this view.
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public virtual IActionResult RedirectToAction<TModel>(string controllerName, string actionName, TModel model)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            Requires.NotNullOrWhiteSpace(actionName, "actionName");
            Requires.NotNull(model, "model");
            return ViewHelper.RedirectToAction<TModel>(controllerName, actionName, model);
        }

        /// <summary>
        /// Open another view, and then close this view.
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        public virtual IActionResult RedirectTo(string controllerName)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            return ViewHelper.RedirectTo(controllerName);
        }

        /// <summary>
        /// Open another view, passing the model, and then close this view.
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public virtual IActionResult RedirectTo<TModel>(string controllerName, TModel model)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            Requires.NotNull(model, "model");
            return ViewHelper.RedirectTo<TModel>(controllerName, model);
        }

        /// <summary>
        /// Invoke specified action of another controller, but do not pass a model to
        /// it. This view is then deactivated (hide).
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        public virtual IActionResult OpenAction(string controllerName, string actionName)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            Requires.NotNullOrWhiteSpace(actionName, "actionName");
            return ViewHelper.OpenAction(controllerName, actionName);
        }

        /// <summary>
        /// Invoke specified action of another controller, passing the model to it.
        /// This view is then deactivated (hide).
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public virtual IActionResult OpenAction<TModel>(string controllerName, string actionName, TModel model)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            Requires.NotNullOrWhiteSpace(actionName, "actionName");
            Requires.NotNull(model, "model");
            return ViewHelper.OpenAction<TModel>(controllerName, actionName, model);
        }

        /// <summary>
        /// Open another view. This view is then deactivated (hide).
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        public virtual IActionResult Open(string controllerName)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            return ViewHelper.Open(controllerName);
        }

        /// <summary>
        /// Open another view, passing the model. This view is then deactivated (hide).
        /// 1. If the target view does not exists, a new view is created and activated.
        /// 2. If the target view exists, it will be activated simplely.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public virtual IActionResult Open<TModel>(string controllerName, TModel model)
        {
            Requires.NotNullOrWhiteSpace(controllerName, "controllerName");
            Requires.NotNull(model, "model");
            return ViewHelper.Open<TModel>(controllerName, model);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing && _closer != null)
            {
                // Store the reference of [_closer] to a local variable, 
                // and set the [_closer] to null to prevent disposing multiple times.
                var closer = _closer;
                _closer = null;

                var realView = View.Cast<BaseView>();
                realView.CloseThisView();

                closer.Close(this);
            }
        }

        #endregion
    }
}