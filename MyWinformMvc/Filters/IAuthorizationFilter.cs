using My.WinformMvc.Filters.Contexts;

namespace My.WinformMvc.Filters
{
    /// <summary>
    /// Authorization filter interface
    /// </summary>
    public interface IAuthorizationFilter
    {
        /// <summary>
        /// The unique method that is executed before all other filters
        /// </summary>
        /// <param name="filterContext">The context of execution</param>
        void OnAuthorization(AuthorizationContext filterContext);
    }
}
