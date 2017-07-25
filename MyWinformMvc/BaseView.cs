using System;
using System.Windows.Forms;
using My.WinformMvc.Navigation;
using My.WinformMvc.Validation;

namespace My.WinformMvc
{
    /// <summary>
    /// Base view implementation
    /// </summary>
    /// <remarks>Can not be abstract, because the ui designer will throw exception</remarks>
    public partial class BaseView : Form, IView
    {
        IController _controller;

        protected BaseView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the form.
        /// Rewrites the same method of the base type to allow the controller take control of resource cleaning.
        /// </summary>
        public new void Close()
        {
            _controller.Dispose();
        }

        #region Internal methods

        internal void Initialize(IController controller)
        {
            if (controller == null)
                throw new Exception("");
            if (_controller != null)
                throw new Exception("");
            _controller = controller;
        }

        internal void CloseThisView()
        {
            base.Close();
        } 

        #endregion

        public Session Session 
        {
            get { return _controller.Session; }
        }

        /// <summary>
        /// Invoke an action of the controller.
        /// </summary>
        /// <param name="actionName">The action name</param>
        /// <param name="parameters">The parameters.</param>
        public void InvokeAction(string actionName, params object[] parameters)
        {
            _controller.InvokeAction(actionName, parameters);
        }

        public void BindDataSource(object dataSource, string suffix)
        {
            _controller.Coordinator.DataBindingManager.BindDataSource(this, dataSource, suffix);
        }

        public void ShowModelError(ModelState state)
        {
            if (state == null)
                throw new Exception("");
            if (!state.IsValid)
                return;
            DoShowModelError(state); 
        }

        protected virtual void DoShowModelError(ModelState state)
        {
        }

        public void RedirectToView(string targetControllerName)
        {
            _controller.InvokeAction(ActionNames.RedirectTo, new object[] { targetControllerName });
        }

        public void RedirectToView<TModel>(string targetControllerName, TModel model)
        {
            _controller.InvokeAction(ActionNames.RedirectTo, new object[] { targetControllerName, model });
        }

        public void OpenView(string targetControllerName)
        {
            _controller.InvokeAction(ActionNames.Open, new object[] { targetControllerName });
        }

        public void OpenView<TModel>(string targetControllerName, TModel model)
        {
            _controller.InvokeAction(ActionNames.Open, new object[] { targetControllerName, model });
        }

        public void CloseView(string targetControllerName)
        {
            _controller.Coordinator.InvokeControllerAction(_controller, targetControllerName, ActionNames.CloseView, null);
        }

        public void HideView(string targetControllerName)
        {
            _controller.Coordinator.InvokeControllerAction(_controller, targetControllerName, ActionNames.HideView, null);
        }
    }
}
