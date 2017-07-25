
using System;

namespace My.WinformMvc.Navigation
{
	class OpenActionResult : IActionResult
	{
		protected readonly string _targetActionName;
        protected readonly string _targetControllerName;

        internal OpenActionResult(string targetControllerName, string targetActionName)
		{
			_targetControllerName = targetControllerName;
			_targetActionName = targetActionName;
		}

        public virtual void ExecuteResult(IController controller)
		{
            controller.Coordinator.InvokeControllerAction(controller, _targetControllerName, _targetActionName, null);
		}
	}

    class OpenActionResult<TModel> : OpenActionResult
    {
        readonly TModel _model;

        internal OpenActionResult(string targetControllerName, string targetActionName, TModel model)
            : base(targetControllerName, targetActionName)
        {
            _model = model;
        }

        public override void ExecuteResult(IController controller)
        {
            controller.Coordinator.InvokeControllerAction(controller, _targetControllerName, _targetActionName, new Object[] { _model });
        }
    }
}