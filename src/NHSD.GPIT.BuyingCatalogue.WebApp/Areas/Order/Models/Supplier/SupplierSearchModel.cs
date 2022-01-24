using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class SupplierSearchModel : OrderingBaseModel
    {
        public SupplierSearchModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            Title = "Find supplier information";
            CallOffId = order.CallOffId;
            OdsCode = odsCode;
        }

        public SupplierSearchModel()
        {
        }

        public CallOffId CallOffId { get; set; }

        public string SearchString { get; set; }
    }
}
