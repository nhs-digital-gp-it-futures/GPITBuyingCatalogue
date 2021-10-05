using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderService : IOrderService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IOrganisationsService organisationService;
        private readonly ICsvService csvService;
        private readonly IEmailService emailService;
        private readonly OrderMessageSettings orderMessageSettings;

        public OrderService(
            BuyingCatalogueDbContext dbContext,
            IOrganisationsService organisationService,
            ICsvService csvService,
            IEmailService emailService,
            OrderMessageSettings orderMessageSettings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.orderMessageSettings = orderMessageSettings ?? throw new ArgumentNullException(nameof(orderMessageSettings));
        }

        public Task<Order> GetOrder(CallOffId callOffId)
        {
            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem).ThenInclude(a => a.AdditionalService)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .Include(o => o.DefaultDeliveryDates)
                .SingleOrDefaultAsync();
        }

        public async Task<IList<Order>> GetOrders(int organisationId)
        {
            return await dbContext.Organisations
                .Where(o => o.Id == organisationId)
                .Include(o => o.Orders).ThenInclude(o => o.LastUpdatedByUser)
                .SelectMany(o => o.Orders)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Order> GetOrderSummary(CallOffId callOffId)
        {
            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.SupplierContact)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .AsQueryable()
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId)
        {
            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SingleOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(string description, string odsCode)
        {
            var orderingParty = await dbContext.Organisations.SingleAsync(o => o.OdsCode == odsCode);

            var order = new Order
            {
                Description = description,
                OrderingParty = orderingParty,
            };

            dbContext.Add(order);
            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(CallOffId callOffId)
        {
            var order = await dbContext.Orders.Where(o => o.Id == callOffId.Id).SingleAsync();

            order.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task CompleteOrder(CallOffId callOffId)
        {
            var order = await GetOrder(callOffId);

            order.Complete();

            await using var fullOrderStream = new MemoryStream();
            await using var patientOrderStream = new MemoryStream();

            await csvService.CreateFullOrderCsvAsync(order, fullOrderStream);

            fullOrderStream.Position = 0;

            var attachments = new List<EmailAttachment>(2)
            {
                new($"{order.CallOffId}_{order.OrderingParty.OdsCode}_Full.csv", fullOrderStream),
            };

            if (!order.OrderItems.Any(oi =>
               oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
            || oi.CataloguePrice.ProvisioningType != ProvisioningType.Patient))
            {
                await csvService.CreatePatientNumberCsvAsync(order, patientOrderStream);

                patientOrderStream.Position = 0;

                attachments.Add(new($"{order.CallOffId}_{order.OrderingParty.OdsCode}_Patients.csv", patientOrderStream));
            }

            var messageTemplate = orderMessageSettings.EmailMessageTemplate with
            {
                Subject = $"New Order {order.CallOffId}_{order.OrderingParty.OdsCode}",
            };

            var message = new EmailMessage(
                messageTemplate,
                new[] { new EmailAddress(orderMessageSettings.Recipient) },
                attachments);

            await emailService.SendEmailAsync(message);

            await dbContext.SaveChangesAsync();
        }
    }
}
