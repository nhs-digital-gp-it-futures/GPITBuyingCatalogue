using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderService
    {
        public Task<Order> GetOrderThin(CallOffId callOffId);

        public Task<Order> GetOrderWithDefaultDeliveryDatesAndOrderItems(CallOffId callOffId);

        public Task<Order> GetOrderWithSupplier(CallOffId callOffId);

        public Task<Order> GetOrderForSummary(CallOffId callOffId);

        public Task<IList<Order>> GetOrders(int organisationId);

        public Task<Order> GetOrderSummary(CallOffId callOffId);

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId);

        public Task<Order> CreateOrder(string description, string odsCode);

        public Task DeleteOrder(CallOffId callOffId);

        public Task CompleteOrder(CallOffId callOffId);
    }
}
