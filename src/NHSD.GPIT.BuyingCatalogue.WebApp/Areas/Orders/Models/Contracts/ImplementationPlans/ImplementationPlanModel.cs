using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans
{
    public class ImplementationPlanModel : NavBaseModel
    {
        public ImplementationPlanModel(ImplementationPlan defaultPlan, ImplementationPlan bespokePlan, Solution solution)
        {
            DefaultPlan = defaultPlan ?? throw new ArgumentNullException(nameof(defaultPlan));
            BespokePlan = bespokePlan;
            Solution = solution;
        }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public Solution Solution { get; set; }

        public ImplementationPlan DefaultPlan { get; set; }

        public ImplementationPlan BespokePlan { get; set; }

        public bool HasBespokeMilestones => BespokePlan != null && BespokePlan.Milestones.Any();

        public string SupplierImplementationPlan => Solution?.ImplementationDetail
            ?? "The supplier has not provided a standard implementation plan. You should contact them to discuss this.";

        public string DefaultMilestoneLabelText => HasBespokeMilestones
            ? "Milestones and payment triggers"
            : "Default milestones and payment triggers";
    }
}
