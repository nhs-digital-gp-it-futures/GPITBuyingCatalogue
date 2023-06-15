using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans
{
    public class DeleteMilestoneModel : NavBaseModel
    {
        public DeleteMilestoneModel()
        {
        }

        public DeleteMilestoneModel(CallOffId callOffId, string internalOrgId, int milestoneId)
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            MilestoneId = milestoneId;
        }

        public int MilestoneId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
