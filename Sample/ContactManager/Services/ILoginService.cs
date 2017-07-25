using ContactManager.Domain;
using ContactManager.Repository;

namespace ContactManager.Services
{
    public interface ILoginService
    {
        Login GetLogin(string loginName, string password);
    }

    public class LoginService : ILoginService
    {
        readonly Database _database;

        public LoginService(Database database)
        {
            _database = database;
        }

        #region ILoginService Members

        public Login GetLogin(string loginName, string password)
        {
            return _database.GetLogin(loginName, password);
        }

        #endregion
    }
}
