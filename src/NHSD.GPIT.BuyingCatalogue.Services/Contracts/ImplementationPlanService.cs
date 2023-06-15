using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public class ImplementationPlanService : IImplementationPlanService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ImplementationPlanService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<ImplementationPlan> GetDefaultImplementationPlan()
        {
            return await dbContext.ImplementationPlans
                .AsNoTracking()
                .Include(x => x.Milestones.OrderBy(m => m.Order))
                .ThenInclude(x => x.AcceptanceCriteria)
                .FirstOrDefaultAsync(x => x.IsDefault == true);
        }

        public async Task AddBespokeMilestone(int orderId, int contractId, string name, string paymentTrigger)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var contract = await dbContext.Contracts.Include(x => x.ImplementationPlan)
                    .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId) ?? throw new ArgumentException("Invalid contract", nameof(contractId));

            if (contract.ImplementationPlan == null)
            {
                var implementationPlan = new ImplementationPlan() { IsDefault = false, };
                contract.ImplementationPlan = implementationPlan;
                await dbContext.SaveChangesAsync();
            }

            contract.ImplementationPlan.Milestones.Add(new ImplementationPlanMilestone() { Title = name, PaymentTrigger = paymentTrigger });
            await dbContext.SaveChangesAsync();
        }

        public async Task<ImplementationPlanMilestone> GetBespokeMilestone(int orderId, int milestoneId)
        {
            return await dbContext.ImplementationPlanMilestones
                .AsNoTracking()
                .Include(x => x.Plan)
                    .ThenInclude(x => x.Contract)
                .FirstOrDefaultAsync(x => x.Id == milestoneId && x.Plan.Contract.OrderId == orderId);
        }

        public async Task EditBespokeMilestone(int orderId, int milestoneId, string name, string paymentTrigger)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var milestone = await GetBespokeMilestone(orderId, milestoneId) ?? throw new ArgumentException("Invalid milestone", nameof(milestoneId));

            milestone.Title = name;
            milestone.PaymentTrigger = paymentTrigger;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteBespokeMilestone(int orderId, int milestoneId)
        {
            var milestone = await GetBespokeMilestone(orderId, milestoneId);

            if (milestone == null)
            {
                return;
            }

            dbContext.ImplementationPlanMilestones.Remove(milestone);
            await dbContext.SaveChangesAsync();
        }
    }
}
