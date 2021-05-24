namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionPriceModel : OrderingBaseModel
    {
        public SelectSolutionPriceModel()
        {
            BackLink = "/order/organisation/03F/order/C010002-01/catalogue-solutions/select/solution"; // TODO
            BackLinkText = "Go back";
            Title = "List price for Write on Time"; // TODO
        }
    }
}
