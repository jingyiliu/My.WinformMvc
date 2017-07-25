namespace My.WinformMvc.Navigation
{
    public class ViewNavigation
    {
        static readonly ViewNavigation _instance;

        public static ViewNavigation Instance
        {
            get { return _instance; }
        }

        private ViewNavigation() { }
    }

    public static class ViewNavigationExtensions
    {
        #region Go to another view

        public static IActionResult OpenAction(this ViewNavigation navigation, string controllerName, string actionName)
        {
            return new OpenActionResult(controllerName, actionName);
        }

        public static IActionResult OpenAction<TModel>(this ViewNavigation navigation, string controllerName, string actionName, TModel model)
        {
            return new OpenActionResult<TModel>(controllerName, actionName, model);
        }

        public static IActionResult Open(this ViewNavigation navigation, string controllerName)
        {
            return new OpenResult(controllerName);
        }

        public static IActionResult Open<TModel>(this ViewNavigation navigation, string controllerName, TModel model)
        {
            return new OpenResult<TModel>(controllerName, model);
        }

        public static IActionResult RedirectToAction(this ViewNavigation navigation, string controllerName, string actionName)
        {
            return new RedirectToActionResult(controllerName, actionName);
        }

        public static IActionResult RedirectToAction<TModel>(this ViewNavigation navigation, string controllerName, string actionName, TModel model)
        {
            return new RedirectToActionResult<TModel>(controllerName, actionName, model);
        }

        public static IActionResult RedirectTo(this ViewNavigation navigation, string controllerName)
        {
            return new RedirectToResult(controllerName);
        }

        public static IActionResult RedirectTo<TModel>(this ViewNavigation navigation, string controllerName, TModel model)
        {
            return new RedirectToResult<TModel>(controllerName, model);
        } 

        #endregion

        public static IActionResult DisplayView(this ViewNavigation navigation)
        {
            return DisplayViewResult.Instance;
        }

        public static IActionResult DisplayView<TModel>(this ViewNavigation navigation, TModel model)
        {
            return new DisplayViewResult<TModel>(model);
        }

        public static IActionResult CloseView(this ViewNavigation navigation)
        {
            return CloseViewResult.Instance;
        }

        public static IActionResult HideView(this ViewNavigation navigation)
        {
            return HideViewResult.Instance;
        }
    }
}