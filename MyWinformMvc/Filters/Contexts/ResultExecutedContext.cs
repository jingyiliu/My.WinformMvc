using System;
using My.WinformMvc.Navigation;
using My.WinformMvc.Properties;

namespace My.WinformMvc.Filters.Contexts
{
    /// <summary>
    /// The context of the post result execution attribute
    /// </summary>
	public class ResultExecutedContext : FilterContext
	{
		#region Constructors

        public ResultExecutedContext(IActionResult result, bool canceled, Exception exception)
		{
			if (result == null) throw new ArgumentNullException(Resources.ArgumentNullException);            

			Result = result;
			Canceled = canceled;
			Exception = exception;
		}

		#endregion Constructors

		#region Public Properties
        /// <summary>
        /// Indicated wheter the execution of an action is cancelled
        /// </summary>
        /// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
        public virtual bool Canceled { get; set; }
        /// <summary>
        /// The exception thrown by an action
        /// </summary>
        /// <value>The exception.</value>
        public virtual Exception Exception { get; set; }
        /// <summary>
        /// Indicates if an exception is handled by the filter
        /// </summary>
        /// <value><c>true</c> if the exception is handled; otherwise, <c>false</c>.</value>
        public bool ExceptionHandled { get; set; }

		#endregion Public Properties
	}
}