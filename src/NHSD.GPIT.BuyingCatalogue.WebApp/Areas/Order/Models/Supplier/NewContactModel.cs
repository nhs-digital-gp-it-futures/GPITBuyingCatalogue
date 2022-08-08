using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class NewContactModel : ContactModel
    {
        public NewContactModel()
        {
        }

        public NewContactModel(CallOffId callOffId, int supplierId, string supplierName)
        {
            CallOffId = callOffId;
            SupplierId = supplierId;
            SupplierName = supplierName;
        }

        public CallOffId CallOffId { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string Title { get; set; }
    }
}
