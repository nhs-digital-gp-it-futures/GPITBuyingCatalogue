using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<int> GetCountForOrderSublocationRecipients(
            string externalOrgId,
            int orderId,
            string sublocationOdsCode)
        {
            return await dbContext.OrderSublocations
                .Where(OrderSublocationPrimaryKeyPredicate(externalOrgId, orderId, sublocationOdsCode))
                .SelectMany(s => s.SublocationRecipients)
                .CountAsync();
        }

        public async Task<OrderSublocation> GetOrderSublocationWithRecipients(
            string externalOrgId,
            int orderId,
            string sublocationOdsCode)
        {
            return await dbContext
                .OrderSublocations
                .AsNoTracking()
                .Where(
                    OrderSublocationPrimaryKeyPredicate(externalOrgId, orderId, sublocationOdsCode))
                .Include(x => x.Order)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .ThenInclude(y => y.RecipientOdsOrganisation)
                .FirstOrDefaultAsync();
        }

        public Task SetSublocationRecipients(
            string parentOdsCode,
            int orderId,
            string sublocationOdsCode,
            HashSet<string> newRecipientOdsCodes)
        {
            throw new NotImplementedException();
        }

        private static Expression<Func<OrderSublocation, bool>> OrderSublocationPrimaryKeyPredicate(
            string parentOdsCode,
            int orderId,
            string sublocationOdsCode)
        {
            return x => x.OwnerOdsCode == parentOdsCode && x.OrderId == orderId
                && x.SublocationOdsCode == sublocationOdsCode;
        }
    }
}
