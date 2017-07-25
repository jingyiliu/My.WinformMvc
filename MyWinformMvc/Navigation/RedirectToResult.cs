
using System;

namespace My.WinformMvc.Navigation
{
	class RedirectToResult : IActionResult
	{
        protected readonly string _targetControllerName;

        internal RedirectToResult(string targetControllerName)
		{
			_targetControllerName = targetControllerName;
		}

        public virtual void ExecuteResult(IController controller)
        {
            controller.Coordinator.InvokeControllerAction(controller, _targetControllerName, ActionNames.DisplayView, null);
            CloseViewResult.Instance.ExecuteResult(controller);
		}
	}

    class RedirectToResult<TModel> : RedirectToResult
    {
        readonly TModel _model;

        internal RedirectToResult(string targetControllerName, TModel model)
            : base(targetControllerName)
        {
            _model = model;
        }

        public override void ExecuteResult(IController controller)
        {
            controller.Coordinator.InvokeControllerAction(controller, _targetControllerName, ActionNames.DisplayView, new Object[] { _model });
            CloseViewResult.Instance.ExecuteResult(controller);
        }
    }
}