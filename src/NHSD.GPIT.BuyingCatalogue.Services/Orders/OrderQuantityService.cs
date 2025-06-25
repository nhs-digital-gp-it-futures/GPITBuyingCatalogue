using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                .FirstOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            List<OrderItemSublocationRecipient> orderItemSublocationRecipients = await dbContext
                .OrderItemSublocationRecipients
                .Where(x => x.OrderId == orderId)
                .ToListAsync();

            if (orderItem == null)
            {
                return;
            }

            orderItem.Quantity = null;

            IEnumerable<OrderItemSublocationRecipient> toDelete =
                orderItemSublocationRecipients.Where(i => i.CatalogueItemId == catalogueItemId);
            dbContext.OrderItemSublocationRecipients.RemoveRange(toDelete);

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

        public async Task SetServiceRecipientQuantities(int orderId, CatalogueItemId catalogueItemId, int quantity)
        {
            List<OrderSublocationRecipient> recipients = await dbContext
                .OrderSublocationRecipients.Where(x => x.OrderId == orderId)
                .Include(x => x.OrderItemSublocationRecipients)
                .ToListAsync();

            if (recipients.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No recipients exist for the provided {nameof(orderId)}: {orderId}");
            }

            recipients.ForEach(x => x.SetQuantityForItem(catalogueItemId, quantity));

            await dbContext.SaveChangesAsync();
        }

        public async Task SetServiceRecipientQuantities(
            int orderId,
            CatalogueItemId catalogueItemId,
            List<OrderItemRecipientQuantityDto> quantities)
        {
            if (quantities is null || quantities is { Count: 0 })
            {
                throw new ArgumentException($"{nameof(quantities)} is null or empty");
            }

            List<OrderSublocationRecipient> recipients = await dbContext
                .OrderSublocationRecipients.Where(x => x.OrderId == orderId)
                .Include(x => x.OrderItemSublocationRecipients)
                .ToListAsync();

            if (recipients.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No recipients exist for the provided {nameof(orderId)}: {orderId}");
            }

            foreach (OrderItemRecipientQuantityDto quantity in quantities)
            {
                OrderSublocationRecipient recipient = recipients.First(x => x.RecipientOdsCode == quantity.OdsCode);

                recipient.SetQuantityForItem(catalogueItemId, quantity.Quantity);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
