using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

        public async Task SetOrderingPartyContact(CallOffId callOffId, Contact contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            var order = await dbContext.Order(callOffId);

            order.OrderingPartyContact = contact;

            await dbContext.SaveChangesAsync();
        }
    }
}
