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

        public async Task AddRequirement(int orderId, int contractId, CatalogueItemId catalogueItemId, string details)
        {
            if (string.IsNullOrEmpty(details))
                throw new ArgumentNullException(nameof(details));

            var contract = await dbContext.Contracts
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .ThenInclude(x => x.CatalogueItem)
                .Include(x => x.ContractBilling)
                .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId) ?? throw new ArgumentException("Invalid contract", nameof(contractId));

            var associatedService = contract.Order.GetAssociatedService(catalogueItemId) ?? throw new ArgumentException("Invalid associated service", nameof(catalogueItemId));

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

            var order = await dbContext.Orders
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.CatalogueItem)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new ArgumentException("Invalid order", nameof(orderId));

            var associatedService = order.GetAssociatedService(catalogueItemId) ?? throw new ArgumentException("Invalid associated service", nameof(catalogueItemId));

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
            var requirements = dbContext.Requirements
                .Where(x => x.OrderId == orderId && catalogueItemIds.Contains(x.CatalogueItemId));

            if (requirements.Any())
            {
                foreach (var requirement in requirements)
                {
                    dbContext.Requirements.Remove(requirement);
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
