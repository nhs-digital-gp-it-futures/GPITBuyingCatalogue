using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderDescriptionService : IOrderDescriptionService
    {
        private readonly ILogWrapper<OrderDescriptionService> logger;
        private readonly IDbRepository<Order, OrderingDbContext> orderRepository;

        public OrderDescriptionService(
            ILogWrapper<OrderDescriptionService> logger,
            IDbRepository<Order, OrderingDbContext> orderRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task SetOrderDescription(string callOffId, string description)
        {
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            description.ValidateNotNullOrWhiteSpace(nameof(description));

            logger.LogInformation($"Setting order descripion for {callOffId} to {description}");

            var order = (await orderRepository.GetAllAsync(o => o.Id == CallOffId.Parse(callOffId).Id)).Single();
            order.Description = description;
            await orderRepository.SaveChangesAsync();
        }
    }
}
