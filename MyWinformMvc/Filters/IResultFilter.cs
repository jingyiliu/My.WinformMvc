
using My.WinformMvc.Filters.Contexts;

namespace My.WinformMvc.Filters
{
	/// <summary>
	/// Result filter interface
	/// </summary>
	public interface IResultFilter
	{
		#region Methods

		/// <summary>
		/// The after method. It is executed after the execution of an action result
		/// </summary>
		/// <param name="filterContext"></param>
		void OnResultExecuted(ResultExecutedContext filterContext);

		/// <summary>
		/// The before method. It is executed before the execution of an action result
		/// </summary>
		/// <param name="filterContext">The context of execution</param>
		void OnResultExecuting(ResultExecutingContext filterContext);

		#endregion Methods
	}
}