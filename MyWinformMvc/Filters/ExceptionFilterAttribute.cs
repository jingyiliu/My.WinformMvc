using System;
using My.WinformMvc.Filters.Contexts;
using My.WinformMvc.Properties;

namespace My.WinformMvc.Filters
{
    /// <summary>
    /// The Exception filter. This filter is executed after any other filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ExceptionFilterAttribute : Attribute, IExceptionFilter
	{
        static readonly Type _baseExceptionType = typeof(Exception);
        Type _exceptionType;

        public bool ExceptionHandled { get; set; }

        /// <summary>
        /// The type of exception to handle
        /// </summary>
		public Type ExceptionType
		{
            get
            {
                return _exceptionType;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!_baseExceptionType.IsAssignableFrom(value))
                    throw new ArgumentException(String.Format(Resources.IncorrectExceptionTypeException, value.FullName));
                _exceptionType = value;
            }
		}

        /// <summary>
        /// The method is executed after all other filters. It handle exceptions
        /// </summary>
        /// <param name="filterContext">The context of execution</param>
		public virtual void OnException(ExceptionContext filterContext)
		{
            ////Avoid null filter context
            //if (filterContext == null)
            //{
            //    throw new ArgumentNullException();
            //}
            ////Retrieve the caught exception
            //Exception exception = filterContext.Exception;
            ////check the type
            //if (!ExceptionType.IsInstanceOfType(exception))
            //{
            //    return;
            //}

            //if (!string.IsNullOrEmpty(ControllerName) && !string.IsNullOrEmpty(ActionName))
            //    filterContext.Result = new RedirectToActionResult(ControllerName, ActionName);
            //else
            //    throw new Exception(Resources.ArgumentMissingException);
            //filterContext.ExceptionHandled = ExceptionHandled;      
		}
	}
}