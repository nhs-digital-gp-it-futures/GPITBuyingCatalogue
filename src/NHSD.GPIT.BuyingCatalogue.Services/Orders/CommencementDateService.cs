using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class CommencementDateService : ICommencementDateService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public CommencementDateService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetCommencementDate(
            CallOffId callOffId,
            string internalOrgId,
            DateTime commencementDate,
            int initialPeriod,
            int maximumTerm)
        {
            var order = await dbContext.Orders.FirstAsync(o => o.Id == callOffId.Id && o.OrderingParty.InternalIdentifier == internalOrgId);

            order.CommencementDate = commencementDate;
            order.InitialPeriod = initialPeriod;
            order.MaximumTerm = maximumTerm;

            await dbContext.SaveChangesAsync();
        }
    }
}
