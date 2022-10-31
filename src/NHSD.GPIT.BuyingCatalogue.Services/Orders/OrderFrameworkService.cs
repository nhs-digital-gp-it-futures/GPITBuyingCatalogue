using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderFrameworkService : IOrderFrameworkService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderFrameworkService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IList<EntityFramework.Catalogue.Models.Framework>> GetFrameworksForOrder(CallOffId callOffId, string internalOrgId, bool isAssociatedServiceOrder)
        {
            return isAssociatedServiceOrder
                ? await FindFrameworksFromAssociatedOnlySolution(callOffId, internalOrgId)
                : await FindFrameworksFromCatalgoueSolution(callOffId, internalOrgId);
        }

        public async Task SetSelectedFrameworkForOrder(
            CallOffId callOffId,
            string internalOrgId,
            string frameworkId)
        {
            var order = await dbContext.Orders.SingleOrDefaultAsync(
                o => o.Id == callOffId.OrderNumber
                && o.OrderingParty.InternalIdentifier == internalOrgId);

            order.SelectedFrameworkId = frameworkId;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder(
            CallOffId callOffId,
            string internalOrgId,
            string frameworkId)
        {
            var order = await dbContext.Orders
                .Include(o => o.SelectedFramework)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemFunding)
                .SingleOrDefaultAsync(
                o => o.Id == callOffId.OrderNumber
                && o.OrderingParty.InternalIdentifier == internalOrgId);

            var selectedFramework = await dbContext.Frameworks.SingleAsync(f => f.Id == frameworkId);

            if (selectedFramework.LocalFundingOnly != order.SelectedFramework.LocalFundingOnly)
            {
                order.OrderItems.Where(oi => oi.OrderItemFunding != null).ForEach(oi =>
                {
                    if (oi.OrderItemFunding.OrderItemFundingType != OrderItemFundingType.NoFundingRequired)
                        oi.OrderItemFunding = null;
                });
            }

            order.SelectedFramework = selectedFramework;

            await dbContext.SaveChangesAsync();
        }

        private async Task<IList<EntityFramework.Catalogue.Models.Framework>> FindFrameworksFromAssociatedOnlySolution(CallOffId callOffId, string internalOrgId) =>
            await dbContext.Orders
                .Where(o => o.Id == callOffId.OrderNumber
                       && o.OrderingParty.InternalIdentifier == internalOrgId)
            .SelectMany(o => o.Solution.Solution.FrameworkSolutions.Select(fs => fs.Framework))
            .OrderBy(f => f.Name)
            .ToListAsync();

        private async Task<IList<EntityFramework.Catalogue.Models.Framework>> FindFrameworksFromCatalgoueSolution(CallOffId callOffId, string internalOrgId) =>
            await dbContext.OrderItems
                .Where(oi => oi.OrderId == callOffId.OrderNumber
                       && oi.Order.OrderingParty.InternalIdentifier == internalOrgId
                       && oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework))
            .OrderBy(f => f.Name)
            .ToListAsync();
    }
}
