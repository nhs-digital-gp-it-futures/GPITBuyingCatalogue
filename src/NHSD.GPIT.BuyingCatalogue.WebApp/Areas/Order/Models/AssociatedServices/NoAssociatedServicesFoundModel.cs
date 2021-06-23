namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class NoAssociatedServicesFoundModel : OrderingBaseModel
    {
        public NoAssociatedServicesFoundModel(string odsCode, string callOffId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            BackLinkText = "Go back";
            Title = "No Associated Services found";
        }
    }
}
