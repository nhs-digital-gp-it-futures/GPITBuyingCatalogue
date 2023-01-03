using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public sealed class SummaryModel : OrderingBaseModel
    {
        public SummaryModel()
        {
        }

        public SummaryModel(string internalOrgId, Order order)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            Order = order;
        }

        public CallOffId CallOffId { get; set; }

        public Order Order { get; set; }

        public string AdviceText { get; set; }

        public bool CanBeAmended { get; set; }
    }
}
