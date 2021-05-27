using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderingPartyService : IOrderingPartyService
    {
        private readonly ILogWrapper<OrderingPartyService> logger;
        private readonly OrderingDbContext dbContext;

        public OrderingPartyService(
            ILogWrapper<OrderingPartyService> logger,
            OrderingDbContext dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetOrderingParty(Order order, OrderingParty orderingParty, Contact contact)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (orderingParty is null)
                throw new ArgumentNullException(nameof(order));

            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            order.OrderingParty.Name = orderingParty.Name;
            order.OrderingParty.OdsCode = orderingParty.OdsCode;
            order.OrderingParty.Address = orderingParty.Address;
            order.OrderingPartyContact = contact;

            await dbContext.SaveChangesAsync();
        }
    }
}
