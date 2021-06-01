using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class FundingSourceService : IFundingSourceService
    {
        private readonly ILogWrapper<FundingSourceService> logger;
        private readonly OrderingDbContext dbContext;

        public FundingSourceService(
            ILogWrapper<FundingSourceService> logger,
            OrderingDbContext dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetFundingSource(string callOffId, bool? onlyGms)
        {
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            onlyGms.ValidateNotNull(nameof(onlyGms));

            logger.LogInformation($"Setting funding source for {callOffId} to {onlyGms.Value}");

            var id = CallOffId.Parse(callOffId);
            var order = await dbContext.Orders.SingleAsync(x => x.Id == id.Id);
            order.FundingSourceOnlyGms = onlyGms.Value;
            await dbContext.SaveChangesAsync();
        }
    }
}
