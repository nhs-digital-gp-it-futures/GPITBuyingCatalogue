using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderSublocationService : IOrderSublocationService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderSublocationService(
            BuyingCatalogueDbContext dbContext
        )
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<int> GetCountForOrderSublocationRecipients(
            string externalOrgId,
            CallOffId callOffId,
            string sublocationOdsCode)
        {
            throw new NotImplementedException();
        }

        public Task<OrderSublocation> GetOrderSublocationWithRecipients(
            string externalOrgId,
            CallOffId callOffId,
            string sublocationOdsCode)
        {
            throw new NotImplementedException();
        }

        public Task SetSublocationRecipients(
            string parentOdsCode,
            CallOffId callOffId,
            string sublocationOdsCode,
            HashSet<string> newRecipientOdsCodes)
        {
            throw new NotImplementedException();
        }
    }
}
