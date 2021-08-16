using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderingPartyService : IOrderingPartyService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderingPartyService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // TODO: remove orderingParty
        public Task SetOrderingParty(Order order, Contact contact)
        {
            order.ValidateNotNull(nameof(order));
            contact.ValidateNotNull(nameof(contact));

            order.OrderingPartyContact = contact;
            return dbContext.SaveChangesAsync();
        }
    }
}
