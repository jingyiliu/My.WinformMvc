
using System;

namespace My.WinformMvc.Navigation
{
	class OpenResult : IActionResult
	{
        protected readonly string _targetControllerName;

        internal OpenResult(string targetControllerName)
		{
			_targetControllerName = targetControllerName;
		}

        public virtual void ExecuteResult(IController controller)
        {
            controller.Coordinator.InvokeControllerAction(controller, _targetControllerName, ActionNames.DisplayView, null);
		}
	}

    class OpenResult<TModel> : OpenResult
    {
        readonly TModel _model;

        internal OpenResult(string targetControllerName, TModel model)
            : base(targetControllerName)
        {
            _model = model;
        }

        public override void ExecuteResult(IController controller)
        {
            controller.Coordinator.InvokeControllerAction(controller, _targetControllerName, ActionNames.DisplayView, new Object[] { _model });
        }
    }
}