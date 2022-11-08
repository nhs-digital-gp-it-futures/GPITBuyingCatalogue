using System;
using System.Threading.Tasks;
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
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.CommencementDate = commencementDate;
            order.InitialPeriod = initialPeriod;
            order.MaximumTerm = maximumTerm;

            await dbContext.SaveChangesAsync();
        }
    }
}
