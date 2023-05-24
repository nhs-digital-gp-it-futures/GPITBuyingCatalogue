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

            var acceptanceCriteria = new List<ImplementationPlanAcceptanceCriteria>
            {
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to your satisfaction that the implementation plan includes all deliverables and responsibilities of you, the Service Recipient and the supplier, with appropriate time allocated for these to be met." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to your satisfaction that the Catalogue Solution has been configured to meet the Service Recipient’s operational requirements." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to the Service Recipient’s satisfaction that they’ve met their obligations set out in the Training Standard." },
                new() { MilestoneId = milestone1.Id, Description = "Where the supplier is responsible for training, they evidence to the Service Recipient’s satisfaction that end users can use the Catalogue Solution to fulfil relevant business functions." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to the Service Recipient’s satisfaction that the Catalogue Solution can connect to and access national and other interfaces applicable to it." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to the Service Recipient’s satisfaction that any Associated Services ordered that are applicable to implementation have been effectively provided." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to the Service Recipient’s satisfaction that the requirements of the Data Migration Standard and Catalogue Solution Migration Process applicable to the supplier for go live have been met and that the relevant data has migrated to enable the Service Recipient to conduct their relevant business functions effectively." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to your and the Service Recipient’s satisfaction that they will meet their obligations set out in the Service Management Standard." },
                new() { MilestoneId = milestone1.Id, Description = "The supplier evidences to your satisfaction that they have appropriate invoicing arrangements in place." },
                new() { MilestoneId = milestone1.Id, Description = "Any commercial issues identified to date are visible to both you and the supplier and an agreement on how they are to be handled has been reached between both parties." },
                new() { MilestoneId = milestone1.Id, Description = "Your approval that all Milestone 1 activities have been successfully completed." },

                new() { MilestoneId = milestone2.Id, Description = "The Service Recipient confirms that the Catalogue Solution is functioning as specified by the supplier and end users can use it effectively." },
                new() { MilestoneId = milestone2.Id, Description = "The supplier evidences to your and the Service Recipient’s satisfaction that all of the requirements of the Data Migration Standard and Catalogue Solution Migration Process that are applicable have been met by the supplier, and that all the relevant data has migrated to the Catalogue Solution." },
                new() { MilestoneId = milestone2.Id, Description = "The supplier evidences to your and the Service Recipient’s satisfaction that they’re meeting their service management obligations set out in appendix 2 of the Service Management Standard. This must be reasonably demonstrated within 10 working days of achievement of Milestone 1." },
                new() { MilestoneId = milestone2.Id, Description = "In relation to Type 2 Catalogue Solutions (which do not need to comply with service levels specified by NHS Digital), the supplier evidences to your and the Service Recipient’s satisfaction that the Catalogue Solution is meeting the applicable service levels." },
                new() { MilestoneId = milestone2.Id, Description = "Any commercial issues identified to date are visible to both you and the supplier and an agreement on how they’re to be handled has been reached between both parties." },
                new() { MilestoneId = milestone2.Id, Description = "Your approval that all Milestone 1 and 2 activities have been successfully completed." },
            };

            context.ImplementationPlanAcceptanceCriteria.AddRange(acceptanceCriteria);
            await context.SaveChangesAsync();
        }
    }
}
