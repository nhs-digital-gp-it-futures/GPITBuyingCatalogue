namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class OrderModel : OrderingBaseModel
    {
        public OrderModel()
        {
            BackLinkText = "Go back to all orders";
            BackLink = "/order/organisation/03F"; // TODO
            Title = "Order C010001-01"; // TODO
        }
    }
}
