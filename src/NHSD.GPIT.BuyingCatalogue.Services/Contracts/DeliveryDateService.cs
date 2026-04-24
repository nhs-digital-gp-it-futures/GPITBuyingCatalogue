using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public class DeliveryDateService : IDeliveryDateService
    {
        private readonly IOrderService orderService;
        private readonly BuyingCatalogueDbContext dbContext;

        public DeliveryDateService(
            IOrderService orderService,
            BuyingCatalogueDbContext dbContext)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetDeliveryDate(string internalOrgId, CallOffId callOffId, DateTime deliveryDate)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;

            order.DeliveryDate = deliveryDate;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetAllDeliveryDates(string internalOrgId, CallOffId callOffId, DateTime deliveryDate)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;

            order.DeliveryDate = deliveryDate;

            var orderItems = order.OrderItems.ToList();

            orderItems.ForEach(i => wrapper
                .DetermineOrderRecipients(i.CatalogueItemId)
                .ForEach(r => r.SetDeliveryDateForItem(i, deliveryDate)));

            await dbContext.SaveChangesAsync();
        }

        public async Task ResetRecipientDeliveryDates(int orderId)
        {
            List<OrderItemSublocationRecipient> recipients = await dbContext.OrderItemSublocationRecipients
                .Where(x => x.OrderId == orderId)
                .ToListAsync();

            recipients.ForEach(x => x.DeliveryDate = null);

            await dbContext.SaveChangesAsync();
        }

        public async Task SetDeliveryDates(int orderId, OrderItem orderItem, List<RecipientDeliveryDateDto> deliveryDates)
        {
            var recipients = await dbContext.OrderSublocationRecipients
                .Where(x => x.OrderId == orderId)
                .ToListAsync();
            var recipientsDict = recipients.ToDictionary(recipient => recipient.RecipientOdsCode);

            deliveryDates?.ForEach(date =>
            {
                if (recipientsDict.TryGetValue(date.OdsCode, out OrderSublocationRecipient recipient))
                {
                    recipient.SetDeliveryDateForItem(orderItem, date.DeliveryDate);
                }
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task ResetDeliveryDates(int orderId, DateTime commencementDate)
        {
            var order = await dbContext.Orders.FirstAsync(x => x.Id == orderId);

            if (order.DeliveryDate < commencementDate)
            {
                order.DeliveryDate = null;
            }

            List<OrderItemSublocationRecipient> recipients = await dbContext.OrderItemSublocationRecipients
                .Where(x => x.OrderId == orderId && x.DeliveryDate < commencementDate)
                .ToListAsync();

            recipients.ForEach(x => x.DeliveryDate = null);

            await dbContext.SaveChangesAsync();
        }
    }
}
