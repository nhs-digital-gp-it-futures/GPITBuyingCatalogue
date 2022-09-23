﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                o => o.Id == callOffId.Id
                && o.OrderingParty.InternalIdentifier == internalOrgId);

            order.SelectedFrameworkId = frameworkId;

            await dbContext.SaveChangesAsync();
        }

        private async Task<IList<EntityFramework.Catalogue.Models.Framework>> FindFrameworksFromAssociatedOnlySolution(CallOffId callOffId, string internalOrgId) =>
            await dbContext.Orders
                .Where(o => o.Id == callOffId.Id
                       && o.OrderingParty.InternalIdentifier == internalOrgId)
            .SelectMany(o => o.Solution.Solution.FrameworkSolutions.Select(fs => fs.Framework))
            .OrderBy(f => f.Name)
            .ToListAsync();

        private async Task<IList<EntityFramework.Catalogue.Models.Framework>> FindFrameworksFromCatalgoueSolution(CallOffId callOffId, string internalOrgId) =>
            await dbContext.OrderItems
                .Where(oi => oi.OrderId == callOffId.Id
                       && oi.Order.OrderingParty.InternalIdentifier == internalOrgId
                       && oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework))
            .OrderBy(f => f.Name)
            .ToListAsync();
    }
}
