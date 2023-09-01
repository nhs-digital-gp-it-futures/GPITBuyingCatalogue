using System;
using System.Collections.Generic;
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
            var contract = await dbContext.Contracts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            var output = await AddContract(contract, orderId);

            return output;
        }

        public async Task<Contract> GetContractWithImplementationPlan(int orderId)
        {
            var contract = await dbContext.Contracts
                .AsNoTracking()
                .Include(x => x.ImplementationPlan)
                    .ThenInclude(x => x.Milestones.OrderBy(m => m.Order))
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            var output = await AddContract(contract, orderId);

            return output;
        }

        public async Task<Contract> GetContractWithContractBilling(int orderId)
        {
            var contract = await dbContext.Contracts
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.ContractBilling)
                    .ThenInclude(x => x.ContractBillingItems)
                        .ThenInclude(x => x.Milestone)
                .Include(x => x.ContractBilling)
                    .ThenInclude(x => x.ContractBillingItems)
                        .ThenInclude(x => x.OrderItem)
                            .ThenInclude(x => x.CatalogueItem)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            var output = await AddContract(contract, orderId);

            return output;
        }

        public async Task<Contract> GetContractWithContractBillingRequirements(int orderId)
        {
            var contract = await dbContext.Contracts
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.ContractBilling)
                    .ThenInclude(x => x.Requirements)
                        .ThenInclude(x => x.OrderItem)
                            .ThenInclude(x => x.CatalogueItem)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            var output = await AddContract(contract, orderId);

            return output;
        }

        public async Task RemoveContract(int orderId)
        {
            var contract = await GetContractWithImplementationPlan(orderId);
            if (contract is not null)
            {
                if (contract.ImplementationPlan?.Milestones.Any() ?? false)
                    dbContext.ImplementationPlanMilestones.RemoveRange(contract.ImplementationPlan.Milestones);

                dbContext.Contracts.Remove(contract);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task ResetContract(int orderId)
        {
            await RemoveContract(orderId);
            await RemoveContractFlags(orderId);
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

        public async Task UseDefaultDataProcessing(int orderId, bool value)
        {
            var flags = await dbContext.ContractFlags.FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (flags != null)
            {
                flags.UseDefaultDataProcessing = value;

                await dbContext.SaveChangesAsync();
            }
        }

        private async Task<Contract> AddContract(Contract contract, int orderId)
        {
            if (contract is null)
            {
                contract = new Contract
                {
                    OrderId = orderId,
                };

                dbContext.Contracts.Add(contract);

                await dbContext.SaveChangesAsync();
            }

            return contract;
        }

        private async Task RemoveContractFlags(int orderId)
        {
            var flags = await GetContractFlags(orderId);
            flags.UseDefaultDataProcessing = false;

            await dbContext.SaveChangesAsync();
        }
    }
}
