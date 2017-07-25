using System;
using My.WinformMvc.Navigation;
using My.WinformMvc.Properties;

namespace My.WinformMvc.Filters.Contexts
{
	public class ResultExecutingContext : FilterContext
	{
        public ResultExecutingContext(IActionResult actionResult, bool cancel)
            : this(actionResult)
        {
            Cancel = cancel;
        }

        public ResultExecutingContext(IActionResult actionResult)
		{
			if (actionResult == null) throw new ArgumentNullException(Resources.ArgumentNullException);
			Result = actionResult;
		}

        public bool Cancel { get; set; }
	}
}