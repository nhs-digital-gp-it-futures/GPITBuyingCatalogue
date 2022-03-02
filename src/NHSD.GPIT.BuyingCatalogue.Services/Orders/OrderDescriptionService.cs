using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderDescriptionService : IOrderDescriptionService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderDescriptionService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetOrderDescription(CallOffId callOffId, string internalOrgId, string description)
        {
            description.ValidateNotNullOrWhiteSpace(nameof(description));

            var order = await dbContext.Orders.SingleAsync(o => o.Id == callOffId.Id && o.OrderingParty.InternalIdentifier == internalOrgId);
            order.Description = description;
            await dbContext.SaveChangesAsync();
        }
    }
}
