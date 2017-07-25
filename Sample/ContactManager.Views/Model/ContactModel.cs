using My.WinformMvc.DataBinding;

namespace ContactManager.Views.Model
{
    public class ContactModel
    {
        public long Id { get; set; }
        [DataBinding]
        public string FirstName { get; set; }
        [DataBinding]
        public string LastName { get; set; }
        [DataBinding]
        public string Email { get; set; }
        [DataBinding]
        public string PhoneNumber { get; set; }

        // Extended property
        public bool IsEdit { get; set; }
    }
}
