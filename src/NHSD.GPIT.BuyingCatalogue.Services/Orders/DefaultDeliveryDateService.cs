using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class DefaultDeliveryDateService : IDefaultDeliveryDateService
    {
        private readonly GPITBuyingCatalogueDbContext dbContext;

        public DefaultDeliveryDateService(GPITBuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: catalogueItemId should be of type CatalogueItemId
        public async Task<DateTime?> GetDefaultDeliveryDate(string callOffId, string catalogueItemId)
        {
            (_, CallOffId id) = CallOffId.Parse(callOffId);

            // TODO - handle case of non-success
            (bool success, var catId) = CatalogueItemId.Parse(catalogueItemId);

            Expression<Func<Order, IEnumerable<DefaultDeliveryDate>>> defaultDeliveryDate = o
                => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catId);

            var date = await dbContext.Orders
                .Where(o => o.Id == id.Id)
                .Include(defaultDeliveryDate)
                .SelectMany(defaultDeliveryDate)
                .Select(d => d.DeliveryDate)
                .SingleOrDefaultAsync();

            return date == default(DateTime) ? null : date;
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: catalogueItemId should be of type CatalogueItemId
        public async Task<DeliveryDateResult> SetDefaultDeliveryDate(string callOffId, string catalogueItemId, DateTime deliveryDate)
        {
            (_, CallOffId id) = CallOffId.Parse(callOffId);

            // TODO - handle case of non-success
            (bool success, var catId) = CatalogueItemId.Parse(catalogueItemId);

            var order = await GetOrder(id, catId);

            DeliveryDateResult addedOrUpdated = order.SetDefaultDeliveryDate(catId, deliveryDate);

            await dbContext.SaveChangesAsync();

            return addedOrUpdated;
        }

        private Task<Order> GetOrder(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId))
                .SingleOrDefaultAsync();
        }
    }
}
