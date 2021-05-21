namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionModel : OrderingBaseModel
    {
        public SelectSolutionModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01/catalogue-solutions"; // TODO
            BackLinkText = "Go back";
            Title = "Add Catalogue Solution for C010001-01"; // TODO
        }
    }
}
