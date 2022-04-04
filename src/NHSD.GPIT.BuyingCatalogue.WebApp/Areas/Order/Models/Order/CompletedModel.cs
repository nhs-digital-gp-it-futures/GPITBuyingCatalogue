using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class CompletedModel : NavBaseModel
    {
        public CompletedModel(string internalOrgId, CallOffId callOffId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }
    }
}
