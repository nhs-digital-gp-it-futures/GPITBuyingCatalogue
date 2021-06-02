using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderService
    {
        public Task<Order> GetOrder(string callOffId);

        public Task<IList<Order>> GetOrders(Guid organisationId);

        public Task<Order> GetOrderSummary(CallOffId calloffId);

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId);

        public Task<Order> CreateOrder(string description, string odsCode);

        public Task DeleteOrder(string callOffId);
    }
}
