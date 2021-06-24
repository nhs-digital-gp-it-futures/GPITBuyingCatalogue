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
            CallOffId = order.CallOffId.ToString();
            Description = order.Description;
        }

        // TODO: should be of type CallOffId
        public string CallOffId { get; set; }

        public string Description { get; set; }
    }
}
