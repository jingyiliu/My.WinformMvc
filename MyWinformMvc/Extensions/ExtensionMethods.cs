using System;
using System.Reflection;
using My.WinformMvc.Core;
using My.WinformMvc.Filters;

namespace My.WinformMvc.Extensions
{
	static class ExtensionMethods
	{
		/// <summary>
        /// Gets the list of custom attributes for a method
		/// </summary>
		/// <typeparam name="TFilter">The type of the custom attributes</typeparam>
        /// <param name="method">The method</param>
		/// <returns>The list of custom filters</returns>
        internal static TFilter[] GetCustomAttributes<TFilter>(this MethodInfo method)
		{
			var attribs = method.GetCustomAttributes(typeof(TFilter), false);
            if (attribs.Length == 0) return null;

            var objs = new TFilter[attribs.Length];
            for (int i = 0; i < attribs.Length; i++)
                objs[i] = (TFilter)attribs[i];

            return objs;
		}

		/// <summary>
		/// Retrives all filters of an method
		/// </summary>
		/// <param name="method">The method</param>
		/// <returns>The list of all filters</returns>
        internal static FilterInfo GetFilters(this MethodInfo method)
		{
		    var authorizationFilters = method.GetCustomAttributes<IAuthorizationFilter>();
            var actionFilters = method.GetCustomAttributes<IActionFilter>();
            var resultFilters = method.GetCustomAttributes<IResultFilter>();
            var exceptionFilters = method.GetCustomAttributes<IExceptionFilter>();

		    return (authorizationFilters != null || actionFilters != null || resultFilters != null || exceptionFilters != null)
		        ? new FilterInfo
		        {
                    AuthorizationFilters = authorizationFilters,
                    ActionFilters = actionFilters,
                    ResultFilters = resultFilters,
                    ExceptionFilters = exceptionFilters
		        }
                : FilterInfo.Instance;
		}

        internal static T Cast<T>(this IView view) where T : class, IView
	    {
            var realView = view as T;
            if (realView != null)
                return realView;
            Logger.Error("");
            throw new Exception("");
	    }
	}
}