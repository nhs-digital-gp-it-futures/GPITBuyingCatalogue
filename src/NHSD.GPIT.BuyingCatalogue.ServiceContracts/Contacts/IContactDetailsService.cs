using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using BC = NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts
{
    public interface IContactDetailsService
    {
        Address AddOrUpdateAddress(Address existingAddress, Organisations.Address newOrUpdatedAddress);

        Address AddOrUpdateAddress(Address existingAddress, BC.Address newOrUpdatedAddress);

        Contact AddOrUpdatePrimaryContact(Contact existingContact, PrimaryContactModel newOrUpdatedContact);
    }
}
