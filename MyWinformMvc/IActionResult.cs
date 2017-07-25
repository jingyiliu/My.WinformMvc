
namespace My.WinformMvc
{
	/// <summary>
	/// Navigates to view after the business logic (action method) has been executed.
	/// </summary>
	public interface IActionResult
	{
        void ExecuteResult(IController controller);
	}
}