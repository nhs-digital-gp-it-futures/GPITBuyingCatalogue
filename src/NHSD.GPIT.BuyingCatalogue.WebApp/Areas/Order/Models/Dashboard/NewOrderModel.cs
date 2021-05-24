namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class NewOrderModel : OrderingBaseModel
    {
        public NewOrderModel()
        {
            BackLinkText = "Go back to all orders";
            BackLink = "/order/organisation/03F"; // TODO
            Title = "New order";
        }
    }
}
