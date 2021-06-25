using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class NoAssociatedServicesFoundModel : OrderingBaseModel
    {
        public NoAssociatedServicesFoundModel(string odsCode, CallOffId callOffId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            BackLinkText = "Go back";
            Title = "No Associated Services found";
        }
    }
}
