﻿using System;
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
        private readonly IServiceRecipientService serviceRecipientService;

        public OrderItemRecipientService(BuyingCatalogueDbContext context, IServiceRecipientService serviceRecipientService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
        }

        public async Task AddOrderItemRecipients(int orderId, CatalogueItemId catalogueItemId, List<ServiceRecipientDto> recipients)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

            var deliveryDate = context.Orders.SingleOrDefault(x => x.Id == orderId)?.DeliveryDate;

            foreach (var recipient in recipients)
            {
                await serviceRecipientService.AddServiceRecipient(recipient);

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

            var deliveryDate = context.Orders.SingleOrDefault(x => x.Id == orderId)?.DeliveryDate;

            var existingRecipients = context.OrderItemRecipients
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId)
                .ToList();

            foreach (var recipient in recipients)
            {
                await serviceRecipientService.AddServiceRecipient(recipient);

                var orderItemRecipient = existingRecipients.FirstOrDefault(x => x.OdsCode == recipient.OdsCode);

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

            foreach (var existing in existingRecipients.Where(x => recipients.All(r => r.OdsCode != x.OdsCode)))
            {
                context.OrderItemRecipients.Remove(existing);
            }

            await context.SaveChangesAsync();
        }
    }
}
