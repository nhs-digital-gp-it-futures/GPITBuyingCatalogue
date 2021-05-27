namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class OrderModel : OrderingBaseModel
    {
        public OrderModel(string odsCode, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back to all orders";
            BackLink = $"/order/organisation/{odsCode}";
            Title = $"Order {order.CallOffId}";
            OdsCode = odsCode;
            CallOffId = order.CallOffId.ToString();
        }

        public string CallOffId { get; set; }
    }
}
