
namespace My.WinformMvc.Action
{
    public interface IActionInvoker
    {
        void InvokeAction(BaseController context, object[] parameters);
    }
}