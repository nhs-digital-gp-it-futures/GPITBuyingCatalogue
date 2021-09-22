using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SupplierContactViewModel
    {
        public SupplierContactViewModel(MarketingContact contact)
        {
            FullName = $"{contact.FirstName} {contact.LastName}";
            PhoneNumber = contact.PhoneNumber;
            Department = contact.Department;
            Email = contact.Email;
        }

        public string FullName { get; }

        public string PhoneNumber { get; }

        public string Department { get; }

        public string Email { get; }
    }
}
