using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans
{
    public class CustomImplementationPlanModel : NavBaseModel
    {
        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }
    }
}
