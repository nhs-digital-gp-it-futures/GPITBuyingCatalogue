using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderDescriptionService : IOrderDescriptionService
    {
        private readonly IDbRepository<Order, GPITBuyingCatalogueDbContext> orderRepository;

        public OrderDescriptionService(
            IDbRepository<Order, GPITBuyingCatalogueDbContext> orderRepository)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task SetOrderDescription(CallOffId callOffId, string description)
        {
            description.ValidateNotNullOrWhiteSpace(nameof(description));

            var order = (await orderRepository.GetAllAsync(o => o.Id == callOffId.Id)).Single();
            order.Description = description;
            await orderRepository.SaveChangesAsync();
        }
    }
}
