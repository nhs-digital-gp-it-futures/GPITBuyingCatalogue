using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData
{
    internal class ContractSeedData : ISeedData
    {
        public static async Task Initialize(BuyingCatalogueDbContext context)
        {
            var plan = new ImplementationPlan
            {
                IsDefault = true,
            };

            context.ImplementationPlans.Add(plan);
            context.SaveChanges();

            var milestone1 = new ImplementationPlanMilestone
            {
                PlanId = plan.Id,
                Order = 1,
                Title = "Milestone 1 (Go Live)",
                PaymentTrigger = "No payment.",
            };

            var milestone2 = new ImplementationPlanMilestone
            {
                PlanId = plan.Id,
                Order = 2,
                Title = "Milestone 2 (Service Stability)",
                PaymentTrigger = "Charges commence on achievement of Milestone 1, but payments will not be made until Milestone 2 is achieved.",
            };

            context.ImplementationPlanMilestones.AddRange(milestone1, milestone2);
            await context.SaveChangesAsync();
        }
    }
}
