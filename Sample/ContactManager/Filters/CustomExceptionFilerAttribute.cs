using My.WinformMvc.Filters;
using My.WinformMvc.Filters.Contexts;

namespace ContactManager.Filters
{
    public class CustomExceptionFilerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            ////Using the controller to store an error message
            //Controller controller = filterContext.Controller as Controller;
            
            //Storing the message
            //controller.AddMessage(filterContext.Exception.Message, Koossery.MVCwin.Util.MessageType.ERROR_MESSAGE);
        }
    }
}
