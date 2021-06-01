using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using BC = NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class ContactDetailsService : IContactDetailsService
    {
        public Address AddOrUpdateAddress(Address existingAddress, ServiceContracts.Organisations.Address newOrUpdatedAddress)
        {
            if (existingAddress is null)
                existingAddress = new Address();

            if (newOrUpdatedAddress is null)
                return existingAddress;

            existingAddress.Line1 = newOrUpdatedAddress.Line1;
            existingAddress.Line2 = newOrUpdatedAddress.Line2;
            existingAddress.Line3 = newOrUpdatedAddress.Line3;
            existingAddress.Line4 = newOrUpdatedAddress.Line4;
            existingAddress.Line5 = newOrUpdatedAddress.Line5;
            existingAddress.Town = newOrUpdatedAddress.Town;
            existingAddress.County = newOrUpdatedAddress.County;
            existingAddress.Postcode = newOrUpdatedAddress.Postcode;
            existingAddress.Country = newOrUpdatedAddress.Country;

            return existingAddress;
        }

        public Address AddOrUpdateAddress(Address existingAddress, BC.Address newOrUpdatedAddress)
        {
            if (existingAddress is null)
            {
                return new Address
                {
                    Line1 = newOrUpdatedAddress.Line1,
                    Line2 = newOrUpdatedAddress.Line2,
                    Line3 = newOrUpdatedAddress.Line3,
                    Line4 = newOrUpdatedAddress.Line4,
                    Line5 = newOrUpdatedAddress.Line5,
                    Town = newOrUpdatedAddress.Town,
                    County = newOrUpdatedAddress.County,
                    Postcode = newOrUpdatedAddress.Postcode,
                    Country = newOrUpdatedAddress.Country,
                };
            }

            if (newOrUpdatedAddress is null)
                return existingAddress;

            existingAddress.Line1 = newOrUpdatedAddress.Line1;
            existingAddress.Line2 = newOrUpdatedAddress.Line2;
            existingAddress.Line3 = newOrUpdatedAddress.Line3;
            existingAddress.Line4 = newOrUpdatedAddress.Line4;
            existingAddress.Line5 = newOrUpdatedAddress.Line5;
            existingAddress.Town = newOrUpdatedAddress.Town;
            existingAddress.County = newOrUpdatedAddress.County;
            existingAddress.Postcode = newOrUpdatedAddress.Postcode;
            existingAddress.Country = newOrUpdatedAddress.Country;

            return existingAddress;
        }

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

            return existingContact;
        }
    }
}
