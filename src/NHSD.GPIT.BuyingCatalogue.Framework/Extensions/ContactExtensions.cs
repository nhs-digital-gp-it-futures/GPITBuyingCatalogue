using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ContactExtensions
    {
        public static Contact ToDomain(this PrimaryContactModel model)
        {
            return model is null
                ? new Contact()
                : new Contact
                {
                    SupplierContactId = model.SupplierContactId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.EmailAddress,
                    Phone = model.TelephoneNumber,
                    Department = model.Department,
                };
        }
    }
}
