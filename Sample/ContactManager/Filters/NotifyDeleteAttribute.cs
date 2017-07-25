
using ContactManager.Utils;
using My.WinformMvc.Filters;
using My.WinformMvc.Filters.Contexts;

namespace ContactManager.Filters
{
    public class NotifyDeleteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext preContext)
        {
            Logger.Log("NotifyDeleteAttribute.OnActionExecuting");
        }

        public override void OnActionExecuted(ActionExecutedContext postContext)
        {
            Logger.Log("NotifyDeleteAttribute.OnActionExecuted");
        }
    }
}
