using ContactManager.DataMapping;
using ContactManager.Filters;
using ContactManager.Services;
using ContactManager.Utils;
using ContactManager.Views.Model;
using ContactManager.Views.Utils;
using My.WinformMvc;

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
