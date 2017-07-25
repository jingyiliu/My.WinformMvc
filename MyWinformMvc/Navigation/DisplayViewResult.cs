using My.WinformMvc.Extensions;

namespace My.WinformMvc.Navigation
{
    class DisplayViewResult : IActionResult
    {
        static readonly DisplayViewResult _instance = new DisplayViewResult();

        private DisplayViewResult() { }

        internal static DisplayViewResult Instance
        {
            get { return _instance; }
        }

        public void ExecuteResult(IController controller)
        {
            if (controller.View.Visible)
                controller.View.Activate();
            else
                controller.View.Show();
        }
    }

	class DisplayViewResult<TModel> : IActionResult
	{
        readonly TModel _model;

        internal DisplayViewResult(TModel model)
        {
            _model = model;
		}

        public void ExecuteResult(IController controller)
        {
            var realView = controller.View.Cast<IView<TModel>>();
            realView.BindModel(_model);
            if (realView.Visible)
                realView.Activate();
            else
                realView.Show();
		}
	}
}