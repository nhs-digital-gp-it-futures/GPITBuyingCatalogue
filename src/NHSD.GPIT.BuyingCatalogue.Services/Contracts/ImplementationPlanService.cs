using System;
using System.Linq;
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

        public async Task<ImplementationPlan> GetDefaultImplementationPlan()
        {
            return await dbContext.ImplementationPlans
                .Include(x => x.Milestones.OrderBy(m => m.Order))
                .ThenInclude(x => x.AcceptanceCriteria)
                .OrderByDescending(x => x.LastUpdated)
                .FirstOrDefaultAsync(x => x.IsDefault == true);
        }
    }
}
