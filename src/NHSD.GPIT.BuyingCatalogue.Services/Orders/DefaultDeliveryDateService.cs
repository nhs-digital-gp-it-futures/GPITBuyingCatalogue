using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class DefaultDeliveryDateService : IDefaultDeliveryDateService
    {
        private readonly OrderingDbContext dbContext;

        public DefaultDeliveryDateService(OrderingDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<DateTime?> GetDefaultDeliveryDate(string callOffId, string catalogueItemId)
        {
            var id = CallOffId.Parse(callOffId);

            // TODO - handle case of non-success
            (var success, var catId) = CatalogueItemId.Parse(catalogueItemId);

            Expression<Func<Order, IEnumerable<DefaultDeliveryDate>>> defaultDeliveryDate = o
                => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catId);

            var date = await dbContext.Orders
                .Where(o => o.Id == id.Id)
                .Include(defaultDeliveryDate)
                .SelectMany(defaultDeliveryDate)
                .Select(d => d.DeliveryDate)
                .SingleOrDefaultAsync();

            if (date == default(DateTime))
                return null;

            return date;
        }

        public async Task<DeliveryDateResult> SetDefaultDeliveryDate(string callOffId, string catalogueItemId, DateTime deliveryDate)
        {
            var id = CallOffId.Parse(callOffId);

            // TODO - handle case of non-success
            (var success, var catId) = CatalogueItemId.Parse(catalogueItemId);

            var order = await GetOrder(id, catId);

            DeliveryDateResult addedOrUpdated = order.SetDefaultDeliveryDate(catId, deliveryDate);

            await dbContext.SaveChangesAsync();

            return addedOrUpdated;
        }

        private async Task<Order> GetOrder(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            return await dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.DefaultDeliveryDates.Where(d => d.CatalogueItemId == catalogueItemId))
                .SingleOrDefaultAsync();
        }
    }
}
