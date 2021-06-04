using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class ServiceRecipientService : IServiceRecipientService
    {
        private readonly OrderingDbContext context;

        public ServiceRecipientService(OrderingDbContext context) =>
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
                .Select(r => new { r.OdsCodeNavigation.OdsCode, r.OdsCodeNavigation.Name })
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

            var newServiceRecipients = requestRecipients.Values.Except(existingServiceRecipients).ToList();

            // ReSharper disable once MethodHasAsyncOverload
            // Non-async method recommended over async version for most cases (see EF Core docs)
            context.ServiceRecipients.AddRange(newServiceRecipients);

            return existingServiceRecipients.Union(newServiceRecipients).ToDictionary(r => r.OdsCode);
        }
    }
}
