using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class FundingSourceService : IFundingSourceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public FundingSourceService(
            BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetFundingSource(CallOffId callOffId, string internalOrgId, bool? onlyGms)
        {
            onlyGms.ValidateNotNull(nameof(onlyGms));

            var order = await dbContext.Orders.SingleAsync(o => o.Id == callOffId.Id && o.OrderingParty.InternalIdentifier == internalOrgId);
            order.FundingSourceOnlyGms = onlyGms.Value;
            await dbContext.SaveChangesAsync();
        }
    }
}
