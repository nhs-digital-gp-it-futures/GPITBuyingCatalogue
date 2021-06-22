namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class NoAdditionalServicesFoundModel : OrderingBaseModel
    {
        public NoAdditionalServicesFoundModel(string odsCode, string callOffId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            BackLinkText = "Go back";
            Title = "No Additional Services found";
        }
    }
}
