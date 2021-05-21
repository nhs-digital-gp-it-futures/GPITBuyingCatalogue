namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class FundingSourceModel : OrderingBaseModel
    {
        public FundingSourceModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010001-01"; // TODO
            Title = "Funding source for C010001-01"; // TODO
        }
    }
}
