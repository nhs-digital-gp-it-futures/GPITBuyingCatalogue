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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using Notify.Client;
using ServiceContractOdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderService : IOrderService
    {
        public const string OrganisationNameToken = "organisation_name";
        public const string FullOrderCsvToken = "full_order_csv";
        public const string OrderIdToken = "order_id";
        public const string OrderSummaryCsv = "order_summary_csv";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ICsvService csvService;
        private readonly IGovNotifyEmailService emailService;
        private readonly IOrderPdfService pdfService;
        private readonly IOdsService odsService;
        private readonly OrderMessageSettings orderMessageSettings;

        public OrderService(
            BuyingCatalogueDbContext dbContext,
            ICsvService csvService,
            IGovNotifyEmailService emailService,
            IOrderPdfService pdfService,
            IOdsService odsService,
            OrderMessageSettings orderMessageSettings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
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
            var orders = dbContext.Orders
                .Include(o => o.AssociatedServicesOnlyDetails.Solution)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.SelectedFramework)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            var order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderWithCatalogueItemAndPrices(CallOffId callOffId, string internalOrgId)
        {
            var orders = dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(x => x.AssociatedServicesOnlyDetails.Solution)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Parent)
                .ThenInclude(i => i.CatalogueItem)
                .ThenInclude(x => x.CataloguePrices.Where(p => p.PublishedStatus == PublicationStatus.Published))
                .ThenInclude(x => x.CataloguePriceTiers)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.OrderItemPrice)
                .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.SelectedFramework)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationOrganisation)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var previousOrders = await orders.AsNoTracking().Where(o => o.Revision < callOffId.Revision).ToListAsync();

            var order = await orders.FirstOrDefaultAsync(x => x.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderWithOrderItems(CallOffId callOffId, string internalOrgId)
        {
            IQueryable<Order> orders = dbContext.Orders
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
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Services)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Parent)
                .Include(o => o.SelectedFramework)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationOrganisation)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            var order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderWithOrderItemsForFunding(CallOffId callOffId, string internalOrgId)
        {
            var orders = dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.OrderItemPrice)
                .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.SelectedFramework)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            var order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId)
        {
            var orders = dbContext.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            var order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        [ExcludeFromCodeCoverage(
            Justification = "Method uses Temporal tables which the In-Memory provider doesn't support.")]
        public async Task<OrderWrapper> GetOrderForSummary(CallOffId callOffId, string internalOrgId)
        {
            var orders = OrdersForSummary(callOffId, internalOrgId);
            var previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            var order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            var supplier = order.Completed.HasValue
                ? await dbContext.Suppliers.TemporalAsOf(order.Completed.Value)
                    .FirstOrDefaultAsync(x => x.Id == order.SupplierId)
                : null;

            if (supplier != null)
            {
                order.Supplier = supplier;
            }

            foreach (var solution in order.GetSolutions())
            {
                var sla = order.Completed.HasValue
                    ? await dbContext.ServiceLevelAgreements.TemporalAsOf(order.Completed.Value)
                        .Include(x => x.Contacts)
                        .Include(x => x.ServiceLevels)
                        .Include(x => x.ServiceHours)
                        .FirstOrDefaultAsync(x => x.SolutionId == solution.CatalogueItemId)
                    : null;

                if (sla != null)
                {
                    solution.CatalogueItem.Solution.ServiceLevelAgreement = sla;
                }
            }

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderForTaskListStatuses(CallOffId callOffId, string internalOrgId)
        {
            var orders = dbContext.Orders
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
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            var order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderWithSublocations(CallOffId callOffId, string internalOrgId)
        {
            IQueryable<Order> orders = dbContext.Orders
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderSublocations)
                .ThenInclude(os => os.SublocationOrganisation)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            List<Order> previousOrders = await orders
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            Order order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<OrderWrapper> GetOrderWithSublocationsAndSublocationRecipients(
            CallOffId callOffId,
            string internalOrgId)
        {
            IQueryable<Order> orders = dbContext.Orders
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderSublocations)
                .ThenInclude(os => os.SublocationOrganisation)
                .Include(o => o.OrderSublocations)
                .ThenInclude(os => os.SublocationRecipients)
                .ThenInclude(sr => sr.RecipientOdsOrganisation)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            List<Order> previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            Order order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            return new OrderWrapper(order, previousOrders);
        }

        public async Task<int> GetOrderTotalRecipientCount(CallOffId callOffId, string internalOrgId)
        {
            return await dbContext.Orders
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .SelectMany(o => o.OrderSublocations)
                .SelectMany(os => os.SublocationRecipients)
                .AsSplitQuery()
                .CountAsync();
        }

        public async Task<bool> GetOrderHasAnySublocations(CallOffId callOffId, string internalOrgId)
        {
            return await dbContext.Orders
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .AnyAsync(o => o.OrderSublocations.Count > 0);
        }

        public async Task SetSublocations(
            CallOffId callOffId,
            string internalOrgId,
            HashSet<string> sublocationOdsCodes)
        {
            ArgumentException.ThrowIfNullOrEmpty(internalOrgId);

            if (sublocationOdsCodes is null or { Count: 0 })
            {
                throw new ArgumentException(@"sublocationOdsCodes is null or empty", nameof(sublocationOdsCodes));
            }

            var hasSubsequentRevisions = await HasSubsequentRevisions(callOffId);
            if (hasSubsequentRevisions)
            {
                throw new InvalidOperationException(
                    "Can only set sublocations on the most recent order.");
            }

            var orderId = await GetOrderId(callOffId);

            Order order = await dbContext.Orders
                .Where(x => x.OrderingParty.InternalIdentifier == internalOrgId && x.Id == orderId)
                .Include(x => x.OrderingParty)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .FirstAsync();

            IEnumerable<ServiceContractOdsOrganisation> validSublocations =
                await odsService.GetSublocationsByParentOdsCode(order.OrderingParty.ExternalIdentifier);

            var allIdsValid = sublocationOdsCodes.All(x => validSublocations.Any(y => y.OdsCode == x));
            if (!allIdsValid)
            {
                throw new InvalidOperationException(
                    "One or more requested Ids not found or not valid for this organisation.");
            }

            List<string> orderSublocations =
                order.OrderSublocations.Select(x => x.SublocationOdsCode).ToList();

            IEnumerable<string> removes = orderSublocations.Except(sublocationOdsCodes);
            IEnumerable<string> adds = sublocationOdsCodes.Except(orderSublocations);

            IEnumerable<OrderSublocation> locationsToAdd = adds.Select(x => new OrderSublocation
            {
                OrderId = orderId, SublocationOdsCode = x, OwnerOdsCode = order.OrderingParty.ExternalIdentifier,
            });

            order.OrderSublocations.AddRange(locationsToAdd);

            List<OrderSublocation> locationsToRemove =
                order.OrderSublocations.Where(x => removes.Contains(x.SublocationOdsCode)).ToList();

            if (order.OrderType.MergerOrSplit)
            {
                HandlePracticeReorganisationOdsCode();
            }

            order.OrderSublocations.RemoveRange(locationsToRemove);

            await dbContext.SaveChangesAsync();

            return;

            void HandlePracticeReorganisationOdsCode()
            {
                var subLocationContainsPracticeReorg = locationsToRemove.Any(x => x.SublocationRecipients.Any(y =>
                    string.Equals(
                        order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode,
                        y.RecipientOdsCode,
                        StringComparison.OrdinalIgnoreCase)));

                if (subLocationContainsPracticeReorg)
                    order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode = null;
            }
        }

        public async Task SetSublocationsAndRecipients(
            CallOffId callOffId,
            string internalOrgId,
            ICollection<OrderSublocation> orderSublocations)
        {
            if (callOffId.IsAmendment)
            {
                throw new InvalidOperationException("Can only set sublocations and recipients on new orders.");
            }

            ArgumentException.ThrowIfNullOrEmpty(internalOrgId);

            if (orderSublocations is null || orderSublocations is { Count: 0 })
            {
                throw new ArgumentException(@"orderSublocations is null or empty", nameof(orderSublocations));
            }

            var hasSubsequentRevisions = await HasSubsequentRevisions(callOffId);

            if (hasSubsequentRevisions)
            {
                throw new InvalidOperationException(
                    "Can only set sublocations on the most recent order.");
            }

            IQueryable<Order> orders = dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            List<Order> previousOrders = await orders
                .AsNoTracking()
                .Where(o => o.Revision < callOffId.Revision)
                .ToListAsync();

            Order order = await orders
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            var wrapper = new OrderWrapper(order, previousOrders);

            IReadOnlyList<ServiceContractOdsOrganisation> validSublocations =
                await odsService.GetSublocationsByParentOdsCode(wrapper.Order.OrderingParty.ExternalIdentifier);

            var validateSublocations = orderSublocations
                .All(x => validSublocations.Select(y => y.OdsCode).Contains(x.SublocationOdsCode));

            if (!validateSublocations)
            {
                throw new InvalidOperationException("Provided sublocations not valid for this organisation.");
            }

            foreach (OrderSublocation sublocation in orderSublocations)
            {
                IReadOnlyList<ServiceRecipient> validServiceRecipients =
                    await odsService.GetServiceRecipientsBySublocation(sublocation.SublocationOdsCode);

                List<OrderSublocationRecipient> invalidRecipients =
                    sublocation.SublocationRecipients
                        .Where(x => validServiceRecipients.All(y => y.OrgId != x.RecipientOdsCode))
                        .ToList();

                if (invalidRecipients.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Provided recipients not valid for this organisation or its sublocations.");
                }
            }

            wrapper.Order.OrderSublocations = orderSublocations;
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrders(int organisationId)
        {
            return (await dbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.LastUpdatedByUser)
                    .Where(o => o.OrderingPartyId == organisationId)
                    .ToListAsync())
                .GroupBy(x => x.OrderNumber)
                .SelectMany(x =>
                    x.OrderByDescending(y => y.Revision)
                        .TakeUntil(y =>
                            y.OrderStatus is OrderStatus.Completed or OrderStatus.Terminated or OrderStatus.Expired))
                .ToList();
        }

        public async Task<(PagedList<Order> Orders, IEnumerable<CallOffId> OrderIds)> GetPagedOrders(int organisationId, PageOptions options, string search = null)
        {
            options ??= new PageOptions();

            var query = await GetOrders(organisationId);

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
            List<Order> baseData = (await dbContext
                    .Orders
                    .AsNoTracking()
                    .Where(o => o.OrderingPartyId == organisationId)
                    .ToListAsync())
                .GroupBy(x => x.OrderNumber)
                .SelectMany(x =>
                    x.OrderByDescending(y => y.Revision)
                        .TakeUntil(y =>
                            y.OrderStatus is OrderStatus.Completed or OrderStatus.Terminated or OrderStatus.Expired))
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

        public async Task<Order> CreateOrder(
            string description,
            string internalOrgId,
            OrderTypeEnum orderType,
            string selectedFrameworkId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(selectedFrameworkId);

            if (orderType == OrderTypeEnum.Unknown)
            {
                throw new InvalidOperationException($"Something has gone wrong during order triage. Cannot create an order if we dont know the order type {orderType}");
            }

            var framework = await dbContext.Frameworks.Where(f => f.Id == selectedFrameworkId).FirstOrDefaultAsync();
            if (framework == null || framework.IsExpired)
            {
                throw new InvalidOperationException($"Something has gone wrong during order triage. Cannot create an order without a valid selected framework {selectedFrameworkId}");
            }

            var orderingParty = await dbContext.Organisations.FirstAsync(o => o.InternalIdentifier == internalOrgId);

            var order = new Order
            {
                OrderNumber = await dbContext.NextOrderNumber(),
                Revision = 1,
                Description = description,
                OrderingParty = orderingParty,
                OrderType = orderType,
                SelectedFrameworkId = selectedFrameworkId,
            };

            dbContext.Add(order);

            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<Order> AmendOrder(string internalOrgId, CallOffId callOffId)
        {
            Order order = await dbContext.Orders.Include(x => x.OrderingPartyContact)
                .Include(x => x.SupplierContact)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.CatalogueItem)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.OrderItemPrice)
                .ThenInclude(x => x.OrderItemPriceTiers)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Revision == callOffId.Revision && x.OrderNumber == callOffId.OrderNumber
                    && x.OrderingParty.InternalIdentifier == internalOrgId);

            var amendment = order.BuildAmendment(await dbContext.NextRevision(order.OrderNumber));

            dbContext.Orders.Add(amendment);

            await dbContext.SaveChangesAsync();

            return amendment;
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
            dbContext.OrderSublocations.RemoveRange(dbContext.OrderSublocations.Where(x => x.OrderId == order.Id));
            dbContext.OrderItems.RemoveRange(dbContext.OrderItems.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemFunding.RemoveRange(dbContext.OrderItemFunding.Where(x => x.OrderItem.OrderId == order.Id));
            dbContext.OrderItemPriceTiers.RemoveRange(dbContext.OrderItemPriceTiers.Where(x => x.OrderItemPrice.OrderItem.OrderId == order.Id));
            dbContext.OrderItemPrices.RemoveRange(dbContext.OrderItemPrices.Where(x => x.OrderItem.OrderId == order.Id));
            dbContext.Orders.Remove(order);

            await dbContext.SaveChangesAsync();
        }

        public async Task TerminateOrder(CallOffId callOffId, string internalOrgId, int userId, DateTime terminationDate, string reason)
        {
            var orders = dbContext.Orders
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            foreach (var orderRevision in orders)
            {
                TerminateOrder(orderRevision, terminationDate, reason);
            }

            await dbContext.SaveChangesAsync();

            Order order = await orders
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
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationOrganisation)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Revision == callOffId.Revision);

            await SendEmailsAndSave(order, callOffId, userId, true);
        }

        public async Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId)
        {
            var order = (await GetOrderThin(callOffId, internalOrgId)).Order;
            order.Complete();

            dbContext.Entry(order).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();

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
            Order order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemPrice)
                .ThenInclude(oip => oip.OrderItemPriceTiers)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemFunding)
                .Include(o => o.SelectedFramework)
                .Include(o => o.OrderingParty)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .AsSplitQuery()
                .FirstAsync(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision == callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            foreach (var orderItem in order.OrderItems.Where(oi => oi.OrderItemFunding is null))
            {
                var selectedFundingType = OrderItemFundingType.None;

                if (orderItem.TotalCost(order.FlattenedRecipients.ToList()) == 0)
                    selectedFundingType = OrderItemFundingType.NoFundingRequired;
                else if (order.OrderingParty.OrganisationType == OrganisationType.GP)
                    selectedFundingType = OrderItemFundingType.LocalFundingOnly;
                else if (order.HasSingleFundingType)
                    selectedFundingType = order.SelectedFramework.FundingTypes.First().AsOrderItemFundingType();

                if (selectedFundingType == OrderItemFundingType.None) continue;

                orderItem.OrderItemFunding = new OrderItemFunding
                {
                    OrderItemId = orderItem.Id,
                    OrderItemFundingType = selectedFundingType,
                };
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task AcceptCallOffTerms(string internalOrgId, CallOffId callOffId, bool declarationAccepted)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);
            order.AcceptedTermsAndConditions = declarationAccepted;

            await dbContext.SaveChangesAsync();
        }

        private static void TerminateOrder(Order order, DateTime dateOfTermination, string reason)
        {
            order.IsTerminated = true;
            order.OrderTermination = new OrderTermination()
            {
                OrderId = order.Id,
                DateOfTermination = dateOfTermination,
                Reason = reason,
            };
        }

        private async Task SendEmailsAndSave(Order order, CallOffId callOffId, int userId, bool showRevisions = false)
        {
            await using var fullOrderStream = new MemoryStream();

            await csvService.CreateFullOrderCsvAsync(order.Id, order.OrderType, fullOrderStream, showRevisions);

            fullOrderStream.Position = 0;
            var fullOrderBytes = fullOrderStream.ToArray();

            _ = await pdfService.CreateOrderSummaryPdf(order);

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

        private IQueryable<Order> OrdersForSummary(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
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
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .ThenInclude(z => z.OrderItem)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.RecipientOdsOrganisation)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.OrderingParty.InternalIdentifier == internalOrgId);
        }
    }
}
