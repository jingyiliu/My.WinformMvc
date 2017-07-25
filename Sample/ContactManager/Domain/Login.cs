
namespace ContactManager.Domain
{
    public class Login : BaseEntity
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
