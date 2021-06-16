using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ILogWrapper<OrderService> logger;
        private readonly OrderingDbContext dbContext;
        private readonly IDbRepository<Order, OrderingDbContext> orderRepository;
        private readonly IOrganisationsService organisationService;

        public OrderService(
            ILogWrapper<OrderService> logger,
            OrderingDbContext dbContext,
            IDbRepository<Order, OrderingDbContext> orderRepository,
            IOrganisationsService organisationService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        public async Task<Order> GetOrder(string callOffId)
        {
            var id = CallOffId.Parse(callOffId);

            return await dbContext.Orders
                .Where(o => o.Id == id.Id)
                .Include(o => o.OrderingParty).ThenInclude(p => p.Address)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier).ThenInclude(s => s.Address)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems).Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.OdsCodeNavigation)
                .Include(o => o.OrderItems).ThenInclude(i => i.PricingUnitNameNavigation)
                .Include(o => o.DefaultDeliveryDates)
                .SingleOrDefaultAsync();
        }

        public async Task<IList<Order>> GetOrders(Guid organisationId)
        {
            return await dbContext.OrderingParties
                .Where(o => o.Id == organisationId)
                .SelectMany(o => o.Orders)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order> GetOrderSummary(CallOffId callOffId)
        {
            return await dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.SupplierContact)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .Include(o => o.OrderProgress)
                .AsQueryable()
                .SingleOrDefaultAsync();
        }

        public async Task<Order> GetOrderForStatusUpdate(CallOffId callOffId)
        {
            return await dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.OdsCodeNavigation)
                .Include(o => o.OrderItems).ThenInclude(i => i.PricingUnitNameNavigation)
                .Include(o => o.OrderProgress)
                .SingleOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(string description, string odsCode)
        {
            var organisationId = (await organisationService.GetOrganisationByOdsCode(odsCode)).OrganisationId;

            OrderingParty orderingParty = (await GetOrderingParty(organisationId)) ?? new OrderingParty { Id = organisationId };

            var order = new Order
            {
                Description = description,
                OrderingParty = orderingParty,
            };

            dbContext.Add(order);
            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(string callOffId)
        {
            var id = CallOffId.Parse(callOffId);

            var order = await dbContext.Orders.Where(o => o.Id == id.Id).SingleAsync();

            order.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }

        private async Task<OrderingParty> GetOrderingParty(Guid organisationId)
        {
            return await dbContext.OrderingParties.FindAsync(organisationId);
        }
    }
}
