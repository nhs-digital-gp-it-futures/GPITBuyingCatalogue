using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderService
    {
        // TODO: callOffId should be of type CallOffId
        public Task<Order> GetOrder(string callOffId);

        public Task<IList<Order>> GetOrders(Guid organisationId);

        public Task<Order> GetOrderSummary(CallOffId callOffId);

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId);

        public Task<Order> CreateOrder(string description, string odsCode);

        // TODO: callOffId should be of type CallOffId
        public Task DeleteOrder(string callOffId);
    }
}
