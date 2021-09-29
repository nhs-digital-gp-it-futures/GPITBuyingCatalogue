using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class EditSupplierModel : NavBaseModel
    {
        public EditSupplierModel()
        {
        }

        public EditSupplierModel(Supplier supplier)
        {
            DetailsStatus =
                !string.IsNullOrWhiteSpace(supplier.Name)
                && !string.IsNullOrWhiteSpace(supplier.LegalName);

            AddressStatus = supplier.Address is not null;

            ContactsStatus = supplier.SupplierContacts.Any();

            SupplierId = supplier.Id;

            SupplierStatus = supplier.IsActive;

            SupplierName = supplier.Name;
        }

        public static IEnumerable<object> EditSupplierRadioOptions =>
            new List<object> { new { Display = "Active", Value = true }, new { Display = "Inactive", Value = false } };

        public bool SupplierStatus { get; init; }

        public bool DetailsStatus { get; }

        public bool AddressStatus { get; }

        public bool ContactsStatus { get; }

        public int SupplierId { get; }

        public string SupplierName { get; }
    }
}
