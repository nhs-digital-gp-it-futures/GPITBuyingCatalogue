using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public class ImplementationPlanService : IImplementationPlanService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ImplementationPlanService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<ImplementationPlan> CreateImplementationPlan()
        {
            var plan = new ImplementationPlan
            {
                IsDefault = false,
            };

            dbContext.ImplementationPlans.Add(plan);

            await dbContext.SaveChangesAsync();

            return plan;
        }

        public async Task<ImplementationPlan> GetDefaultImplementationPlan()
        {
            return await dbContext.ImplementationPlans
                .Include(x => x.Milestones)
                .ThenInclude(x => x.AcceptanceCriteria)
                .FirstOrDefaultAsync(x => x.IsDefault == true);
        }
    }
}
