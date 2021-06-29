using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class OrderModel : OrderingBaseModel
    {
        public OrderModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            BackLinkText = "Go back to all orders";
            BackLink = $"/order/organisation/{odsCode}";
            Title = $"Order {order.CallOffId}";
            OdsCode = odsCode;
            CallOffId = order.CallOffId;
            Description = order.Description;
        }

        public CallOffId CallOffId { get; set; }

        public string Description { get; set; }
    }
}
