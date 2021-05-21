namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class SelectOrganisationModel : OrderingBaseModel
    {
        public SelectOrganisationModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F"; // TODO
            Title = "Which organisation are you looking for?";
        }
    }
}
