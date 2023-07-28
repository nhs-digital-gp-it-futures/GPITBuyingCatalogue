using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderItemRecipientService : IOrderItemRecipientService
    {
        private readonly BuyingCatalogueDbContext context;

        public OrderItemRecipientService(BuyingCatalogueDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddOrderItemRecipients(int orderId, CatalogueItemId catalogueItemId, List<ServiceRecipientDto> recipients)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

            var deliveryDate = context.Orders.FirstOrDefault(x => x.Id == orderId)?.DeliveryDate;

            foreach (var recipient in recipients)
            {
                var orderItemRecipient = context.OrderItemRecipients
                    .FirstOrDefault(x => x.OrderId == orderId
                        && x.CatalogueItemId == catalogueItemId
                        && x.OdsCode == recipient.OdsCode);

                if (orderItemRecipient == null)
                {
                    context.OrderItemRecipients.Add(new OrderItemRecipient
                    {
                        OrderId = orderId,
                        CatalogueItemId = catalogueItemId,
                        OdsCode = recipient.OdsCode,
                        DeliveryDate = deliveryDate,
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateOrderItemRecipients(int orderId, CatalogueItemId catalogueItemId, List<ServiceRecipientDto> recipients)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

            var deliveryDate = context.Orders.FirstOrDefault(x => x.Id == orderId)?.DeliveryDate;

            var existingRecipients = context.OrderItemRecipients
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId)
                .ToList();

            foreach (var recipient in recipients)
            {
                var orderItemRecipient = existingRecipients.FirstOrDefault(x => x.OdsCode == recipient.OdsCode);

                if (orderItemRecipient == null)
                {
                    context.OrderItemRecipients.Add(
                        new OrderItemRecipient
                        {
                            OrderId = orderId,
                            CatalogueItemId = catalogueItemId,
                            OdsCode = recipient.OdsCode,
                            DeliveryDate = deliveryDate,
                        });
                }
            }

            foreach (var existing in existingRecipients.Where(x => recipients.All(r => r.OdsCode != x.OdsCode)))
            {
                context.OrderItemRecipients.Remove(existing);
            }

            await context.SaveChangesAsync();
        }
    }
}
