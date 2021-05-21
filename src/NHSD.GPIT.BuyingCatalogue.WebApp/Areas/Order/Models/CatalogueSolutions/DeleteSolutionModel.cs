namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class DeleteSolutionModel : OrderingBaseModel
    {
        public DeleteSolutionModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01/catalogue-solutions/10000-002"; // TODO
            BackLinkText = "Go back";
            Title = "Delete Anywhere Consult from C010001-01?"; // TODO
        }
    }
}
