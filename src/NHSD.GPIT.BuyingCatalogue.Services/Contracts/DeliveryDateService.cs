using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public class DeliveryDateService : IDeliveryDateService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public DeliveryDateService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetDeliveryDate(string internalOrgId, CallOffId callOffId, DateTime deliveryDate)
        {
            var order = await dbContext.Orders
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision == callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .FirstAsync();

            order.DeliveryDate = deliveryDate;

            var recipients = await dbContext.OrderItemRecipients
                .Where(x => x.OrderId == order.Id)
                .ToListAsync();

            recipients.ForEach(x => x.DeliveryDate = deliveryDate);

            await dbContext.SaveChangesAsync();
        }

        public async Task SetDeliveryDates(int orderId, CatalogueItemId catalogueItemId, List<RecipientDeliveryDateDto> deliveryDates)
        {
            var recipients = await dbContext.OrderItemRecipients
                .Where(x => x.OrderId == orderId && x.CatalogueItemId == catalogueItemId)
                .ToListAsync();

            foreach (var recipient in recipients)
            {
                var dto = deliveryDates.FirstOrDefault(x => x.OdsCode == recipient.OdsCode);

                if (dto != null)
                {
                    recipient.DeliveryDate = dto.DeliveryDate;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task MatchDeliveryDates(int orderId, CatalogueItemId solutionId, CatalogueItemId serviceId)
        {
            var solutionRecipients = await dbContext.OrderItemRecipients
                .Where(x => x.OrderId == orderId && x.CatalogueItemId == solutionId)
                .ToListAsync();

            var serviceRecipients = await dbContext.OrderItemRecipients
                .Where(x => x.OrderId == orderId && x.CatalogueItemId == serviceId)
                .ToListAsync();

            foreach (var solutionRecipient in solutionRecipients)
            {
                var serviceRecipient = serviceRecipients.FirstOrDefault(x => x.OdsCode == solutionRecipient.OdsCode);

                if (serviceRecipient == null)
                {
                    continue;
                }

                serviceRecipient.DeliveryDate = solutionRecipient.DeliveryDate;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task ResetDeliveryDates(int orderId, DateTime commencementDate)
        {
            var order = await dbContext.Orders.FirstAsync(x => x.Id == orderId);

            if (order.DeliveryDate < commencementDate)
            {
                order.DeliveryDate = null;
            }

            var recipients = await dbContext.OrderItemRecipients
                .Where(x => x.OrderId == orderId && x.DeliveryDate < commencementDate)
                .ToListAsync();

            recipients.ForEach(x => x.DeliveryDate = null);

            await dbContext.SaveChangesAsync();
        }
    }
}
