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
        IOdsService odsService,
        IOrderService orderService)
        : IOrderSublocationService
    {
        private readonly BuyingCatalogueDbContext dbContext =
            dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        private readonly IOdsService odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));

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

            OrderWrapper wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(
                sublocation.Order.CallOffId,
                sublocation.Order.OrderingParty.InternalIdentifier);

            var hasSubsequentRevisions = await orderService.HasSubsequentRevisions(sublocation.Order.CallOffId);

            if (hasSubsequentRevisions)
            {
                throw new InvalidOperationException(
                    "Can only set sublocation recipients on the most recent order.");
            }

            var sublocationsAreEditable =
                sublocation.Order.OrderStatus == OrderStatus.InProgress;

            if (!sublocationsAreEditable)
            {
                throw new InvalidOperationException(
                    "Sublocations cannot be edited for this order.");
            }

            IEnumerable<ServiceRecipient> validRecipientsForSublocation =
                await odsService.GetServiceRecipientsBySublocation(sublocationOdsCode);

            HashSet<string> currentRecipientsOdsCodes =
                sublocation.SublocationRecipients.Select(x => x.RecipientOdsCode).ToHashSet();

            var allIdsValid = newRecipientOdsCodes.All(x => validRecipientsForSublocation.Any(y => y.OrgId == x));

            if (!allIdsValid)
            {
                throw new InvalidOperationException(
                    "One or more requested Ids not found or not valid for this sublocation.");
            }

            HashSet<string> adds = [.. newRecipientOdsCodes];
            adds.ExceptWith(currentRecipientsOdsCodes);

            HashSet<string> removes = [.. currentRecipientsOdsCodes];
            removes.ExceptWith(newRecipientOdsCodes);

            List<OrderSublocationRecipient> sublocationRecipientsToAdd = adds
                .Select(x =>
                    wrapper.CreateRecipientWithExistingOrderContext(x, sublocationOdsCode))
                .ToList();

            sublocation.SublocationRecipients.AddRange(sublocationRecipientsToAdd);

            List<OrderSublocationRecipient> sublocationRecipientsToRemove =
                sublocation.SublocationRecipients.Where(x => removes.Contains(x.RecipientOdsCode)).ToList();

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
