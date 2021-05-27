using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts
{
    public interface IContactDetailsService
    {
        Address AddOrUpdateAddress(Address existingAddress, Address newOrUpdatedAddress);

        Contact AddOrUpdatePrimaryContact(Contact existingContact, PrimaryContactModel newOrUpdatedContact);
    }
}
