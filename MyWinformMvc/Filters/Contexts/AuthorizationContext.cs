
namespace My.WinformMvc.Filters.Contexts
{
	/// <summary>
	/// The context of an Authorization filter
	/// </summary>
	public class AuthorizationContext : FilterContext
	{
		bool _isAuthenticated = false;

		public AuthorizationContext()
		{
            IsAuthorized = false;
		}

		public bool IsAuthenticated
		{
			get { return _isAuthenticated; }
			set 
            {
                _isAuthenticated = value;
                IsAuthorized = value;
            }
		}

        public bool IsAuthorized { get; set; }
	}
}