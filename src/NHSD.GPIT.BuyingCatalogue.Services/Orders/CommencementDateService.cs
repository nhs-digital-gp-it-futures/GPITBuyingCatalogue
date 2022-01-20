using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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

        public async Task SetCommencementDate(CallOffId callOffId, string odsCode, DateTime? commencementDate)
        {
            commencementDate.ValidateNotNull(nameof(commencementDate));

            var order = await dbContext.Orders.SingleAsync(o => o.Id == callOffId.Id && o.OrderingParty.OdsCode == odsCode);
            order.CommencementDate = commencementDate.Value;
            await dbContext.SaveChangesAsync();
        }
    }
}
