using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderQuantityService : IOrderQuantityService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderQuantityService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task ResetItemQuantities(int orderId, CatalogueItemId catalogueItemId)
        {
            var orderItem = await dbContext.OrderItems
                .Include(x => x.OrderItemRecipients)
                .FirstOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            if (orderItem == null)
            {
                return;
            }

            orderItem.Quantity = null;

            if (orderItem.OrderItemRecipients?.Any() ?? false)
            {
                orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task SetOrderItemQuantity(int orderId, CatalogueItemId catalogueItemId, int quantity)
        {
            var orderItem = await dbContext.OrderItems
                .FirstOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            if (orderItem == null)
            {
                return;
            }

            orderItem.Quantity = quantity;

            dbContext.OrderItems.Update(orderItem);

            await dbContext.SaveChangesAsync();
        }

        public async Task SetServiceRecipientQuantities(int orderId, CatalogueItemId catalogueItemId, List<OrderItemRecipientQuantityDto> quantities)
        {
            if (quantities == null)
            {
                throw new ArgumentNullException(nameof(quantities));
            }

            var recipients = await dbContext.OrderRecipients.Include(x => x.OrderItemRecipients).Where(x => x.OrderId == orderId).ToListAsync();
            if (recipients.Count == 0)
            {
                return;
            }

            foreach (var recipient in recipients)
            {
                var quantity = quantities.FirstOrDefault(x => x.OdsCode == recipient.OdsCode);

                if (quantity != null)
                {
                    recipient.SetQuantityForItem(catalogueItemId, quantity.Quantity);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
