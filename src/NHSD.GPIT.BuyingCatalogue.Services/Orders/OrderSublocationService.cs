using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderSublocationService(
        BuyingCatalogueDbContext dbContext,
        IOrderService orderService)
        : IOrderSublocationService
    {
        private readonly BuyingCatalogueDbContext dbContext =
            dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        private readonly IOrderService orderService =
            orderService ?? throw new ArgumentNullException(nameof(orderService));

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

        public async Task SetSublocationRecipients(
            string parentOdsCode,
            int orderId,
            string sublocationOdsCode,
            HashSet<string> newRecipientOdsCodes)
        {
            ArgumentException.ThrowIfNullOrEmpty(parentOdsCode);
            ArgumentException.ThrowIfNullOrEmpty(sublocationOdsCode);
            if (newRecipientOdsCodes is null or { Count: 0 })
            {
                throw new ArgumentException(@"recipientOdsCodes is null or empty", nameof(newRecipientOdsCodes));
            }

            OrderSublocation sublocation = await dbContext
                .OrderSublocations
                .Where(
                    OrderSublocationPrimaryKeyPredicate(parentOdsCode, orderId, sublocationOdsCode))
                .Include(x => x.Order)
                .ThenInclude(y => y.OrderingParty)
                .Include(x => x.SublocationOrganisation)
                .Include(x => x.SublocationRecipients)
                .FirstAsync();

            HashSet<string> currentRecipientsOdsCodes =
                sublocation.SublocationRecipients.Select(x => x.RecipientOdsCode).ToHashSet();

            OrderWrapper wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(
                sublocation.Order.CallOffId,
                sublocation.Order.OrderingParty.InternalIdentifier);

            HashSet<string> removes = currentRecipientsOdsCodes.Except(newRecipientOdsCodes).ToHashSet();
            HashSet<string> adds = newRecipientOdsCodes.Except(currentRecipientsOdsCodes).ToHashSet();

            List<OrderSublocationRecipient> sublocationRecipientsToAdd = adds
                .Select(x =>
                    wrapper.CreateRecipientWithExistingOrderContext(x, sublocationOdsCode))
                .ToList();

            sublocation.SublocationRecipients.AddRange(sublocationRecipientsToAdd);

            List<OrderSublocationRecipient> sublocationRecipientsToRemove =
                sublocation.SublocationRecipients.Where(x => removes.Contains(x.RecipientOdsCode)).ToList();

            if (wrapper.Order.OrderType.MergerOrSplit && sublocationRecipientsToRemove.Any(x => string.Equals(
                    x.RecipientOdsCode,
                    wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode,
                    StringComparison.OrdinalIgnoreCase)))
            {
                wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode = null;
            }

            sublocation.SublocationRecipients.RemoveRange(sublocationRecipientsToRemove);

            await dbContext.SaveChangesAsync();
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
