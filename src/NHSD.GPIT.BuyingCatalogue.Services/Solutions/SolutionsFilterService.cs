using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    [ExcludeFromCodeCoverage]
    public sealed class SolutionsFilterService : ISolutionsFilterService
    {
        private static readonly PublicationStatus[] AllowedPublicationStatuses = { PublicationStatus.Published, PublicationStatus.InRemediation };
        private readonly BuyingCatalogueDbContext dbContext;

        public SolutionsFilterService(BuyingCatalogueDbContext dbContext) =>
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async
            Task<(IQueryable<CatalogueItem> CatalogueItems, List<CapabilitiesAndCountModel> CapabilitiesAndCount)>
            GetFilteredAndNonFilteredQueryResults(
                Dictionary<int, string[]> capabilitiesAndEpics)
        {
            (IQueryable<CatalogueItem> query, List<CapabilitiesAndCountModel> count) = (capabilitiesAndEpics?.Count ?? 0) == 0
                ? NonFilteredQuery(dbContext)
                : await FilteredQuery(dbContext, capabilitiesAndEpics);

            return (query, count);
        }

        public async
            Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel>
                CapabilitiesAndCount)> GetAllSolutionsFiltered(
                PageOptions options,
                Dictionary<int, string[]> capabilitiesAndEpics = null,
                string search = null,
                string selectedFrameworkId = null,
                string selectedApplicationTypeIds = null,
                string selectedHostingTypeIds = null,
                Dictionary<SupportedIntegrations, int[]> selectedIntegrationsAndTypes = null)
        {
            (IQueryable<CatalogueItem> query, List<CapabilitiesAndCountModel> count) = await GetFilteredAndNonFilteredQueryResults(capabilitiesAndEpics);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(ci => ci.Supplier.Name.Contains(search) || ci.Name.Contains(search));

            if (!string.IsNullOrWhiteSpace(selectedFrameworkId))
            {
                query = query.Where(
                    ci => ci.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == selectedFrameworkId));
            }

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
                    x => x.Hosting != null && x.Hosting.IsValid());
            }

            if (selectedIntegrationsAndTypes is { Count: > 0 })
            {
                var integrationsPredicate = IntegrationsPredicate(selectedIntegrationsAndTypes);

                query = query.AsExpandable().Where(x => integrationsPredicate.Invoke(x.Solution));
            }

            var totalNumberOfItems = await query.CountAsync();
            query = (options?.Sort ?? PageOptions.SortOptions.AtoZ) switch
            {
                PageOptions.SortOptions.LastPublished => query.OrderByDescending(ci => ci.LastPublished)
                    .ThenBy(ci => ci.Name),
                PageOptions.SortOptions.ZToA => query.OrderByDescending(ci => ci.Name),
                _ => query.OrderBy(ci => ci.Name),
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

        public async
            Task<IList<CatalogueItem>> GetAllSolutionsFilteredFromFilterIds(
                FilterIdsModel filterIds)
        {
            var (catalogueItems, _, _) = await GetAllSolutionsFiltered(
                null,
                capabilitiesAndEpics: filterIds?.CapabilityAndEpicIds,
                selectedFrameworkId: filterIds?.FrameworkId,
                selectedApplicationTypeIds: filterIds.ApplicationTypeIds.ToFilterString(),
                selectedHostingTypeIds: filterIds.HostingTypeIds.ToFilterString(),
                selectedIntegrationsAndTypes: filterIds.IntegrationsIds);
            return catalogueItems;
        }

        public async Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15)
        {
            var searchBySolutionNameQuery = dbContext.CatalogueItems.AsNoTracking()
                .Where(
                    ci =>
                        ci.Name.Contains(searchTerm)
                        && ci.CatalogueItemType == CatalogueItemType.Solution
                        && AllowedPublicationStatuses.Contains(ci.PublishedStatus)
                        && ci.Supplier.IsActive)
                .Select(ci => new SearchFilterModel { Title = ci.Name, Category = "Solution", });

            var searchBySupplierNameQuery = dbContext.Suppliers.AsNoTracking()
                .Where(s => s.Name.Contains(searchTerm) && s.IsActive)
                .Select(s => new SearchFilterModel { Title = s.Name, Category = "Supplier", });

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

            var selectedFilterEnums = SolutionsFilterHelper.ParseSelectedFilterIds<T>(selectedFilterIds);

            foreach (var row in query)
            {
                if (!isValid(row.Solution))
                {
                    query = query.Where(ci => ci.Id != row.Id);
                    continue;
                }

                var matchingTypes = getSelectedFilters(row.Solution, selectedFilterEnums).ToList();

                if (!(matchingTypes.Count < selectedFilterEnums?.Count)) continue;
                query = query.Where(ci => ci.Id != row.Id);
            }

            return query;
        }

        private static (IQueryable<CatalogueItem> Query, List<CapabilitiesAndCountModel> Count) NonFilteredQuery(
            BuyingCatalogueDbContext dbContext) =>
            (dbContext.CatalogueItems
                .AsNoTracking()
                .AsSplitQuery()
                .Include(i => i.Solution)
                .ThenInclude(s => s.FrameworkSolutions)
                .ThenInclude(s => s.Framework)
                .Include(i => i.Supplier)
                .Include(i => i.CatalogueItemCapabilities)
                .ThenInclude(cic => cic.Capability)
                .Where(
                    i =>
                        i.CatalogueItemType == CatalogueItemType.Solution
                        && AllowedPublicationStatuses.Contains(i.PublishedStatus)
                        && i.Supplier.IsActive), new List<CapabilitiesAndCountModel>());

        private static async Task<(IQueryable<CatalogueItem> Query, List<CapabilitiesAndCountModel> Count)>
            FilteredQuery(
                BuyingCatalogueDbContext dbContext,
                Dictionary<int, string[]> capabilitiesAndEpics)
        {
            capabilitiesAndEpics.Where(kv => kv.Value == null)
                .ForEach(kv => capabilitiesAndEpics[kv.Key] = Array.Empty<string>());

            var capabilitiesAndCount = await dbContext.Capabilities
                .AsNoTracking()
                .Where(c => capabilitiesAndEpics.Keys.Contains(c.Id))
                .Select(
                    c => new CapabilitiesAndCountModel()
                    {
                        CapabilityId = c.Id,
                        CapabilityName = c.Name,
                        CountOfEpics = capabilitiesAndEpics.GetValueOrDefault(c.Id, Array.Empty<string>()).Length,
                    })
                .ToListAsync();

            var itemPredicate = CapabilitiesAndEpicsPredicate(
                capabilitiesAndEpics.Where(kv => (kv.Value?.Length ?? 0) == 0).Select(kv => kv.Key),
                capabilitiesAndEpics.Where(kv => (kv.Value?.Length ?? 0) > 0));

            return (dbContext.CatalogueItems
                    .AsNoTracking()
                    .AsExpandable()
                    .AsSplitQuery()
                    .Include(i => i.Supplier)
                    .Include(i => i.Solution)
                    .ThenInclude(
                        s => s.AdditionalServices
                            .Where(adit => AllowedPublicationStatuses.Contains(adit.CatalogueItem.PublishedStatus) && itemPredicate.Invoke(adit.CatalogueItem)))
                    .ThenInclude(adit => adit.CatalogueItem)
                    .ThenInclude(ci => ci.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                    .Include(i => i.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(fs => fs.Framework)
                    .Include(i => i.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                    .Where(
                        i =>
                            i.CatalogueItemType == CatalogueItemType.Solution
                            && AllowedPublicationStatuses.Contains(i.PublishedStatus)
                            && i.Supplier.IsActive
                            && (itemPredicate.Invoke(i) || i.Solution.AdditionalServices.Any(y => itemPredicate.Invoke(y.CatalogueItem)))),
                capabilitiesAndCount);
        }

        private static Expression<Func<CatalogueItem, bool>> CapabilitiesAndEpicsPredicate(
            IEnumerable<int> capabilitiesOnly,
            IEnumerable<KeyValuePair<int, string[]>> capabilitiesAndEpics)
        {
            var itemPredicate = PredicateBuilder.New<CatalogueItem>(true);
            if (capabilitiesOnly.Any())
            {
                var capabilityPredicate = PredicateBuilder.New<CatalogueItem>();
                capabilitiesOnly
                    .ForEach(k => capabilityPredicate = capabilityPredicate.And(c => c.CatalogueItemCapabilities.Any(cic => cic.CapabilityId == k)));

                itemPredicate = itemPredicate.And(capabilityPredicate);
            }

            capabilitiesAndEpics
                .ForEach(
                    kv =>
                    {
                        var epicsPredicate = EpicsPredicate(kv.Key, kv.Value);
                        itemPredicate = itemPredicate.And(epicsPredicate);
                    });

            return itemPredicate;
        }

        private static Expression<Func<CatalogueItem, bool>> EpicsPredicate(int capabilityId, string[] epics)
        {
            var epicsPredicate = PredicateBuilder.New<CatalogueItem>();

            epics.ForEach(
                id =>
                {
                    epicsPredicate = epicsPredicate.And(
                        i => i.CatalogueItemEpics.Any(e => e.CapabilityId == capabilityId && e.EpicId == id));
                });

            return epicsPredicate;
        }

        private static Expression<Func<Solution, bool>> IntegrationsPredicate(
            IDictionary<SupportedIntegrations, int[]> selectedIntegrations)
        {
            var predicate = PredicateBuilder.New<Solution>();

            foreach (var integration in selectedIntegrations)
            {
                var integrationPredicate = PredicateBuilder.New<Solution>();
                if (integration.Value.Length == 0)
                {
                    integrationPredicate = integrationPredicate.And(x => x.Integrations.Any(y => y.IntegrationType.IntegrationId == integration.Key));
                }
                else
                {
                    integration.Value.ForEach(x =>
                    {
                        integrationPredicate =
                            integrationPredicate.And(y => y.Integrations.Any(z => z.IntegrationTypeId == x));
                    });
                }

                predicate.And(integrationPredicate);
            }

            return predicate;
        }

        private static IEnumerable<ApplicationType> GetSelectedFilterApplication(
            Solution solution,
            IEnumerable<ApplicationType> selectedFilterEnums)
        {
            var applicationTypeDetail = solution.ApplicationTypeDetail;
            return selectedFilterEnums?.Where(t => applicationTypeDetail.HasApplicationType(t));
        }

        private static IEnumerable<HostingType> GetSelectedFiltersHosting(
            Solution solution,
            IEnumerable<HostingType> selectedFilterEnums)
        {
            return selectedFilterEnums?.Where(t => solution.Hosting.HasHostingType(t));
        }
    }
}
