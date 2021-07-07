using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class ServiceRecipientService : IServiceRecipientService
    {
        private readonly GPITBuyingCatalogueDbContext context;

        public ServiceRecipientService(GPITBuyingCatalogueDbContext context) =>
            this.context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<List<ServiceRecipient>> GetAllOrderItemRecipients(CallOffId callOffId)
        {
            if (!await context.Orders.AnyAsync(o => o.Id == callOffId.Id))
                return null;

            return await context.Orders
                .Where(o => o.Id == callOffId.Id)
                .SelectMany(o => o.OrderItems)
                .Where(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .SelectMany(o => o.OrderItemRecipients)
                .Select(r => new { r.Recipient.OdsCode, r.Recipient.Name })
                .Distinct()
                .OrderBy(r => r.Name)
                .Select(r => new ServiceRecipient { OdsCode = r.OdsCode, Name = r.Name })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<ServiceRecipient> recipients)
        {
            var requestRecipients = recipients.ToDictionary(r => r.OdsCode);

            var existingServiceRecipients = await context.ServiceRecipients
                .Where(s => requestRecipients.Keys.Contains(s.OdsCode))
                .ToListAsync();

            foreach (var recipient in existingServiceRecipients)
                recipient.Name = requestRecipients[recipient.OdsCode].Name;

            var newServiceRecipients = requestRecipients.Values.Where(p => !existingServiceRecipients.Any(p2 => p2.OdsCode == p.OdsCode));

            // ReSharper disable once MethodHasAsyncOverload
            // Non-async method recommended over async version for most cases (see EF Core docs)
            context.ServiceRecipients.AddRange(newServiceRecipients);

            return existingServiceRecipients.Union(newServiceRecipients).ToDictionary(r => r.OdsCode);
        }
    }
}
