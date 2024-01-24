using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier
{
    public class NoAvailableSuppliersModel : OrderingBaseModel
    {
        public NoAvailableSuppliersModel(string internalOrgId, CallOffId callOffId, OrderType orderType)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            OrderTypeText = orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceMerger => "Merger",
                OrderTypeEnum.AssociatedServiceSplit => "Split",
                _ => "Unknown",
            };
        }

        public CallOffId CallOffId { get; set; }

        public string OrderTypeText { get; set; }
    }
}
