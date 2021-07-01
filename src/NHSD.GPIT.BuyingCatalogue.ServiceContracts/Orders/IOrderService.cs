using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderService
    {
        public Task<Order> GetOrder(CallOffId callOffId);

        public Task<IList<Order>> GetOrders(Guid organisationId);

        public Task<Order> GetOrderSummary(CallOffId callOffId);

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId);

        public Task<Order> CreateOrder(string description, string odsCode);

        public Task DeleteOrder(CallOffId callOffId);

        public Task CompleteOrder(CallOffId callOffId);
    }
}
