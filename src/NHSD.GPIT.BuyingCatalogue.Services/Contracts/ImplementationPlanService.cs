using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

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
                .FirstOrDefaultAsync(x => x.IsDefault == true);
        }

        public async Task<Contract> AddImplementationPlan(int orderId, int contractId)
        {
            var contract = await dbContext.Contracts.Include(x => x.ImplementationPlan)
                .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId);

            contract.ImplementationPlan ??= new ImplementationPlan() { IsDefault = false, };
            await dbContext.SaveChangesAsync();

            return contract;
        }

        public async Task AddBespokeMilestone(int orderId, int contractId, string name, string paymentTrigger)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var contract = await AddImplementationPlan(orderId, contractId);

            contract.ImplementationPlan.Milestones.Add(new ImplementationPlanMilestone() { Title = name, PaymentTrigger = paymentTrigger });
            await dbContext.SaveChangesAsync();
        }

        public async Task<ImplementationPlanMilestone> GetMilestone(int orderId, int milestoneId)
        {
            return await dbContext.ImplementationPlanMilestones
                .FirstOrDefaultAsync(x => x.Id == milestoneId && x.Plan.Contract.OrderId == orderId);
        }

        public async Task EditMilestone(int orderId, int milestoneId, string name, string paymentTrigger)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var milestone = await GetMilestone(orderId, milestoneId);

            if (milestone == null)
            {
                return;
            }

            milestone.Title = name;
            milestone.PaymentTrigger = paymentTrigger;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteMilestone(int orderId, int milestoneId)
        {
            var milestone = await GetMilestone(orderId, milestoneId);

            if (milestone == null)
            {
                return;
            }

            dbContext.ImplementationPlanMilestones.Remove(milestone);
            await dbContext.SaveChangesAsync();
        }
    }
}
