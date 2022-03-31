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
        private readonly IServiceRecipientService serviceRecipientService;

        public OrderItemRecipientService(BuyingCatalogueDbContext context, IServiceRecipientService serviceRecipientService)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
        }

        public async Task AddOrderItemRecipients(int orderId, CatalogueItemId catalogueItemId, IEnumerable<ServiceRecipientDto> recipients)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

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
                        Quantity = 1,
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
