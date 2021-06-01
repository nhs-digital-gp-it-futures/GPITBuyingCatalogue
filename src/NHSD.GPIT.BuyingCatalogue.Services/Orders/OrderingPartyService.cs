using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
            order.ValidateNotNull(nameof(order));
            orderingParty.ValidateNotNull(nameof(orderingParty));
            contact.ValidateNotNull(nameof(contact));

            logger.LogInformation($"Setting ordering party for {order.CallOffId}");

            order.OrderingParty.Name = orderingParty.Name;
            order.OrderingParty.OdsCode = orderingParty.OdsCode;
            order.OrderingParty.Address = orderingParty.Address;
            order.OrderingPartyContact = contact;
            await dbContext.SaveChangesAsync();
        }
    }
}
