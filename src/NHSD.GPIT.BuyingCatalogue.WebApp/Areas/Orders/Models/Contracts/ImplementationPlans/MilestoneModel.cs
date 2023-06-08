using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans
{
    public class MilestoneModel : NavBaseModel
    {
        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public string Name { get; set; }

        public string PaymentTrigger { get; set; }
    }
}
