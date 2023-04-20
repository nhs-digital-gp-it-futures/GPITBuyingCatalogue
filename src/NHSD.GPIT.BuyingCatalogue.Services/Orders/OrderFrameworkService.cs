using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
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
                : await FindFrameworksFromCatalogueSolution(callOffId, internalOrgId);
        }

        public async Task SetSelectedFrameworkForOrder(
            CallOffId callOffId,
            string internalOrgId,
            string frameworkId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

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
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemFunding)
                .FirstOrDefaultAsync(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision == callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var selectedFramework = await dbContext.Frameworks.FirstAsync(f => f.Id == frameworkId);

            if (!order.IsLocalFundingOnly)
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
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision == callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .SelectMany(o => o.Solution.Solution.FrameworkSolutions.Select(fs => fs.Framework))
                .OrderBy(f => f.Name)
                .ToListAsync();

        private async Task<IList<EntityFramework.Catalogue.Models.Framework>> FindFrameworksFromCatalogueSolution(CallOffId callOffId, string internalOrgId)
        {
            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            return await dbContext.OrderItems
                .Where(oi => oi.OrderId == orderId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId
                    && oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework))
                .OrderBy(f => f.Name)
                .ToListAsync();
        }
    }
}
