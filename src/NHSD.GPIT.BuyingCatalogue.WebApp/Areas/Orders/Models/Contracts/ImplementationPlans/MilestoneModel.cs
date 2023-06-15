using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans
{
    public class MilestoneModel : NavBaseModel
    {
        public MilestoneModel()
        {
        }

        public MilestoneModel(CallOffId callOffId, string internalOrgId)
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
        }

        public MilestoneModel(ImplementationPlanMilestone milestone, CallOffId callOffId, string internalOrgId)
            : this(callOffId, internalOrgId)
        {
            MilestoneId = milestone.Id;
            Name = milestone.Title;
            PaymentTrigger = milestone.PaymentTrigger;
        }

        public bool IsEdit => MilestoneId != 0;

        public int MilestoneId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string PaymentTrigger { get; set; }

        public override string Title => IsEdit ? "Edit a bespoke implementation milestone" : "Add a bespoke implementation milestone";
    }
}
