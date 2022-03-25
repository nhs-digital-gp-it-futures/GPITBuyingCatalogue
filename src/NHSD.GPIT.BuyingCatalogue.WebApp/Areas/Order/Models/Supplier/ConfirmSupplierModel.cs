using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class ConfirmSupplierModel : OrderingBaseModel
    {
        public ConfirmSupplierModel()
        {
        }

        public ConfirmSupplierModel(string internalOrgId, CallOffId callOffId, EntityFramework.Catalogue.Models.Supplier supplier)
        {
            Title = "Supplier details";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            SupplierId = supplier.Id;
            Name = supplier.Name;
            LegalName = supplier.LegalName;
            Address = supplier.Address;
        }

        public CallOffId CallOffId { get; set; }

        public int SupplierId { get; set; }

        public string Name { get; set; }

        public string LegalName { get; set; }

        public Address Address { get; set; }
    }
}
