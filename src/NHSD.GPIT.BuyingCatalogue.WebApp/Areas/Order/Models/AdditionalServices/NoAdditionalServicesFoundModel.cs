using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class NoAdditionalServicesFoundModel : OrderingBaseModel
    {
        public NoAdditionalServicesFoundModel(string odsCode, CallOffId callOffId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            Title = "No Additional Services found";
        }
    }
}
