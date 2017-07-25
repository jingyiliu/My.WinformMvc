using My.WinformMvc.Validation;

namespace My.WinformMvc
{
	/// <summary>
	/// Base Interface for view
	/// </summary>
	public interface IView
	{
	    bool Visible { get; }
	    void Hide();
	    void Activate();
	    void Show();
	    void Close();

        /// <summary>
        /// Invoke an action of the binding controller.
        /// </summary>
        /// <param name="actionName">The action name</param>
        /// <param name="parameters">The parameters.</param>
	    void InvokeAction(string actionName, params object[] parameters);
        /// <summary>
        /// Binds the data source to the view based on the convention.
        /// Please note that this is a two way data binding.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="suffix">The suffix.</param>
	    void BindDataSource(object dataSource, string suffix);
        void ShowModelError(ModelState state);

        /// <summary>
        /// Open another view, but do not close this view (It is deactivated).
        /// </summary>
        /// <param name="targetControllerName">Name of the target controller.</param>
        void OpenView(string targetControllerName);
        /// <summary>
        /// Open another view and provide a model to populate it, but do not close this view
        /// (It is deactivated).
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="targetControllerName">Name of the target controller.</param>
        /// <param name="model">The model.</param>
        void OpenView<TModel>(string targetControllerName, TModel model);

        /// <summary>
        /// Open another view and close this view.
        /// </summary>
        /// <param name="targetControllerName">Name of the target controller.</param>
        void RedirectToView(string targetControllerName);
        /// <summary>
        /// Open another view, provide a model to populate it, and close this view.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="targetControllerName">Name of the target controller.</param>
        /// <param name="model">The model.</param>
        void RedirectToView<TModel>(string targetControllerName, TModel model);

        /// <summary>
        /// Closes the specified view. This can be useful when running in MDI mode.
        /// </summary>
        /// <param name="targetControllerName">Name of the target controller.</param>
        void CloseView(string targetControllerName);
        /// <summary>
        /// Hides the specified view. This can be useful when running in MDI mode.
        /// </summary>
        /// <param name="targetControllerName">Name of the target controller.</param>
        void HideView(string targetControllerName);
	}

    public interface IView<TModel> : IView
    {
        void BindModel(TModel model);
    }
}