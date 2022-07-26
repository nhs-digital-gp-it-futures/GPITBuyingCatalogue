using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.AssociatedServicesBilling
{
    public sealed class BasicBillingModel : NavBaseModel
    {
        public BasicBillingModel()
        {
        }

        public BasicBillingModel(string internalOrgId, CallOffId callOffId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
        }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
