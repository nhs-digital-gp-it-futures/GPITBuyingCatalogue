using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class ConfirmSupplierModel : OrderingBaseModel
    {
        public ConfirmSupplierModel()
        {
        }

        public ConfirmSupplierModel(string odsCode, CallOffId callOffId, EntityFramework.Catalogue.Models.Supplier supplier, string search)
        {
            Title = "Supplier details";
            InternalOrgId = odsCode;
            CallOffId = callOffId;
            Search = search;
            SupplierId = supplier.Id;
            Name = supplier.Name;
            LegalName = supplier.LegalName;
            Address = supplier.Address;
        }

        public CallOffId CallOffId { get; set; }

        public int SupplierId { get; set; }

        public string Search { get; set; }

        public string Name { get; set; }

        public string LegalName { get; set; }

        public Address Address { get; set; }
    }
}
