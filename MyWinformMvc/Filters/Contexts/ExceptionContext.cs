using System;
using My.WinformMvc.Properties;

namespace My.WinformMvc.Filters.Contexts
{
	/// <summary>
	/// The context of an Exception attribute
	/// </summary>
	public class ExceptionContext : FilterContext
	{
		#region Constructors

        public ExceptionContext(Exception exception)
		{
			if (exception == null) throw new ArgumentNullException(Resources.ArgumentNullException);
			Exception = exception;
		}

		#endregion Constructors

		#region Public Properties

        /// <summary>
        /// if ExceptionHandled is set to true then contains the Exception
        /// </summary>
        /// <value>The exception.</value>
        public virtual Exception Exception { get; set; }

		/// <summary>
		/// Indicate whether the caught exception is handled or not
		/// </summary>
        public virtual bool ExceptionHandled { get; set; }

		#endregion Public Properties
	}
}