namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class OrganisationModel : OrderingBaseModel
    {
        public OrganisationModel()
        {
            BackLinkText = "Go back to homepage";
            BackLink = "/";
            Title = "NHS Hull CCG"; // TODO
        }
    }
}
