using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class NoSupplierFoundModel : OrderingBaseModel
    {
        public NoSupplierFoundModel(string odsCode, CallOffId callOffId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/supplier/search";
        }
    }
}
