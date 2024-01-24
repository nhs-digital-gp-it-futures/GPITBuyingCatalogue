using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using Notify.Client;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderService : IOrderService
    {
        public const string OrganisationNameToken = "organisation_name";
        public const string FullOrderCsvToken = "full_order_csv";
        public const string OrderIdToken = "order_id";
        public const string OrderSummaryLinkToken = "order_summary_link";
        public const string OrderSummaryCsv = "order_summary_csv";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ICsvService csvService;
        private readonly IGovNotifyEmailService emailService;
        private readonly IOrderPdfService pdfService;
        private readonly OrderMessageSettings orderMessageSettings;

        public OrderService(
            BuyingCatalogueDbContext dbContext,
            ICsvService csvService,
            IGovNotifyEmailService emailService,
            IOrderPdfService pdfService,
            OrderMessageSettings orderMessageSettings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.orderMessageSettings = orderMessageSettings ?? throw new ArgumentNullException(nameof(orderMessageSettings));
        }

        public async Task<int> GetOrderId(CallOffId callOffId)
        {
            return await dbContext.OrderId(callOffId);
        }

        public async Task<int> GetOrderId(string internalOrgId, CallOffId callOffId)
        {
            return await dbContext.OrderId(internalOrgId, callOffId);
        }

        public async Task<bool> HasSubsequentRevisions(CallOffId callOffId)
        {
            return await dbContext.Orders
                .AnyAsync(x => x.OrderNumber == callOffId.OrderNumber && x.Revision > callOffId.Revision);
        }

        public async Task<OrderWrapper> GetOrderThin(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(o => o.AssociatedServicesOnlyDetails.Solution)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.SelectedFramework)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithCatalogueItemAndPrices(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(x => x.AssociatedServicesOnlyDetails.Solution)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                    .ThenInclude(x => x.CataloguePrices.Where(p => p.PublishedStatus == PublicationStatus.Published))
                    .ThenInclude(x => x.CataloguePriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.SelectedFramework)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OdsOrganisation)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithOrderItems(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(x => x.AssociatedServicesOnlyDetails.Solution)
                .Include(x => x.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers.OrderBy(t => t.LowerRange))
                .Include(o => o.SelectedFramework)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OdsOrganisation)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OrderItemRecipients)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithOrderItemsForFunding(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.SelectedFramework)
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OdsOrganisation)
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        [ExcludeFromCodeCoverage(Justification = "Method uses Temporal tables which the In-Memory provider doesn't support.")]
        public async Task<OrderWrapper> GetOrderForSummary(CallOffId callOffId, string internalOrgId)
        {
            List<Order> orders = await OrdersForSummary(callOffId, internalOrgId);

            foreach (var order in orders)
            {
                var supplier = order.Completed.HasValue
                    ? await dbContext.Suppliers.TemporalAsOf(order.Completed.Value).FirstOrDefaultAsync(x => x.Id == order.SupplierId)
                    : null;

                if (supplier != null)
                {
                    order.Supplier = supplier;
                }
            }

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderForTaskListStatuses(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.ContractFlags)
                .Include(x => x.Contract)
                    .ThenInclude(x => x.ImplementationPlan)
                .Include(x => x.Contract)
                    .ThenInclude(x => x.ContractBilling)
                .Include(x => x.LastUpdatedByUser)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.CatalogueItem)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.OrderItemFunding)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.OrderItemPrice)
                .Include(x => x.OrderingPartyContact)
                .Include(x => x.OrderingParty)
                .Include(x => x.Supplier)
                .Include(x => x.SupplierContact)
                .Include(x => x.SelectedFramework)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OdsOrganisation)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(x => x.OrderNumber == callOffId.OrderNumber
                    && x.Revision <= callOffId.Revision
                    && x.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<(PagedList<Order> Orders, IEnumerable<CallOffId> OrderIds)> GetPagedOrders(int organisationId, PageOptions options, string search = null)
        {
            options ??= new PageOptions();

            var query = (await dbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.LastUpdatedByUser)
                    .Where(o => o.OrderingPartyId == organisationId)
                    .ToListAsync())
                .GroupBy(x => x.OrderNumber)
                .SelectMany(x => x.OrderByDescending(y => y.Revision).TakeUntil(y => y.OrderStatus is OrderStatus.Completed or OrderStatus.Terminated))
                .ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.CallOffId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                    || o.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            options.TotalNumberOfItems = query.Count;
            var orderIds = query.Select(x => x.CallOffId);

            query = query
                .OrderByDescending(o => o.LastUpdated)
                .ToList();

            if (options.PageNumber != 0)
                query = query.Skip((options.PageNumber - 1) * options.PageSize).ToList();

            var results = query.Take(options.PageSize).ToList();

            return (new PagedList<Order>(results, options), orderIds);
        }

        public async Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(int organisationId, string searchTerm)
        {
            var baseData = (await dbContext
                    .Orders
                    .AsNoTracking()
                    .Where(o => o.OrderingPartyId == organisationId)
                    .ToListAsync())
                .GroupBy(x => x.OrderNumber)
                .SelectMany(x => x.OrderByDescending(y => y.Revision).TakeUntil(y => y.OrderStatus is OrderStatus.Completed or OrderStatus.Terminated))
                .ToList();

            var matches = baseData
                .Where(o => o.CallOffId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                         || o.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(o => new SearchFilterModel
                {
                    Title = o.Description,
                    Category = o.CallOffId.ToString(),
                });

            return matches.ToList();
        }

        public async Task<Order> CreateOrder(string description, string internalOrgId, OrderTriageValue? orderTriageValue, OrderTypeEnum orderType)
        {
            if (orderType == OrderTypeEnum.Unknown)
            {
                throw new InvalidOperationException($"Something has gone wrong during order triage. Cannot create an order if we dont know the order type {orderType}");
            }

            var orderingParty = await dbContext.Organisations.FirstAsync(o => o.InternalIdentifier == internalOrgId);

            var order = new Order
            {
                OrderNumber = await dbContext.NextOrderNumber(),
                Revision = 1,
                Description = description,
                OrderingParty = orderingParty,
                OrderType = orderType,
                OrderTriageValue = orderTriageValue,
            };

            dbContext.Add(order);

            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<Order> AmendOrder(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = new OrderWrapper(await OrdersForSummary(callOffId, internalOrgId));
            var order = orderWrapper.Order;

            var amendment = order.BuildAmendment(await dbContext.NextRevision(order.OrderNumber));

            dbContext.Add(amendment);

            await dbContext.SaveChangesAsync();

            return amendment;
        }

        public async Task EnsureOrderItemsForAmendment(string internalOrgId, CallOffId callOffId)
        {
            if (callOffId.IsAmendment)
            {
                var orderWrapper = await GetOrderWithOrderItems(callOffId, internalOrgId);
                var order = orderWrapper.Order;
                order.InitialiseOrderItemsFrom(orderWrapper.RolledUp.OrderItems);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteOrder(CallOffId callOffId, string internalOrgId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task HardDeleteOrder(CallOffId callOffId, string internalOrgId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            dbContext.ImplementationPlanMilestones.RemoveRange(dbContext.ImplementationPlanMilestones.Where(x => x.Plan.Contract.OrderId == order.Id));
            dbContext.ImplementationPlans.RemoveRange(dbContext.ImplementationPlans.Where(x => x.Contract.OrderId == order.Id));
            dbContext.Contracts.RemoveRange(dbContext.Contracts.Where(x => x.OrderId == order.Id));
            dbContext.ContractFlags.RemoveRange(dbContext.ContractFlags.Where(x => x.OrderId == order.Id));
            dbContext.OrderDeletionApprovals.RemoveRange(dbContext.OrderDeletionApprovals.Where(x => x.OrderId == order.Id));
            dbContext.OrderRecipients.RemoveRange(dbContext.OrderRecipients.Where(x => x.OrderId == order.Id));
            dbContext.OrderItems.RemoveRange(dbContext.OrderItems.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemFunding.RemoveRange(dbContext.OrderItemFunding.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemPriceTiers.RemoveRange(dbContext.OrderItemPriceTiers.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemPrices.RemoveRange(dbContext.OrderItemPrices.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemRecipients.RemoveRange(dbContext.OrderItemRecipients.Where(x => x.OrderId == order.Id));
            dbContext.Orders.Remove(order);

            await dbContext.SaveChangesAsync();
        }

        public async Task TerminateOrder(CallOffId callOffId, string internalOrgId, int userId, DateTime terminationDate, string reason)
        {
            var orderWrapper = await GetOrderWithOrderItems(callOffId, internalOrgId);

            TerminateOrder(orderWrapper.Order, terminationDate, reason);

            foreach (var order in orderWrapper.PreviousOrders)
            {
                TerminateOrder(order, terminationDate, reason);
            }

            await dbContext.SaveChangesAsync();

            await SendEmailsAndSave(orderWrapper.Order, callOffId, userId, true);
        }

        public async Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId)
        {
            var order = (await GetOrderThin(callOffId, internalOrgId)).Order;
            order.Complete();

            await SendEmailsAndSave(order, callOffId, userId);
        }

        public async Task<List<Order>> GetUserOrders(int userId)
        {
            return await dbContext.Orders
                .Where(o => o.LastUpdatedBy == userId)
                .OrderByDescending(o => o.LastUpdated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SetSolutionId(string internalOrgId, CallOffId callOffId, CatalogueItemId solutionId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.AssociatedServicesOnlyDetails.SolutionId = solutionId;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetOrderPracticeReorganisationRecipient(string internalOrgId, CallOffId callOffId, string odsCode)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode = odsCode;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetFundingSourceForForceFundedItems(string internalOrgId, CallOffId callOffId)
        {
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemPrice)
                    .ThenInclude(oip => oip.OrderItemPriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemFunding)
                .Include(o => o.SelectedFramework)
                .Include(o => o.OrderingParty)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OdsOrganisation)
                .AsSplitQuery()
                .FirstAsync(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision == callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            foreach (var orderItem in order.OrderItems.Where(oi => oi.OrderItemFunding is null))
            {
                var selectedFundingType = OrderItemFundingType.None;

                if (orderItem.TotalCost(order.OrderRecipients) == 0)
                    selectedFundingType = OrderItemFundingType.NoFundingRequired;
                else if (order.OrderingParty.OrganisationType == OrganisationType.GP)
                    selectedFundingType = OrderItemFundingType.LocalFundingOnly;
                else if (order.HasSingleFundingType)
                    selectedFundingType = order.SelectedFramework.FundingTypes.First().AsOrderItemFundingType();

                if (selectedFundingType == OrderItemFundingType.None) continue;

                var orderId = await dbContext.OrderId(internalOrgId, callOffId);

                orderItem.OrderItemFunding = new OrderItemFunding
                {
                    OrderId = orderId,
                    CatalogueItemId = orderItem.CatalogueItemId,
                    OrderItemFundingType = selectedFundingType,
                };
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSelectedFramework(string internalOrgId, CallOffId callOffId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.SelectedFrameworkId = null;

            await dbContext.SaveChangesAsync();
        }

        private static void TerminateOrder(Order order, DateTime dateOfTermination, string reason)
        {
            order.IsTerminated = true;
            order.OrderTermination = new OrderTermination()
            {
                OrderId = order.Id, DateOfTermination = dateOfTermination, Reason = reason,
            };
        }

        private async Task SendEmailsAndSave(Order order, CallOffId callOffId, int userId, bool showRevisions = false)
        {
            await using var fullOrderStream = new MemoryStream();

            await csvService.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream, showRevisions);

            fullOrderStream.Position = 0;
            var fullOrderBytes = fullOrderStream.ToArray();

            var adminTokens = new Dictionary<string, dynamic>
            {
                { OrganisationNameToken, order.OrderingParty.Name },
                { FullOrderCsvToken, NotificationClient.PrepareUpload(fullOrderBytes, true) },
            };

            var templateId = order.IsTerminated ? orderMessageSettings.OrderTerminatedAdminTemplateId : orderMessageSettings.SingleCsvTemplateId;

            var userTokens = new Dictionary<string, dynamic>
            {
                { OrderIdToken, $"{callOffId}" },
                { OrderSummaryCsv, NotificationClient.PrepareUpload(fullOrderBytes, true) },
            };

            dbContext.Entry(order).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            _ = await pdfService.CreateOrderSummaryPdf(order);

            var userEmail = dbContext.Users.First(x => x.Id == userId).Email;

            await Task.WhenAll(
                emailService.SendEmailAsync(orderMessageSettings.Recipient.Address, templateId, adminTokens),
                emailService.SendEmailAsync(userEmail, GetUserTemplateId(order), userTokens));
        }

        private string GetUserTemplateId(Order order)
        {
            return order.IsTerminated
                ? orderMessageSettings.OrderTerminatedUserTemplateId
                : order.OrderType.AssociatedServicesOnly
                    ? orderMessageSettings.UserAssociatedServiceTemplateId
                    : order.IsAmendment
                        ? orderMessageSettings.UserAmendTemplateId
                        : orderMessageSettings.UserTemplateId;
        }

        private async Task<List<Order>> OrdersForSummary(CallOffId callOffId, string internalOrgId)
        {
            return await dbContext.Orders
                .Include(x => x.ContractFlags)
                .Include(x => x.Contract)
                    .ThenInclude(i => i.ImplementationPlan)
                        .ThenInclude(m => m.Milestones)
                .Include(x => x.Contract)
                    .ThenInclude(i => i.ContractBilling)
                        .ThenInclude(m => m.ContractBillingItems)
                            .ThenInclude(m => m.Milestone)
                .Include(x => x.Contract)
                    .ThenInclude(i => i.ContractBilling)
                        .ThenInclude(m => m.ContractBillingItems)
                            .ThenInclude(m => m.OrderItem)
                                .ThenInclude(m => m.CatalogueItem)
                .Include(x => x.Contract)
                    .ThenInclude(i => i.ContractBilling)
                        .ThenInclude(m => m.Requirements)
                            .ThenInclude(m => m.OrderItem)
                                .ThenInclude(m => m.CatalogueItem)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.AssociatedServicesOnlyDetails.Solution)
                .ThenInclude(o => o.Solution)
                .ThenInclude(o => o.FrameworkSolutions)
                .ThenInclude(o => o.Framework)
                .Include(x => x.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.SelectedFramework)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                    .ThenInclude(ci => ci.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(fs => fs.Framework)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers.OrderBy(pt => pt.LowerRange))
                .Include(o => o.OrderTermination)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderRecipients).ThenInclude(x => x.OdsOrganisation)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();
        }
    }
}
