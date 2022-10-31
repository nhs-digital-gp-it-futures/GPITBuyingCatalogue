using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public sealed class AssociatedServicesBillingService : IAssociatedServicesBillingService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public AssociatedServicesBillingService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<OrderItem>> GetAssociatedServiceOrderItems(string internalOrgId, CallOffId callOffId)
        {
            var orderId = dbContext.Orders
                .First(x => x.OrderNumber == callOffId.OrderNumber
                    && x.Revision == callOffId.Revision
                    && x.OrderingParty.InternalIdentifier == internalOrgId).Id;

            return await dbContext.OrderItems
                .Include(oi => oi.OrderItemRecipients)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice)
                .Where(oi => oi.OrderId == orderId
                    && oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
                .ToListAsync();
        }
    }
}
