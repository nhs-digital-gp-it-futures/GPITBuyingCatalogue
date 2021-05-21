namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class SummaryModel : OrderingBaseModel
    {
        public SummaryModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F"; // TODO
            Title = "Order summary for C010000-01"; // TODO
        }
    }
}
