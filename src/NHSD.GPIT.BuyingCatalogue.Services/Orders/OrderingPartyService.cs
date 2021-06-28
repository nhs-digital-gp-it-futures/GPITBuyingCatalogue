using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderingPartyService : IOrderingPartyService
    {
        private readonly ILogWrapper<OrderingPartyService> logger;
        private readonly GPITBuyingCatalogueDbContext dbContext;

        public OrderingPartyService(
            ILogWrapper<OrderingPartyService> logger,
            GPITBuyingCatalogueDbContext dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // TODO: remove orderingParty
        public Task SetOrderingParty(Order order, Organisation orderingParty, Contact contact)
        {
            order.ValidateNotNull(nameof(order));
            contact.ValidateNotNull(nameof(contact));

            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Setting ordering party for {order.CallOffId}");

            order.OrderingPartyContact = contact;
            return dbContext.SaveChangesAsync();
        }
    }
}
