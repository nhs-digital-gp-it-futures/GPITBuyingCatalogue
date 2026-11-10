using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public async Task SetAllDeliveryDates(string internalOrgId, CallOffId callOffId, DateTime deliveryDate)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;

            order.DeliveryDate = deliveryDate;

            var orderItems = order.OrderItems.ToList();

            orderItems.ForEach(i => wrapper
                .DetermineOrderRecipients(i)
                .ForEach(r => r.SetDeliveryDateForItem(i, deliveryDate)));

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

        public async Task<IEnumerable<OrderSublocationRecipient>> GetSelectedRecipientsAsync(CallOffId callOffId, IEnumerable<string> selectedRecipients)
        {
            return await dbContext.OrderSublocationRecipients
                .Include(x => x.RecipientOdsOrganisation)
                .Include(x => x.ParentSublocation.SublocationOrganisation)
                .Include(x => x.OrderItemSublocationRecipients)
                .Where(x => x.Order.OrderNumber == callOffId.OrderNumber
                    && x.Order.Revision == callOffId.Revision
                    && selectedRecipients.Contains(x.RecipientOdsOrganisation.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItemSublocationRecipient>> GetRecipientOrderItemsDeliveryDatesAsync(CallOffId callOffId, string recipientId)
        {
            return await dbContext.OrderItemSublocationRecipients
                .Include(x => x.Recipient.RecipientOdsOrganisation)
                .Include(x => x.OrderItem.CatalogueItem)
                .Include(x => x.OrderItem.Parent.CatalogueItem)
                .Where(x => x.OrderItem.Order.OrderNumber == callOffId.OrderNumber
                    && x.OrderItem.Order.Revision == callOffId.Revision
                    && x.RecipientOdsCode == recipientId)
                .AsNoTracking()
                .ToListAsync();
        }

        [ExcludeFromCodeCoverage(Justification = "EF Core’s InMemory provider does not support ExecuteUpdateAsync")]
        public async Task SetRecipientOrderItemDeliveryDatesAsync(IEnumerable<RecipientOrderItemDeliveryDateDto> deliveryDates)
        {
            ArgumentNullException.ThrowIfNull(deliveryDates);

            foreach (var deliveryDate in deliveryDates)
            {
                await dbContext.Set<OrderItemSublocationRecipient>()
                    .Where(r => r.Id == deliveryDate.OrderSublocationRecipientId
                        && r.OrderItem.Order.OrderNumber == deliveryDate.CallOffId.OrderNumber
                        && r.OrderItem.Order.Revision == deliveryDate.CallOffId.Revision)
                    .ExecuteUpdateAsync(s => s.SetProperty(r => r.DeliveryDate, _ => deliveryDate.DeliveryDate));
            }
        }

        public async Task<DateTime?> GetMostCommonRecipientDeliveryDateAsync(CallOffId callOffId)
        {
            return await dbContext.OrderItemSublocationRecipients
                .Where(x =>
                    x.OrderItem.Order.OrderNumber == callOffId.OrderNumber &&
                    x.OrderItem.Order.Revision == callOffId.Revision)
                .GroupBy(x => x.DeliveryDate)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .Select(group => group.Key)
                .FirstOrDefaultAsync();
        }
    }
}
