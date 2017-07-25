using System;
using System.Drawing;
using System.Windows.Forms;
using ContactManager.Views.Model;
using ContactManager.Views.Utils;
using My.WinformMvc;
using My.WinformMvc.Validation;

namespace ContactManager.Views
{
    [MvcView("Login")]
    public partial class LoginView : BaseView, IView<LoginModel>
    {
        LoginModel _model;

        public LoginView()
        {
            InitializeComponent();
        }

        public void BindModel(LoginModel model)
        {
            _model = model;
            BindDataSource(model, null);
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            InvokeAction("Login", _model);
        }

        protected override void DoShowModelError(ModelState state)
        {
            ModelErrorCollection error;
            if (state.TryGetValue(Constant.ModelErrorKey, out error))
            {
                var errorMsg = string.Empty;
                foreach (var errorItem in error)
                {
                    errorMsg 
                        += (errorItem.ErrorMessage ?? string.Empty) 
                        +  (errorItem.Exception == null ? string.Empty : errorItem.Exception.Message);
                }

                lblMessageVisibility.Visible = true;
                lblMessageVisibility.ForeColor = Color.IndianRed;
                lblMessageVisibility.Text = errorMsg;
            }
        }

        private void lbExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}