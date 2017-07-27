# My.WinformMvc
My.WinformMvc brings the power of mvc pattern into winform development. It helps to separate code, minimize code coupling and improve testability.

### Usage

```cs
using System;
using System.Windows.Forms;
using ContactManager.Repository;
using ContactManager.Services;
using ContactManager.Views;
using ContactManager.DataMapping;
using ContactManager.Filters;
using ContactManager.Services;
using ContactManager.Utils;
using ContactManager.Views.Model;
using ContactManager.Views.Utils;
using My.IoC;
using My.WinformMvc;

namespace ContactManager
{
    static class Start
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                        
            try
            {
                var container = new ObjectContainer(false);
                container.Register(typeof (Database)).In(Lifetime.Container());
                container.Register(typeof(ILoginService), typeof(LoginService)).In(Lifetime.Container());
                container.Register(typeof(IContactService), typeof(ContactService)).In(Lifetime.Container());

                IIocWrapper iocWrapper = new IoCWrapper(container);
                IPairManager pairManager = new PairManager();
                pairManager.RegisterAssembly(typeof(LoginView).Assembly);
                pairManager.RegisterAssembly(typeof(Start).Assembly);

                ICoordinator coordinator = new Coordinator(pairManager, iocWrapper);
                coordinator.StartApplication("ListController");
            }
            catch (Exception e)
            {
                Console.WriteLine("...Error while initializing the application please contact thhe administrator...");
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}

namespace ContactManager.Controllers
{
    [MvcController(Constant.ListController)]
    public class ListController : BaseController
    {
        readonly IContactService _contactService;

        public ListController(IContactService contactService, IView view)
            : base(view)
        {
            _contactService = contactService;
        }

        public override IActionResult DisplayView()
        {
            var myContacts = _contactService.GetContacts().ToModel();
            return base.DisplayView(myContacts);
        }

        [RoleBasedAuthorization(Roles = Constant.AdminRoles)]
        [ConfirmDelete]
        [NotifyDelete]
        [WriteResult]
        public IActionResult Delete(ContactModel model)
        {
            Logger.Log("Before ListController.Delete");
            _contactService.DeleteContact(model.Id);
            Logger.Log("After ListController.Delete");
            return DisplayView();
        }
    }
}

namespace ContactManager.Views
{
    [MvcView("ListView")]
    public partial class ListView : BaseView, IView<List<ContactModel>>
    {
        public ListView()
        {
            InitializeComponent();
        }

        public void BindModel(List<ContactModel> model)
        {
            Logger.Log("ListView.BindModel");
            contactDataGridView.DataSource = model;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (contactDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row first!", "Information");
                return;
            }

            var contact = contactDataGridView.CurrentRow.DataBoundItem as ContactModel;
            InvokeAction("Delete", contact);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenView(Constant.EditController, new ContactModel{ IsEdit = false });
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (contactDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row first!", "Information");
                return;
            }
            var contact = contactDataGridView.CurrentRow.DataBoundItem as ContactModel;
            contact.IsEdit = true;
            OpenView(Constant.EditController, contact);
        }
    }
}
```

Screenshots:<br>

![Listview form](http://files.cnblogs.com/files/johnny-liu/winformmvc_listview.gif)
(Listview form)<br>

![Login form](http://files.cnblogs.com/files/johnny-liu/winformmvc_login.gif)
(Login form)
