using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

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
            var order = await dbContext
                .Orders
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.CatalogueItem.Solution.FrameworkSolutions)
                .ThenInclude(fs => fs.Framework)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Id == callOffId.Id);

            return order;
        }

        public async Task<PagedList<AdminManageOrder>> GetPagedOrders(
            PageOptions options,
            string search = null)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            var baseQuery = dbContext.Orders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var parsedSearch = ParseCallOffId(search);

                baseQuery = baseQuery.Where(o => o.Id.ToString().Contains(parsedSearch)
                    || o.OrderingParty.Name.Contains(search)
                    || o.Supplier.Name.Contains(search)
                    || o.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                    && oi.CatalogueItem.Name.Contains(search)));
            }

            options.TotalNumberOfItems = await baseQuery.CountAsync();

            if (options.PageNumber != 0)
                baseQuery = baseQuery.Skip((options.PageNumber - 1) * options.PageSize);

            baseQuery = baseQuery.Take(options.PageSize);

            var results = await baseQuery
                    .Select(o => new AdminManageOrder
                    {
                        CallOffId = o.CallOffId,
                        OrganisationName = o.OrderingParty.Name,
                        Created = o.Created,
                        Status = o.OrderStatus,
                    }).ToListAsync();

            return new PagedList<AdminManageOrder>(
                results,
                options);
        }

        public async Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                throw new ArgumentException(NullOrEmptySearchExceptionMessage, nameof(search));

            var baseQuery = dbContext.Orders.AsNoTracking();

            var parsedSearch = ParseCallOffId(search);

            var orderIdSearch = baseQuery.Where(o => o.Id.ToString().Contains(parsedSearch)).Select(o => new SearchFilterModel
            {
                Title = "Call-off ID",
                Category = o.CallOffId.ToString(),
            });

            var organisationSearch = await baseQuery.Where(o => o.OrderingParty.Name.Contains(search)).Select(o => new SearchFilterModel
            {
                Title = "Organisation",
                Category = o.CallOffId.ToString(),
            }).ToListAsync();

            var supplierSearch = await baseQuery.Where(o => o.Supplier.Name.Contains(search)).Select(o => new SearchFilterModel
            {
                Title = "Supplier",
                Category = o.CallOffId.ToString(),
            }).ToListAsync();

            var solutionSearch = await baseQuery
                .Where(o => o.OrderItems.Any(oi =>
                    oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                    && oi.CatalogueItem.Name.Contains(search)))
                .Select(o => new SearchFilterModel
                {
                    Title = "Solution",
                    Category = o.CallOffId.ToString(),
                }).ToListAsync();

            return organisationSearch
                .Concat(supplierSearch)
                .Concat(solutionSearch)
                .Concat(orderIdSearch)
                .Take(15)
                .ToList();
        }

        private static string ParseCallOffId(string search) => search
                    .Replace("C0", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("-01", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
