using System;
using System.Linq;
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
                .AsNoTracking()
                .Include(x => x.ImplementationPlan)
                .ThenInclude(x => x.Milestones.OrderBy(m => m.Order))
                .Include(x => x.ContractBilling)
                    .ThenInclude(x => x.ContractBillingItems)
                        .ThenInclude(x => x.Milestone)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (output != null)
            {
                return output;
            }

            output = new Contract
            {
                OrderId = orderId,
            };

            dbContext.Contracts.Add(output);

            await dbContext.SaveChangesAsync();

            return output;
        }

        public async Task<ContractFlags> GetContractFlags(int orderId)
        {
            var output = await dbContext.ContractFlags.FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (output != null)
            {
                return output;
            }

            output = new ContractFlags
            {
                OrderId = orderId,
            };

            dbContext.ContractFlags.Add(output);

            await dbContext.SaveChangesAsync();

            return output;
        }

        public async Task RemoveContractFlags(int orderId)
        {
            var contract = await GetContractFlags(orderId);

            contract.UseDefaultImplementationPlan = null;
            contract.UseDefaultBilling = null;
            contract.HasSpecificRequirements = null;
            contract.UseDefaultDataProcessing = null;

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveBillingAndRequirements(int orderId)
        {
            var contract = await GetContractFlags(orderId);

            contract.UseDefaultBilling = null;
            contract.HasSpecificRequirements = null;

            await dbContext.SaveChangesAsync();
        }

        public async Task UseDefaultDataProcessing(int orderId, bool value)
        {
            var flags = await dbContext.ContractFlags.FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (flags != null)
            {
                flags.UseDefaultDataProcessing = value;

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
