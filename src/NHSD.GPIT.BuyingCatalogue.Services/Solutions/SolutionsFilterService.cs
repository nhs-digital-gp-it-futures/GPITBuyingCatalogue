using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    [ExcludeFromCodeCoverage]
    public sealed class SolutionsFilterService : ISolutionsFilterService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public SolutionsFilterService(BuyingCatalogueDbContext dbContext) =>
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<(IQueryable<CatalogueItem> CatalogueItems, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetFilteredAndNonFilteredQueryResults(
            Dictionary<int, string[]> capabiltiesAndEpics)
        {
            var (query, count) = (capabiltiesAndEpics?.Count ?? 0) == 0
                ? NonFilteredQuery(dbContext)
                : await FilteredQuery(dbContext, capabiltiesAndEpics);

            return (query, count);
        }

        public async Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetAllSolutionsFiltered(
            PageOptions options,
            Dictionary<int, string[]> capabilitiesAndEpics = null,
            string search = null,
            string selectedFrameworkId = null,
            string selectedApplicationTypeIds = null,
            string selectedHostingTypeIds = null,
            string selectedIM1Integrations = null,
            string selectedGPConnectIntegrations = null)
        {
            var (query, count) = await GetFilteredAndNonFilteredQueryResults(capabilitiesAndEpics);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(ci => ci.Supplier.Name.Contains(search) || ci.Name.Contains(search));

            if (!string.IsNullOrWhiteSpace(selectedFrameworkId))
                query = query.Where(ci => ci.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == selectedFrameworkId));
            if (!string.IsNullOrWhiteSpace(selectedApplicationTypeIds))
            {
                query = ApplyAdditionalFilterToQuery<ApplicationType>(
                    query,
                    selectedApplicationTypeIds,
                    GetSelectedFilterApplication,
                    x => x.ApplicationTypeDetail != null);
            }

            if (!string.IsNullOrWhiteSpace(selectedHostingTypeIds))
            {
                query = ApplyAdditionalFilterToQuery<HostingType>(
                    query,
                    selectedHostingTypeIds,
                    GetSelectedFiltersHosting,
                    x => x.Hosting.IsValid());
            }

            if (!string.IsNullOrWhiteSpace(selectedIM1Integrations))
            {
                string[] integrationParts = selectedIM1Integrations.Split('.');
                foreach (string s in integrationParts)
                {
                    query = query.Where(ci => ci.Solution.Integrations.Contains(s));
                }
            }

            if (!string.IsNullOrWhiteSpace(selectedGPConnectIntegrations))
            {
                string[] gpIntegrationParts = selectedGPConnectIntegrations.Split('.');
                foreach (string s in gpIntegrationParts)
                {
                    query = query.Where(ci => ci.Solution.Integrations.Contains(s));
                }
            }

            var totalNumberOfItems = await query.CountAsync();
            query = (options?.Sort ?? PageOptions.SortOptions.AtoZ) switch
            {
                PageOptions.SortOptions.LastPublished => query.OrderByDescending(ci => ci.LastPublished)
                                                                .ThenBy(ci => ci.Name),
                PageOptions.SortOptions.ZToA => query.OrderByDescending(ci => ci.Name),
                PageOptions.SortOptions.AtoZ or _ => query.OrderBy(ci => ci.Name),
            };

            if (options != null)
            {
                if (options.PageNumber != 0)
                    query = query.Skip((options.PageNumber - 1) * options.PageSize);

                query = query.Take(options.PageSize);
            }
            else
            {
                options = new PageOptions("1", totalNumberOfItems);
            }

            options.TotalNumberOfItems = totalNumberOfItems;
            var results = await query.ToListAsync();

            return (results, options, count);
        }

        public async Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15)
        {
            var searchBySolutionNameQuery = dbContext.CatalogueItems.AsNoTracking()
                .Where(ci =>
                    ci.Name.Contains(searchTerm)
                    && ci.CatalogueItemType == CatalogueItemType.Solution
                    && (ci.PublishedStatus == PublicationStatus.Published || ci.PublishedStatus == PublicationStatus.InRemediation)
                    && ci.Supplier.IsActive)
                .Select(ci => new SearchFilterModel
                {
                    Title = ci.Name,
                    Category = "Solution",
                });

            var searchBySupplierNameQuery = dbContext.Suppliers.AsNoTracking()
                .Where(s => s.Name.Contains(searchTerm) && s.IsActive)
                .Select(s => new SearchFilterModel
                {
                    Title = s.Name,
                    Category = "Supplier",
                });

            return await searchBySolutionNameQuery
                .Union(searchBySupplierNameQuery)
                .OrderBy(ssfm => ssfm.Title)
                .Take(maxToBringBack)
                .ToListAsync();
        }

        private static IQueryable<CatalogueItem> ApplyAdditionalFilterToQuery<T>(
            IQueryable<CatalogueItem> query,
            string selectedFilterIds,
            Func<Solution, IEnumerable<T>, IEnumerable<T>> getSelectedFilters,
            Predicate<Solution> isValid)
            where T : struct, Enum
        {
            if (string.IsNullOrEmpty(selectedFilterIds))
                return query;

            var selectedFilterEnums = selectedFilterIds.Split(FilterConstants.Delimiter)
                .Where(t => Enum.TryParse<T>(t, out var enumVal) && Enum.IsDefined(enumVal))
                .Select(Enum.Parse<T>)
                .ToList();

            if (selectedFilterEnums == null || !selectedFilterEnums.Any())
                throw new ArgumentException("Invalid filter format", nameof(selectedFilterIds));

            foreach (var row in query)
            {
                if (!isValid(row.Solution))
                {
                    query = query.Where(ci => ci.Id != row.Id);
                    continue;
                }

                var matchingTypes = getSelectedFilters(row.Solution, selectedFilterEnums);

                if (matchingTypes.Count() < selectedFilterEnums?.Count)
                {
                    query = query.Where(ci => ci.Id != row.Id);
                }
            }

            return query;
        }

        private static (IQueryable<CatalogueItem> Query, List<CapabilitiesAndCountModel> Count) NonFilteredQuery(BuyingCatalogueDbContext dbContext) =>
            (dbContext.CatalogueItems
                .AsNoTracking()
                .AsSplitQuery()
                .Include(i => i.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(s => s.Framework)
                .Include(i => i.Supplier)
                .Include(i => i.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Where(i =>
                i.CatalogueItemType == CatalogueItemType.Solution
                && (i.PublishedStatus == PublicationStatus.Published || i.PublishedStatus == PublicationStatus.InRemediation)
                && i.Supplier.IsActive), new List<CapabilitiesAndCountModel>());

        private static async Task<(IQueryable<CatalogueItem> Query, List<CapabilitiesAndCountModel> Count)> FilteredQuery(
            BuyingCatalogueDbContext dbContext,
            Dictionary<int, string[]> capabilitiesAndEpics)
        {
            capabilitiesAndEpics.Where(kv => kv.Value == null)
                .ForEach(kv => capabilitiesAndEpics[kv.Key] = Array.Empty<string>());

            var capabilitiesAndCount = await dbContext.Capabilities
                .AsNoTracking()
                .Where(c => capabilitiesAndEpics.Keys.Contains(c.Id))
                .Select(c => new CapabilitiesAndCountModel()
                {
                    CapabilityId = c.Id,
                    CapabilityName = c.Name,
                    CountOfEpics = capabilitiesAndEpics.GetValueOrDefault(c.Id, Array.Empty<string>()).Length,
                })
                .ToListAsync();

            var capabilityPredicate = CapabilitiesPredicate(capabilitiesAndEpics.Where(kv => (kv.Value?.Length ?? 0) == 0));
            var itemPredicate = PredicateBuilder.New<CatalogueItem>()
                .Start(p => p.CatalogueItemCapabilities.Any(cic => capabilityPredicate.Invoke(cic)));
            capabilitiesAndEpics
                .Where(kv => (kv.Value?.Length ?? 0) > 0)
                .ForEach(kv =>
                {
                    var epicsPredicate = EpicsPredicate(kv.Key, kv.Value);
                    itemPredicate = itemPredicate.Or(epicsPredicate);
                });

            return (dbContext.CatalogueItems
                .AsNoTracking()
                .AsExpandable()
                .AsSplitQuery()
                .Include(i => i.Supplier)
                .Include(i => i.Solution)
                    .ThenInclude(s => s.AdditionalServices
                        .Where(adit => itemPredicate.Invoke(adit.CatalogueItem)))
                    .ThenInclude(adit => adit.CatalogueItem)
                    .ThenInclude(ci => ci.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Include(i => i.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(fs => fs.Framework)
                .Include(i => i.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Where(i =>
                    i.CatalogueItemType == CatalogueItemType.Solution
                    && itemPredicate.Invoke(i)
                    && i.Solution.FrameworkSolutions.Select(f => f.Framework).Distinct().Any(f => !f.IsExpired)),
                capabilitiesAndCount);
        }

        private static Expression<Func<CatalogueItem, bool>> EpicsPredicate(int capabilityId, string[] epics)
        {
            var epicsPredicate = PredicateBuilder.New<CatalogueItem>();

            epics.ForEach(id =>
            {
                epicsPredicate = epicsPredicate.And(i => i.CatalogueItemEpics.Any(e => e.CapabilityId == capabilityId && e.EpicId == id));
            });

            return epicsPredicate;
        }

        private static Expression<Func<CatalogueItemCapability, bool>> CapabilitiesPredicate(IEnumerable<KeyValuePair<int, string[]>> capabiltiesAndEpics)
        {
            var capabilityPredicate = PredicateBuilder.New<CatalogueItemCapability>();
            capabiltiesAndEpics
                .ForEach(kv => capabilityPredicate = capabilityPredicate.Or(c => c.CapabilityId == kv.Key));
            return capabilityPredicate;
        }

        private static IEnumerable<ApplicationType> GetSelectedFilterApplication(Solution solution, IEnumerable<ApplicationType> selectedFilterEnums)
        {
            var applicationTypeDetail = solution.ApplicationTypeDetail;
            return selectedFilterEnums?.Where(t => applicationTypeDetail.HasApplicationType(t));
        }

        private static IEnumerable<HostingType> GetSelectedFiltersHosting(Solution solution, IEnumerable<HostingType> selectedFilterEnums)
        {
            return selectedFilterEnums?.Where(t => solution.Hosting.HasHostingType(t));
        }
    }
}
