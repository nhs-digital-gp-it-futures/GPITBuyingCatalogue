namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class OrderingPartyModel : OrderingBaseModel
    {
        public OrderingPartyModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010004-01"; // TOOD
            Title = "Call-off Ordering Party information for C010004-01";
        }
    }
}
