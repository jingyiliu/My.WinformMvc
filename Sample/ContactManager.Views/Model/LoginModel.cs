using System;
using My.WinformMvc.DataBinding;

namespace ContactManager.Views.Model
{
	public class LoginModel
	{
        [DataBinding]
        public string LoginName { get; set; }
        [DataBinding]
        public string Password { get; set; }
	}
}