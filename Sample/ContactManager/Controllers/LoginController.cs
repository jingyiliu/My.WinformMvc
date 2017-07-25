using System;
using ContactManager.Services;
using ContactManager.Views.Model;
using ContactManager.Views.Utils;
using My.WinformMvc;
using My.WinformMvc.Validation;

namespace ContactManager.Controllers
{
	/// <summary>
	/// The login controller class
	/// </summary>
    [MvcController(Constant.LoginController)]
    public class LoginController : BaseController
	{
        readonly ILoginService _loginService;

        public LoginController(ILoginService loginService, IView view)
            : base(view)
        {
            _loginService = loginService;
        }

        public IActionResult Login(LoginModel model)
        {
            var login = _loginService.GetLogin(model.LoginName, model.Password);
            if (login != null)
            {
                Session[Constant.RoleSessionKey] = login;
                return RedirectTo(Constant.ListController);
            }
            else
            {
                var error = new ModelState();
                error.AddModelError(Constant.ModelErrorKey, "Invalid login, please input the right username and password...");
                View.ShowModelError(error);
                return DisplayView();
            }
        }
	}
}