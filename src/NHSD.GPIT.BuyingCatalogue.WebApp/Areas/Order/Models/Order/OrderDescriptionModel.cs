namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class OrderDescriptionModel : OrderingBaseModel
    {
        public OrderDescriptionModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010004-01"; // TOOD
            Title = "Order description";
        }
    }
}
