
using System.Windows.Forms;
using ContactManager.Utils;
using My.WinformMvc.Filters;
using My.WinformMvc.Filters.Contexts;

namespace ContactManager.Filters
{
    public class ConfirmDeleteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext preContext)
        {
            Logger.Log("ConfirmDeleteAttribute.OnActionExecuting");

            DialogResult dr = MessageBox.Show("Do you really want to delete the contact?", "Information",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (dr != DialogResult.Yes) // If the user choose 'No', return to the list view
                preContext.Result = preContext.Controller.DisplayView();
        }

        public override void OnActionExecuted(ActionExecutedContext postContext)
        {
            Logger.Log("ConfirmDeleteAttribute.OnActionExecuted");
        }
    }
}
