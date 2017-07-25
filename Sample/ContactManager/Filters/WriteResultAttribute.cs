using ContactManager.Utils;
using My.WinformMvc.Filters;
using My.WinformMvc.Filters.Contexts;

namespace ContactManager.Filters
{
    public class WriteResultAttribute : ResultFilterAttribute
    {
        /// <summary>
        /// The after method. It is executed after the execution of an action result
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Logger.Log("WriteResultAttribute.OnResultExecuted");
        }

        /// <summary>
        /// The before method. It is executed before the execution of an action result
        /// </summary>
        /// <param name="filterContext">The context of execution</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Logger.Log("WriteResultAttribute.OnResultExecuting");
        }
    }
}
