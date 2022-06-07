using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contacts
{
    public sealed class ContactDetailsService : IContactDetailsService
    {
        public Contact AddOrUpdatePrimaryContact(Contact existingContact, PrimaryContactModel newOrUpdatedContact)
        {
            if (existingContact is null)
                return newOrUpdatedContact.ToDomain();

            if (newOrUpdatedContact is null)
                return existingContact;

            existingContact.FirstName = newOrUpdatedContact.FirstName;
            existingContact.LastName = newOrUpdatedContact.LastName;
            existingContact.Email = newOrUpdatedContact.EmailAddress;
            existingContact.Phone = newOrUpdatedContact.TelephoneNumber;
            existingContact.Department = newOrUpdatedContact.Department;

            return existingContact;
        }
    }
}
