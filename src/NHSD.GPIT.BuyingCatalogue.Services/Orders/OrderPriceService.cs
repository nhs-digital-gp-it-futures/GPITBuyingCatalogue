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
    }
}
