using System;
using System.Threading.Tasks;
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

            var order = await dbContext.Order(internalOrgId, callOffId);

            order.Description = description;

            await dbContext.SaveChangesAsync();
        }
    }
}
