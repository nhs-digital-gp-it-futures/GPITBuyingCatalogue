﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderService
    {
        public Task<Order> GetOrderThin(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderWithDefaultDeliveryDatesAndOrderItems(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderForSummary(CallOffId callOffId, string internalOrgId);

        public Task<PagedList<Order>> GetPagedOrders(int organisationId, PageOptions options, string search = null);

        public Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(int organisationId, string searchTerm);

        public Task<Order> GetOrderSummary(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId, string internalOrgId);

        public Task<Order> CreateOrder(string description, string internalOrgId, bool isAssociatedServiceOnly);

        public Task DeleteOrder(CallOffId callOffId, string internalOrgId);

        public Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId, Uri orderSummaryUri);
    }
}
