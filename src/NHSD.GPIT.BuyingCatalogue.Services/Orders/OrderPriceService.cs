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
    public class OrderPriceService : IOrderPriceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderPriceService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertPrice(int orderId, CataloguePrice price, List<OrderPricingTierDto> agreedPrices)
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
                .Include(x => x.OrderItemPrice)
                .ThenInclude(x => x.OrderItemPriceTiers)
                .SingleOrDefaultAsync(x => x.OrderId == orderId
                    && x.CatalogueItemId == price.CatalogueItemId);

            if (orderItem == null)
            {
                return;
            }

            if (orderItem.OrderItemPrice != null)
            {
                dbContext.OrderItemPrices.Remove(orderItem.OrderItemPrice);
                dbContext.OrderItemPriceTiers.RemoveRange(orderItem.OrderItemPrice.OrderItemPriceTiers);
            }

            orderItem.OrderItemPrice = new OrderItemPrice(orderItem, price);

            foreach (var agreedPrice in agreedPrices)
            {
                var tier = orderItem.OrderItemPrice
                    .OrderItemPriceTiers
                    .Single(x => x.LowerRange == agreedPrice.LowerRange
                        && x.UpperRange == agreedPrice.UpperRange);

                tier.Price = agreedPrice.Price;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
