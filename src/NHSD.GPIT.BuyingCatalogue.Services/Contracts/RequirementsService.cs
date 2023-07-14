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
    public sealed class RequirementsService : IRequirementsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public RequirementsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Contract> SetRequirementComplete(int orderId, int contractId)
        {
            var contract = await dbContext.Contracts
                .Include(x => x.ContractBilling)
                .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId);

            contract.ContractBilling ??= new ContractBilling();
            contract.ContractBilling.HasConfirmedRequirements = true;
            await dbContext.SaveChangesAsync();

            return contract;
        }

        public async Task AddRequirement(int orderId, int contractId, CatalogueItemId catalogueItemId, string details)
        {
            if (string.IsNullOrEmpty(details))
                throw new ArgumentNullException(nameof(details));

            var contract = await dbContext.Contracts
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .ThenInclude(x => x.CatalogueItem)
                .Include(x => x.ContractBilling)
                .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId);

            var associatedService = contract.Order.GetAssociatedService(catalogueItemId);

            contract.ContractBilling.Requirements.Add(new Requirement()
            {
                OrderItem = associatedService,
                Details = details,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task<Requirement> GetRequirement(int orderId, int requirementId)
        {
            return await dbContext.Requirements
                .Include(x => x.OrderItem)
                    .ThenInclude(x => x.CatalogueItem)
                .FirstOrDefaultAsync(x => x.Id == requirementId && x.OrderId == orderId);
        }

        public async Task EditRequirement(int orderId, int requirementId, CatalogueItemId catalogueItemId, string details)
        {
            if (string.IsNullOrEmpty(details))
                throw new ArgumentNullException(nameof(details));

            var requirement = await GetRequirement(orderId, requirementId);

            if (requirement == null)
            {
                return;
            }

            var associatedService = await dbContext.OrderItems
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.CatalogueItemId == catalogueItemId);

            requirement.OrderItem = associatedService;
            requirement.Details = details;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteRequirement(int orderId, int requirementId)
        {
            var requirement = await GetRequirement(orderId, requirementId);

            if (requirement == null)
            {
                return;
            }

            dbContext.Requirements.Remove(requirement);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteRequirements(int orderId, IEnumerable<CatalogueItemId> catalogueItemIds)
        {
            var requirements = await dbContext.Requirements
                .Where(x => x.OrderId == orderId && catalogueItemIds.Contains(x.CatalogueItemId))
                .ToListAsync();

            if (!requirements.Any())
            {
                return;
            }

            dbContext.Requirements.RemoveRange(requirements);

            await dbContext.SaveChangesAsync();
        }
    }
}
