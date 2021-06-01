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
    public sealed class CommencementDateService : ICommencementDateService
    {
        private readonly ILogWrapper<CommencementDateService> logger;
        private readonly OrderingDbContext dbContext;

        public CommencementDateService(
            ILogWrapper<CommencementDateService> logger,
            OrderingDbContext dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SetCommencementDate(string callOffId, DateTime? commencementDate)
        {
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            commencementDate.ValidateNotNull(nameof(commencementDate));

            logger.LogInformation($"Setting commencement date for {callOffId} to {commencementDate.Value.ToLongDateString()}");

            var id = CallOffId.Parse(callOffId);
            var order = await dbContext.Orders.SingleAsync(x => x.Id == id.Id);
            order.CommencementDate = commencementDate.Value;
            await dbContext.SaveChangesAsync();
        }
    }
}
