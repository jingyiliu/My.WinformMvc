using System.Collections.Generic;
using ContactManager.Domain;

namespace ContactManager.Services
{
    public interface IContactService
    {
        void AddContact(Contact contactToCreate);
        void UpdateContact(Contact contactToUpdate);
        void DeleteContact(long id);
        List<Contact> GetContacts();
    }
}
