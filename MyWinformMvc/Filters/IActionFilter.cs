using My.WinformMvc.Filters.Contexts;

namespace My.WinformMvc.Filters
{
    /// <summary>
    /// Action Filter interface
    /// </summary>
    public interface IActionFilter
    {
        /// <summary>
        /// The before method. This method is executed before the execution of an action
        /// </summary>
        /// <param name="preContext">The context of execution</param>
        void OnActionExecuting(ActionExecutingContext preContext);

        /// <summary>
        /// The after method. This method is executed after the execution of an action
        /// </summary>
        /// <param name="postContext">The context of execution</param>
        void OnActionExecuted(ActionExecutedContext postContext);
    }
}
