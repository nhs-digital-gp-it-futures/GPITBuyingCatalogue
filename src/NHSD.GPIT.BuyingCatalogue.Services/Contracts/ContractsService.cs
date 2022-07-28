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

        public async Task<ContractFlags> GetContract(int orderId)
        {
            var output = await dbContext.ContractFlags.SingleOrDefaultAsync(x => x.OrderId == orderId);

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

        public async Task HasSpecificRequirements(int orderId, bool value)
        {
            var flags = await dbContext.ContractFlags.SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (flags != null)
            {
                flags.HasSpecificRequirements = value;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UseDefaultBilling(int orderId, bool value)
        {
            var flags = await dbContext.ContractFlags.SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (flags != null)
            {
                flags.UseDefaultBilling = value;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UseDefaultDataProcessing(int orderId, bool value)
        {
            var flags = await dbContext.ContractFlags.SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (flags != null)
            {
                flags.UseDefaultDataProcessing = value;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UseDefaultImplementationPlan(int orderId, bool value)
        {
            var flags = await dbContext.ContractFlags.SingleOrDefaultAsync(x => x.OrderId == orderId);

            if (flags != null)
            {
                flags.UseDefaultImplementationPlan = value;

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
