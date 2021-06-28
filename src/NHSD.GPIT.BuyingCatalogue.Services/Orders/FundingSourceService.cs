using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class FundingSourceService : IFundingSourceService
    {
        private readonly ILogWrapper<FundingSourceService> logger;
        private readonly GPITBuyingCatalogueDbContext dbContext;

        public FundingSourceService(
            ILogWrapper<FundingSourceService> logger,
            GPITBuyingCatalogueDbContext dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetFundingSource(CallOffId callOffId, bool? onlyGms)
        {
            onlyGms.ValidateNotNull(nameof(onlyGms));

            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Setting funding source for {callOffId} to {onlyGms.Value}");

            var order = await dbContext.Orders.SingleAsync(o => o.Id == callOffId.Id);
            order.FundingSourceOnlyGms = onlyGms.Value;
            await dbContext.SaveChangesAsync();
        }
    }
}
