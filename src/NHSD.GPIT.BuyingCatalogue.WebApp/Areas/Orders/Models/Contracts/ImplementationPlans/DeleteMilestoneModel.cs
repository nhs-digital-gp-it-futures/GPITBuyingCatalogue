using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans
{
    public class DeleteMilestoneModel : NavBaseModel
    {
        public DeleteMilestoneModel()
        {
        }

        public DeleteMilestoneModel(CallOffId callOffId, string internalOrgId, ImplementationPlanMilestone milestone)
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            MilestoneId = milestone.Id;
            MilestoneName = milestone.Title;
        }

        public int MilestoneId { get; set; }

        public string MilestoneName { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
