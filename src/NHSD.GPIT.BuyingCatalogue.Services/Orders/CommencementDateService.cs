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
    public sealed class CommencementDateService : ICommencementDateService
    {
        private readonly ILogWrapper<CommencementDateService> logger;
        private readonly GPITBuyingCatalogueDbContext dbContext;

        public CommencementDateService(
            ILogWrapper<CommencementDateService> logger,
            GPITBuyingCatalogueDbContext dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // TODO: callOffId should be of type CallOffId
        public async Task SetCommencementDate(string callOffId, DateTime? commencementDate)
        {
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            commencementDate.ValidateNotNull(nameof(commencementDate));

            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Setting commencement date for {callOffId} to {commencementDate.Value.ToLongDateString()}");

            (_, CallOffId id) = CallOffId.Parse(callOffId);
            var order = await dbContext.Orders.SingleAsync(o => o.Id == id.Id);
            order.CommencementDate = commencementDate.Value;
            await dbContext.SaveChangesAsync();
        }
    }
}
