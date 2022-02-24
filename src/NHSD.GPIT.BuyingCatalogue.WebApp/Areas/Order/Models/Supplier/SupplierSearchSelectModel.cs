using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class SupplierSearchSelectModel : OrderingBaseModel
    {
        public SupplierSearchSelectModel(string internalOrgId, CallOffId callOffId, List<EntityFramework.Catalogue.Models.Supplier> suppliers, int? selectedSupplierId = null)
        {
            Title = "Suppliers found";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            Suppliers = suppliers;
            SelectedSupplierId = selectedSupplierId;
        }

        public SupplierSearchSelectModel()
        {
        }

        public CallOffId CallOffId { get; set; }

        public List<EntityFramework.Catalogue.Models.Supplier> Suppliers { get; set; }

        public int? SelectedSupplierId { get; set; }
    }
}
