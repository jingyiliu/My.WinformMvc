using System.Collections.Generic;
using ContactManager.Domain;

namespace ContactManager.Repository
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Database
    {
        readonly List<Login> _logins; 
        readonly List<Contact> _contacts;

        public Database()
        {
            _contacts = GenerateContacts();
            _logins = GenerateLogins();
        }

        public void AddContact(Contact contact)
        {
            //Inserting the contact into the list
            _contacts.Add(contact);
        }

        public void UpdateContact(Contact contact)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                if (_contacts[i].Id != contact.Id)
                    continue;
                _contacts[i] = contact;
                return;
            }
        }

        public void DeleteContact(long id)
        {
            for (int i = 0; i < _contacts.Count; i++)
            {
                if (_contacts[i].Id != id)
                    continue;
                _contacts.RemoveAt(i);
                return;
            }
        }

        public List<Contact> GetContacts()
        {
            return _contacts;
        }

        #region Generating Data

        List<Contact> GenerateContacts()
        {
            var contacts = new List<Contact>();

            contacts.Add(CreateContact(1, "Mary", "Liu", "+23799657392", "Johnny.Liu@gmail.com"));
            contacts.Add(CreateContact(2, "Tom", "Zhou", "+23799727707", "djomgat@gmail.com"));
            contacts.Add(CreateContact(3, "BAKE", "Mu", "1111111", "bbakeneghe@yahoo.fr"));
            contacts.Add(CreateContact(4, "MAYENGUE", "An", "+23799138606", "mayengue2008@yahoo.fr"));

            return contacts;
        }

        Contact CreateContact(long id, string firstName, string lastName, string phoneNumber, string email)
        {
            return new Contact { Id = id, FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber, Email = email };
        }

        #endregion

        public Login GetLogin(string loginName, string password)
        {
            foreach (var login in _logins)
            {
                if (login.LoginName == loginName && login.Password == password)
                    return login;
            }
            return null;
        }

        List<Login> GenerateLogins()
        {
            var logins = new List<Login>
            {
                new Login { Id = 1, LoginName = "Johnny.Liu", Password = "admin", Role = "ADMINISTRATOR", Version = 0 },
                new Login { Id = 2, LoginName = "admin", Password = "admin", Role = "COORDINATOR", Version = 0 },
                new Login { Id = 3, LoginName = "user", Password = "admin", Role = "USER", Version = 0 }
            };
            return logins;
        }
    }
}
