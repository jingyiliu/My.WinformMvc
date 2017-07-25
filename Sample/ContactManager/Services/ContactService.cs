using System.Collections.Generic;
using ContactManager.Domain;
using ContactManager.Repository;

namespace ContactManager.Services
{
    public class ContactService : IContactService
    {
        static long _id = long.MaxValue;
        readonly Database _database;

        public ContactService(Database database)
        {
            _database = database;
        }

        #region IContactManagerService Members

        public void AddContact(Contact contact)
        {
            //Initializing contactTosave
            contact.Id = --_id;
            _database.AddContact(contact);
        }

        public void UpdateContact(Contact contact)
        {
            _database.UpdateContact(contact);
        }

        public void DeleteContact(long id)
        {
            _database.DeleteContact(id);
        }

        public List<Contact> GetContacts()
        {
            return _database.GetContacts();
        }

        #endregion
    }
}
