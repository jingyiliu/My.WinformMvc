using System;
using My.WinformMvc.Action;
using My.WinformMvc.DataBinding;

namespace My.WinformMvc
{
    public interface ICoordinator : IDisposable
    {
        Session Session { get; }
        DataBindingManager DataBindingManager { get; }
        ActionInvokerProvider ActionInvokerProvider { get; }
        IPairManager PairManager { get; }

        /// <summary>
        /// Start the application (Open the first view of application).
        /// </summary>
        /// <param name="defaultControllerName">Default name of the controller.</param>
        void StartApplication(string defaultControllerName);
        // Can only be called by IController, because we use [controllerName] here
        void InvokeControllerAction(IController sourceController, string targetControllerName, string targetActionName, object[] parameters);

        //string GetPairNameByView(string viewName);
        //string GetPairNameByController(string controllerName);

        //#region For call from IView and IController both

        /////// <summary>
        /////// Open another view, and close the current view.
        /////// </summary>
        /////// <param name="sourceController">The source controller.</param>
        /////// <param name="targetPairName">Name of the target pair.</param>
        ////void RedirectToView(IController sourceController, string targetPairName);
        /////// <summary>
        /////// Open another view, provide a model to populate it, and close the current view.
        /////// </summary>
        /////// <typeparam name="TModel">The type of the model.</typeparam>
        /////// <param name="sourceController">The source controller.</param>
        /////// <param name="targetPairName">Name of the target pair.</param>
        /////// <param name="model">The model.</param>
        ////void RedirectToView<TModel>(IController sourceController, string targetPairName, TModel model);

        /////// <summary>
        /////// Open another view, but do not close the current view (It is deactivated).
        /////// </summary>
        /////// <param name="sourceController">The source controller.</param>
        /////// <param name="targetPairName">Name of the target pair.</param>
        ////void OpenView(IController sourceController, string targetPairName);
        /////// <summary>
        /////// Open another view, provide a model to populate it, but do not close the current 
        /////// view (It is deactivated).
        /////// </summary>
        /////// <typeparam name="TModel">The type of the model.</typeparam>
        /////// <param name="sourceController">The source controller.</param>
        /////// <param name="targetPairName">Name of the target pair.</param>
        /////// <param name="model">The model.</param>
        ////void OpenView<TModel>(IController sourceController, string targetPairName, TModel model);

        /////// <summary>
        /////// Closes the specified view.
        /////// This can be useful when running in MDI mode, in which case we can close a child view from the MDI parent.
        /////// </summary>
        /////// <param name="sourceController">The source controller.</param>
        /////// <param name="targetPairName">Name of the target pair.</param>
        ////void CloseView(IController sourceController, string targetPairName);
        /////// <summary>
        /////// Hides the specified view.
        /////// This can be useful when running in MDI mode, in which case we can close a child view from the MDI parent.
        /////// </summary>
        /////// <param name="sourceController">The source controller.</param>
        /////// <param name="targetPairName">Name of the target pair.</param>
        ////void HideView(IController sourceController, string targetPairName);

        //#endregion
    }
}
