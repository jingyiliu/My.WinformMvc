using System;

namespace My.WinformMvc
{
	public interface IController : IDisposable
	{
        /// <summary>
        /// The parent that created this controller.
        /// </summary>
	    IController Parent { get; }
	    IView View { get; }
        ICoordinator Coordinator { get; }
	    Session Session { get; }
	    void InvokeAction(string actionName, object[] parameters);

        ///// <summary>
        ///// Display this view (Activate or Show), but do not pass a model to it.
        ///// </summary>
        ///// <returns></returns>
        //IActionResult DisplayView();

        ///// <summary>
        ///// Display this view (Activate or Show), and provide a model to populate it.
        ///// </summary>
        ///// <typeparam name="TModel">The type of the model.</typeparam>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //IActionResult DisplayView<TModel>(TModel model);

        ///// <summary>
        ///// Hides this view.
        ///// </summary>
        ///// <returns></returns>
        //IActionResult HideView();

        ///// <summary>
        ///// Closes this view.
        ///// </summary>
        ///// <returns></returns>
        //IActionResult CloseView();

        ///// <summary>
        ///// Invoke specified action of another controller, but do not pass a model to
        ///// it, and then close this view.
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="actionName">Name of the action.</param>
        ///// <returns></returns>
        //IActionResult RedirectToAction(string controllerName, string actionName);

        ///// <summary>
        ///// Invoke specified action of another controller, passing the model to it, 
        ///// and then close this view.
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <typeparam name="TModel">The type of the model.</typeparam>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="actionName">Name of the action.</param>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //IActionResult RedirectToAction<TModel>(string controllerName, string actionName, TModel model);

        ///// <summary>
        ///// Open another view, and then close this view.
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <returns></returns>
        //IActionResult RedirectTo(string controllerName);

        ///// <summary>
        ///// Open another view, passing the model, and then close this view.
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <typeparam name="TModel">The type of the model.</typeparam>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //IActionResult RedirectTo<TModel>(string controllerName, TModel model);

        ///// <summary>
        ///// Invoke specified action of another controller, but do not pass a model to
        ///// it. This view is then deactivated (hide).
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="actionName">Name of the action.</param>
        ///// <returns></returns>
        //IActionResult OpenAction(string controllerName, string actionName);

        ///// <summary>
        ///// Invoke specified action of another controller, passing the model to it.
        ///// This view is then deactivated (hide).
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <typeparam name="TModel">The type of the model.</typeparam>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="actionName">Name of the action.</param>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //IActionResult OpenAction<TModel>(string controllerName, string actionName, TModel model);

        ///// <summary>
        ///// Open another view. This view is then deactivated (hide).
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <returns></returns>
        //IActionResult Open(string controllerName);

        ///// <summary>
        ///// Open another view, passing the model. This view is then deactivated (hide).
        ///// 1. If the target view does not exists, a new view is created and activated.
        ///// 2. If the target view exists, it will be activated simplely.
        ///// </summary>
        ///// <typeparam name="TModel">The type of the model.</typeparam>
        ///// <param name="controllerName">Name of the controller.</param>
        ///// <param name="model">The model.</param>
        ///// <returns></returns>
        //IActionResult Open<TModel>(string controllerName, TModel model);
	}
}