using System;
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
    public class OrderPriceService : IOrderPriceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderPriceService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddPrice(int orderId, CataloguePrice price, List<OrderPricingTierDto> agreedPrices)
        {
            if (price == null)
            {
                throw new ArgumentNullException(nameof(price));
            }

            if (agreedPrices == null)
            {
                throw new ArgumentNullException(nameof(agreedPrices));
            }

            var orderItem = await dbContext.OrderItems
                .SingleOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == price.CatalogueItemId);

            if (orderItem != null)
            {
                var orderItemPrice = new OrderItemPrice(orderItem, price);

                foreach (var agreedPrice in agreedPrices)
                {
                    var tier = orderItemPrice.OrderItemPriceTiers
                        .Single(x => x.LowerRange == agreedPrice.LowerRange && x.UpperRange == agreedPrice.UpperRange);

                    tier.Price = agreedPrice.Price;
                }

                dbContext.OrderItemPrices.Add(orderItemPrice);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdatePrice(int orderId, CatalogueItemId catalogueItemId, List<OrderPricingTierDto> agreedPrices)
        {
            if (agreedPrices == null)
            {
                throw new ArgumentNullException(nameof(agreedPrices));
            }

            var orderItem = await dbContext.OrderItems
                .Include(x => x.OrderItemPrice)
                .ThenInclude(x => x.OrderItemPriceTiers)
                .Where(x => x.OrderItemPrice != null)
                .SingleOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            if (orderItem != null)
            {
                foreach (var tier in orderItem.OrderItemPrice.OrderItemPriceTiers)
                {
                    var agreedPrice = agreedPrices
                        .FirstOrDefault(x => x.LowerRange == tier.LowerRange
                            && x.UpperRange == tier.UpperRange);

                    if (agreedPrice == null
                        || agreedPrice.Price == tier.Price)
                    {
                        continue;
                    }

                    tier.Price = agreedPrice.Price;

                    dbContext.OrderItemPriceTiers.Update(tier);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task SetOrderItemQuantity(int orderId, CatalogueItemId catalogueItemId, int quantity)
        {
            var orderItem = await dbContext.OrderItems
                .SingleOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            if (orderItem == null)
            {
                return;
            }

            orderItem.Quantity = quantity;

            dbContext.OrderItems.Update(orderItem);

            await dbContext.SaveChangesAsync();
        }

        public async Task SetServiceRecipientQuantities(int orderId, CatalogueItemId catalogueItemId, List<OrderPricingTierQuantityDto> quantities)
        {
            if (quantities == null)
            {
                throw new ArgumentNullException(nameof(quantities));
            }

            var orderItem = await dbContext.OrderItems
                .Include(x => x.OrderItemRecipients)
                .SingleOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            if (orderItem == null)
            {
                return;
            }

            foreach (var recipient in orderItem.OrderItemRecipients)
            {
                var quantity = quantities.FirstOrDefault(x => x.OdsCode == recipient.OdsCode);

                if (quantity != null)
                {
                    recipient.Quantity = quantity.Quantity;
                }
            }

            dbContext.OrderItems.Update(orderItem);

            await dbContext.SaveChangesAsync();
        }
    }
}
