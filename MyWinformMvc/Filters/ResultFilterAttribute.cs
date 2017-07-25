using System;
using My.WinformMvc.Filters.Contexts;

namespace My.WinformMvc.Filters
{
    /// <summary>
    /// The result filter. This filter is executed before and after the execution of a action result
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public abstract class ResultFilterAttribute : Attribute, IResultFilter
	{
		#region Public Methods

        /// <summary>
        /// The after method. It is executed after the execution of an action result
        /// </summary>
        /// <param name="filterContext"></param>
		public virtual void OnResultExecuted(ResultExecutedContext filterContext)
		{
		}

        /// <summary>
        /// The before method. It is executed before the execution of an action result
        /// </summary>
        /// <param name="filterContext">The context of execution</param>
		public virtual void OnResultExecuting(ResultExecutingContext filterContext)
		{
		}

		#endregion Public Methods
	}
}