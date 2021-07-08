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
        private readonly BuyingCatalogueDbContext dbContext;

        public DefaultDeliveryDateService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<DateTime?> GetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<DefaultDeliveryDate>>> defaultDeliveryDate = o
                => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId);

            var date = await dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(defaultDeliveryDate)
                .SelectMany(defaultDeliveryDate)
                .Select(d => d.DeliveryDate)
                .SingleOrDefaultAsync();

            return date == default(DateTime) ? null : date;
        }

        public async Task<DeliveryDateResult> SetDefaultDeliveryDate(CallOffId callOffId, CatalogueItemId catalogueItemId, DateTime deliveryDate)
        {
            var order = await GetOrder(callOffId, catalogueItemId);

            DeliveryDateResult addedOrUpdated = order.SetDefaultDeliveryDate(catalogueItemId, deliveryDate);

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
