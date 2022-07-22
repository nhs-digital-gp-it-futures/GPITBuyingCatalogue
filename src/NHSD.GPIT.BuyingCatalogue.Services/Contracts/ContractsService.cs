using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public class ContractsService : IContractsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ContractsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Contract> GetContract(int orderId)
        {
            var output = await dbContext.Contracts
                .Include(x => x.BillingItems)
                .Include(x => x.ImplementationPlan).ThenInclude(x => x.Milestones).ThenInclude(x => x.AcceptanceCriteria)
                .Include(x => x.DataProcessingPlan).ThenInclude(x => x.Steps).ThenInclude(x => x.Category)
                .AsNoTracking()
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (output != null) return output;

            output = new Contract { OrderId = orderId };

            dbContext.Contracts.Add(output);

            await dbContext.SaveChangesAsync();

            return output;
        }

        public async Task SetImplementationPlanId(int orderId, int? implementationPlanId)
        {
            var contract = await dbContext.Contracts.SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (contract == null)
            {
                return;
            }

            contract.ImplementationPlanId = implementationPlanId;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetDataProcessingPlanId(int orderId, int? dataProcessingPlanId)
        {
            var contract = await dbContext.Contracts.SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (contract == null)
            {
                return;
            }

            contract.DataProcessingPlanId = dataProcessingPlanId;

            await dbContext.SaveChangesAsync();
        }
    }
}
