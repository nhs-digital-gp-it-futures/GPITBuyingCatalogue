using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EnumsNET;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
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
                string selectedIm1Integrations = null,
                string selectedGpConnectIntegrations = null,
                string selectedInteroperabilityOptions = null)
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
                    x => x.ApplicationTypeDetail != null,
                    false);
            }

            if (!string.IsNullOrWhiteSpace(selectedHostingTypeIds))
            {
                query = ApplyAdditionalFilterToQuery<HostingType>(
                    query,
                    selectedHostingTypeIds,
                    GetSelectedFiltersHosting,
                    x => x.Hosting != null && x.Hosting.IsValid(),
                    false);
            }

            if (!string.IsNullOrWhiteSpace(selectedIm1Integrations))
            {
                query = ApplyAdditionalFilterToQuery<InteropIm1IntegrationType>(
                    query,
                    selectedIm1Integrations,
                    GetSelectedFiltersIm1Integration,
                    x => x.Integrations != null,
                    false);
            }
            else if (!string.IsNullOrWhiteSpace(selectedInteroperabilityOptions)
                     && selectedInteroperabilityOptions.Contains(
                         ((int)InteropIntegrationType.Im1).ToString(CultureInfo.InvariantCulture),
                         StringComparison.OrdinalIgnoreCase))
            {
                InteropIm1IntegrationType[] enumValues =
                    (InteropIm1IntegrationType[])Enum.GetValues(typeof(InteropIm1IntegrationType));

                string im1Integrations = enumValues.Select(e => (int)e).ToFilterString();
                query = ApplyAdditionalFilterToQuery<InteropIm1IntegrationType>(
                    query,
                    im1Integrations,
                    GetSelectedFiltersIm1Integration,
                    x => x.Integrations != null,
                    true);
            }

            if (!string.IsNullOrWhiteSpace(selectedGpConnectIntegrations))
            {
                query = ApplyAdditionalFilterToQuery<InteropGpConnectIntegrationType>(
                    query,
                    selectedGpConnectIntegrations,
                    GetSelectedFiltersGpConnectIntegration,
                    x => x.Integrations != null,
                    false);
            }
            else if (!string.IsNullOrWhiteSpace(selectedInteroperabilityOptions)
                     && selectedInteroperabilityOptions.Contains(
                         ((int)InteropIntegrationType.GpConnect).ToString(CultureInfo.InvariantCulture),
                         StringComparison.OrdinalIgnoreCase))
            {
                InteropGpConnectIntegrationType[] enumValues =
                    (InteropGpConnectIntegrationType[])Enum.GetValues(typeof(InteropIm1IntegrationType));

                string gpConnectIntegrations = enumValues.Select(e => (int)e).ToFilterString();
                query = ApplyAdditionalFilterToQuery<InteropGpConnectIntegrationType>(
                    query,
                    gpConnectIntegrations,
                    GetSelectedFiltersGpConnectIntegration,
                    x => x.Integrations != null,
                    true);
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

        public async
            Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel>
                CapabilitiesAndCount)> GetAllSolutionsFilteredFromFilterIds(
                FilterIdsModel filterIds)
        {
                return await GetAllSolutionsFiltered(
                null,
                capabilitiesAndEpics: filterIds?.CapabilityAndEpicIds,
                selectedFrameworkId: filterIds?.FrameworkId,
                selectedApplicationTypeIds: filterIds.ApplicationTypeIds.ToFilterString(),
                selectedHostingTypeIds: filterIds.HostingTypeIds.ToFilterString(),
                selectedIm1Integrations: filterIds.IM1Integrations.ToFilterString(),
                selectedGpConnectIntegrations: filterIds.GPConnectIntegrations.ToFilterString(),
                selectedInteroperabilityOptions: filterIds.InteroperabilityOptions.ToFilterString());
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
            Predicate<Solution> isValid,
            bool isInteropFilter)
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

                if (isInteropFilter)
                {
                    bool shouldKeepRow = matchingTypes.Any(mt => selectedFilterEnums.Contains(mt));

                    if (!shouldKeepRow)
                    {
                        query = query.Where(ci => ci.Id != row.Id);
                    }
                }
                else
                {
                    query = query.Where(ci => ci.Id != row.Id);
                }
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
                            .Where(adit => AllowedPublicationStatuses.Contains(adit.CatalogueItem.PublishedStatus) || itemPredicate.Invoke(adit.CatalogueItem)))
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
                            && itemPredicate.Invoke(i)
                            && i.Solution.FrameworkSolutions.Select(f => f.Framework)
                                .Distinct()
                                .Any(f => !f.IsExpired)),
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

        private static IEnumerable<InteropIm1IntegrationType> GetSelectedFiltersIm1Integration(
            Solution solution,
            IEnumerable<InteropIm1IntegrationType> selectedFilterEnums)
        {
            return selectedFilterEnums?.Where(
                t => solution.Integrations.Contains(t.GetName().Replace('_', ' '), StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<InteropGpConnectIntegrationType> GetSelectedFiltersGpConnectIntegration(
            Solution solution,
            IEnumerable<InteropGpConnectIntegrationType> selectedFilterEnums)
        {
            return selectedFilterEnums?.Where(
                t => solution.Integrations.Contains(t.GetName().Replace('_', ' '), StringComparison.OrdinalIgnoreCase));
        }
    }
}
