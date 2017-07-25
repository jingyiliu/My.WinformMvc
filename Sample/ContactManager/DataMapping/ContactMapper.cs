using System.Collections.Generic;
using ContactManager.Domain;
using ContactManager.Views.Model;

namespace ContactManager.DataMapping
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ContactMapper
    {
        public static Contact ToEntity(this ContactModel model)
        {
            return new Contact
            {
                Id = model.Id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };
        }

        public static ContactModel ToModel(this Contact entity)
        {
            return new ContactModel
            {
                Id = entity.Id,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber
            };
        }

        public static List<ContactModel> ToModel(this List<Contact> entities)
        {
            var models = new List<ContactModel>();
            foreach (var entity in entities)
                models.Add(entity.ToModel());
            return models;
        }
    }
}
