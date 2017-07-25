
using System;
using My.WinformMvc.Filters.Contexts;

namespace My.WinformMvc.Filters
{
    /// <summary>
    /// The action filter, This filter is executed before and after the action
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ActionFilterAttribute : Attribute, IActionFilter
	{
		#region Public Methods

        /// <summary>
        /// The after method. This method is executed after the execution of an action
        /// </summary>
        /// <param name="postContext">The context of execution</param>
		public virtual void OnActionExecuted(ActionExecutedContext postContext)
		{
		}

        /// <summary>
        /// The before method. This method is executed before the execution of an action
        /// </summary>
        /// <param name="preContext">The context of execution</param>
		public virtual void OnActionExecuting(ActionExecutingContext preContext)
		{
		}

		#endregion Public Methods
	}
}