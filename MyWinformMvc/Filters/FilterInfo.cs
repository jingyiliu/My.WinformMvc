
namespace My.WinformMvc.Filters
{
	class FilterInfo
	{
        static readonly FilterInfo _instance = new FilterInfo();

        internal static FilterInfo Instance
	    {
	        get { return _instance; }
	    }

        public IActionFilter[] ActionFilters { get; internal set; }
        public IAuthorizationFilter[] AuthorizationFilters { get; internal set; }
        public IExceptionFilter[] ExceptionFilters { get; internal set; }
        public IResultFilter[] ResultFilters { get; internal set; }
	}
}