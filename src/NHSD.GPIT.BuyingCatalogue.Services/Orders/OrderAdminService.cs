using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "LINQ to SQL")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity", Justification = "LINQ to SQL")]
    public sealed class OrderAdminService : IOrderAdminService
    {
        private const string NullOrEmptySearchExceptionMessage = "Search cannot be null or empty";
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderAdminService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Order> GetOrder(CallOffId callOffId)
        {
            var order = await dbContext.Orders
                .IgnoreQueryFilters()
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.SelectedFramework)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.OrderItemFunding)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.OrderItemPrice).ThenInclude(oip => oip.OrderItemPriceTiers)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.OrderNumber == callOffId.OrderNumber && o.Revision == callOffId.Revision);

            if (order?.Completed.HasValue ?? false)
            {
                var supplier = await dbContext.Suppliers
                    .TemporalAsOf(order.Completed.Value)
                    .FirstOrDefaultAsync(x => x.Id == order.SupplierId);

                if (supplier != null)
                {
                    order.Supplier = supplier;
                }
            }

            return order;
        }

        public async Task<PagedList<AdminManageOrder>> GetPagedOrders(
            PageOptions options,
            string search = null,
            string searchTermType = null,
            string framework = null,
            string status = null)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            var baseQuery = dbContext.Orders.Include(x => x.OrderingParty).IgnoreQueryFilters().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                baseQuery = GetSearchTermBySearchType(baseQuery, search, searchTermType);

            if (!string.IsNullOrWhiteSpace(framework))
            {
                baseQuery = baseQuery.Where(
                    o => o.SelectedFrameworkId.Contains(framework));
            }

            var orderList = await baseQuery.ToListAsync();
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse(status, true, out OrderStatus orderStatus)
                    && Enum.IsDefined(typeof(OrderStatus), orderStatus))
                {
                    orderList = orderList.Where(y => y.OrderStatus == orderStatus).ToList();
                }
            }

            options.TotalNumberOfItems = orderList.Count;

            if (options.PageNumber != 0)
                orderList = orderList.Skip((options.PageNumber - 1) * options.PageSize).ToList();

            orderList = orderList.Take(options.PageSize).ToList();

            var results = orderList
                    .Select(o => new AdminManageOrder
                    {
                        CallOffId = o.CallOffId,
                        OrganisationName = o.OrderingParty.Name,
                        Created = o.Created,
                        Status = o.OrderStatus,
                    }).ToList();

            return new PagedList<AdminManageOrder>(
                results,
                options);
        }

        public async Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                throw new ArgumentException(NullOrEmptySearchExceptionMessage, nameof(search));

            var baseQuery = dbContext.Orders.IgnoreQueryFilters().AsNoTracking();

            var parsedSearch = ParseCallOffId(search);

            var orderIdSearch = await baseQuery.Where(o => o.OrderNumber.ToString().Contains(parsedSearch)).Select(o => new SearchFilterModel
            {
                Title = o.CallOffId.ToString(),
                Category = OrderSearchTerms.CallOffId,
            }).Distinct().ToListAsync();

            var organisationSearch = await baseQuery.Where(o => o.OrderingParty.Name.Contains(search)).Select(o => new SearchFilterModel
            {
                Title = o.OrderingParty.Name,
                Category = OrderSearchTerms.Organisation,
            }).Distinct().ToListAsync();

            var supplierSearch = await baseQuery.Where(o => o.Supplier.Name.Contains(search)).Select(o => new SearchFilterModel
            {
                Title = o.Supplier.Name,
                Category = OrderSearchTerms.Supplier,
            }).Distinct().ToListAsync();

            var solutionSearch = await baseQuery
                .SelectMany(o => o.OrderItems)
                .Select(oi => oi.CatalogueItem)
                .Where(o => o.CatalogueItemType == CatalogueItemType.Solution && o.Name.Contains(search))
                .Select(o => new SearchFilterModel
                {
                    Title = o.Name,
                    Category = OrderSearchTerms.Solution,
                }).Distinct().ToListAsync();

            return organisationSearch
                .Concat(supplierSearch)
                .Concat(solutionSearch)
                .Concat(orderIdSearch)
                .Take(15)
                .OrderBy(s => s.Title)
                .ToList();
        }

        public async Task DeleteOrder(CallOffId callOffId, string nameOfRequester, string nameOfApprover, DateTime? dateOfApproval)
        {
            nameOfRequester.ValidateNotNull(nameof(nameOfRequester));
            nameOfApprover.ValidateNotNull(nameof(nameOfApprover));
            dateOfApproval.ValidateNotNull(nameof(dateOfApproval));

            var order = await dbContext.Order(callOffId);

            order.IsDeleted = true;

            order.OrderDeletionApproval = new OrderDeletionApproval
            {
                NameOfRequester = nameOfRequester,
                NameOfApprover = nameOfApprover,
                DateOfApproval = dateOfApproval!.Value,
            };

            await dbContext.SaveChangesAsync();
        }

        private static IQueryable<Order> GetSearchTermBySearchType(IQueryable<Order> baseQuery, string searchTerm, string searchTermType)
        {
            if (!OrderSearchTerms.SearchTermFilters.ContainsKey(searchTermType))
            {
                var parsedCallOffId = ParseCallOffId(searchTerm);
                return baseQuery.Where(o => o.OrderNumber.ToString().Contains(parsedCallOffId)
                    || o.OrderingParty.Name.Contains(searchTerm)
                    || o.Supplier.Name.Contains(searchTerm)
                    || o.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                    && oi.CatalogueItem.Name.Contains(searchTerm)));
            }

            return OrderSearchTerms.SearchTermFilters[searchTermType](baseQuery, searchTerm);
        }

        private static string ParseCallOffId(string search) => search
            .Replace("C0", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-01", string.Empty, StringComparison.OrdinalIgnoreCase);

        private static class OrderSearchTerms
        {
            internal const string CallOffId = "Call-off ID";
            internal const string Organisation = "Organisation";
            internal const string Supplier = "Supplier";
            internal const string Solution = "Solution";

            internal static readonly Dictionary<string, Func<IQueryable<Order>, string, IQueryable<Order>>> SearchTermFilters = new()
            {
                {
                    CallOffId,
                    (baseQuery, searchTerm) =>
                    {
                        var parsedCallOffId = ParseCallOffId(searchTerm);
                        return baseQuery.Where(o => o.OrderNumber.ToString().Contains(parsedCallOffId));
                    }
                },
                {
                    Solution,
                    (baseQuery, searchTerm)
                        => baseQuery.Where(o => o.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                            && oi.CatalogueItem.Name.Contains(searchTerm)))
                },
                { Organisation, (baseQuery, searchTerm) => baseQuery.Where(o => o.OrderingParty.Name.Contains(searchTerm)) },
                { Supplier, (baseQuery, searchTerm) => baseQuery.Where(o => o.Supplier.Name.Contains(searchTerm)) },
            };
        }
    }
}
