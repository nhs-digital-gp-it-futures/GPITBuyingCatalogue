using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class SolutionsFilterService : ISolutionsFilterService
    {
        private const string FrameworkCacheKey = "framework-filter";
        private const string CategoryCacheKey = "category-filter";
        private const string FoundationCapabilitiesKey = "FC";
        private const string AllSolutionsFrameworkKey = "All";
        private const int UndefinedCategory = 0;
        private const int ProductivityCapability = 41;
        private const char CapabilitiesDelimiter = '|';
        private const char CapabilitiesStartingCharacter = 'C';
        private const char EpicStartingCharacter = 'E';

        private readonly MemoryCacheEntryOptions memoryCacheOptions;

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IMemoryCache memoryCache;

        public SolutionsFilterService(
            BuyingCatalogueDbContext dbContext,
            IMemoryCache memoryCache)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            memoryCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(60));
        }

        public async Task<PagedList<CatalogueItem>> GetAllSolutionsFiltered(
            PageOptions options,
            string frameworkId = null,
            string selectedCapabilities = null)
        {
            if (options is null)
                options = new();

            var query = dbContext.CatalogueItems.AsNoTracking()
            .Include(i => i.Solution)
            .Include(i => i.Supplier)
            .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
            .Where(i => i.CatalogueItemType == CatalogueItemType.Solution && i.PublishedStatus == PublicationStatus.Published);

            if (!string.IsNullOrWhiteSpace(frameworkId) && frameworkId != AllSolutionsFrameworkKey)
                query = query.Where(ci => ci.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == frameworkId));

            if (!string.IsNullOrWhiteSpace(selectedCapabilities))
            {
                var capabilitiesPredicate = BuildCapabilitiesPredicate(dbContext, selectedCapabilities);

                query = query.AsExpandableEFCore().Where(capabilitiesPredicate);
            }

            options.TotalNumberOfItems = await query.CountAsync();

            query = options.Sort switch
            {
                PageOptions.SortOptions.LastUpdated => query.OrderByDescending(ci => ci.Solution.LastUpdated),
                _ => query.OrderBy(ci => ci.Name),
            };

            if (options.PageNumber != 0)
                query = query.Skip((options.PageNumber - 1) * options.PageSize);

            query = query.Take(options.PageSize);

            var results = await query.ToListAsync();

            return new PagedList<CatalogueItem>(results, options);
        }

        public async Task<Dictionary<EntityFramework.Catalogue.Models.Framework, int>> GetAllFrameworksAndCountForFilter()
        {
            if (memoryCache.TryGetValue(FrameworkCacheKey, out List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>> value))
                return value.ToDictionary(r => r.Key, r => r.Value);

            var allSolutionsCount = await dbContext.CatalogueItems.AsNoTracking()
                .Where(ci => ci.PublishedStatus == PublicationStatus.Published && ci.CatalogueItemType == CatalogueItemType.Solution)
                .CountAsync();

            var frameworkSolutions = await dbContext.FrameworkSolutions.AsNoTracking()
                .Include(fs => fs.Framework)
                .Where(fs => fs.Solution.CatalogueItem.PublishedStatus == PublicationStatus.Published)
                .OrderBy(fs => fs.Framework.ShortName)
                .ToListAsync();

            var results = frameworkSolutions
                .GroupBy(fs => fs.Framework.Id)
                .Select(fs => new KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>(fs.First().Framework, fs.Count())).ToList();

            results.Insert(
                0,
                new KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>(
                    new EntityFramework.Catalogue.Models.Framework
                    {
                        Id = AllSolutionsFrameworkKey,
                        ShortName = AllSolutionsFrameworkKey,
                    },
                    allSolutionsCount));

            memoryCache.Set(FrameworkCacheKey, results, memoryCacheOptions);

            return results.ToDictionary(r => r.Key, r => r.Value);
        }

        public async Task<CategoryFilterModel> GetAllCategoriesAndCountForFilter(string frameworkId = null)
        {
            if (memoryCache.TryGetValue($"{CategoryCacheKey}-{frameworkId ?? AllSolutionsFrameworkKey}", out CategoryFilterModel value))
                return value;

            var query = dbContext.CatalogueItems.AsNoTracking()
                .Include(ci => ci.CatalogueItemCapabilities)
                .ThenInclude(cic => cic.Capability)
                .ThenInclude(c => c.Category)
                .Include(ci => ci.CatalogueItemEpics)
                .ThenInclude(cie => cie.Epic)
                .Where(ci => ci.PublishedStatus == PublicationStatus.Published && ci.CatalogueItemType == CatalogueItemType.Solution);

            if (!string.IsNullOrWhiteSpace(frameworkId) && frameworkId != AllSolutionsFrameworkKey)
                query = query.Where(ci => ci.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == frameworkId));

            var results = await query.ToListAsync();

            var categories = results.SelectMany(ci => ci.CatalogueItemCapabilities)
                .Select(cic => cic.Capability.Category)
                .DistinctBy(c => c.Id)
                .Where(c => c.Id != UndefinedCategory)
                .Select(c =>
                new CapabilityCategoryFilter
                {
                    Name = c.Name,
                    Description = c.Description,
                    CategoryId = c.Id,
                }).ToList();

            var capabilites = results.SelectMany(ci => ci.CatalogueItemCapabilities)
                .Select(cic => cic.Capability)
                .DistinctBy(c => c.Id)
                .Where(c => c.Id != ProductivityCapability)
                .ToList();

            var foundationCapabilities = await dbContext.FrameworkCapabilities.AsNoTracking()
                .Include(fc => fc.Capability)
                .Where(fc => fc.IsFoundation)
                .OrderBy(fc => fc.Capability.Name)
                .ToListAsync();

            var countOfCatalogueItemsWithFoundationCapability =
                results
                .Where(ci => ci.CatalogueItemCapabilities
                .Where(cic => foundationCapabilities
                            .Any(fc => fc.CapabilityId == cic.CapabilityId))
                            .Count() == foundationCapabilities.Count())
                .Count();

            var foundationCapabilitiesFilter =
                foundationCapabilities
                .Select(c => new CapabilitiesFilter
                {
                    CapabilityId = c.CapabilityId,
                    Name = c.Capability.Name,
                    CapabilityRef = c.Capability.CapabilityRef,
                })
                .ToList();

            var epics = results.SelectMany(ci => ci.CatalogueItemEpics)
                .Select(cie => cie.Epic)
                .DistinctBy(e => e.Id)
                .ToList();

            // for each category, add to its list of capabilities the capabilities that reference that category, and count how many
            // catalogue items reference that capabilitiy
            categories.ForEach(c =>
            c.Capabilities.AddRange(
                capabilites.Where(cap => cap.CategoryId == c.CategoryId)
                           .Select(cap =>
                            new CapabilitiesFilter
                            {
                                CapabilityId = cap.Id,
                                Name = cap.Name,
                                CapabilityRef = cap.CapabilityRef,
                                Count = results.Where(ci => ci.CatalogueItemCapabilities.Any(cic => cic.CapabilityId == cap.Id)).Count(),
                            })));

            // for each category, then each capability in that cateogry, add to its list of epics the epics that reference that capability, and count how many
            // catalogue items reference that epic
            categories.ForEach(
                c => c.Capabilities.ForEach(
                    cap => cap.Epics.AddRange(
                        epics.Where(e => e.CapabilityId == cap.CapabilityId)
                             .Select(e =>
                             new EpicsFilter
                             {
                                 Id = e.Id,
                                 Name = e.Name,
                                 Count = results.Where(ci => ci.CatalogueItemEpics.Any(cie => cie.EpicId == e.Id)).Count(),
                             }))));

            var response = new CategoryFilterModel
            {
                CategoryFilters = categories,
                FoundationCapabilities = foundationCapabilitiesFilter,
                CountOfCatalogueItemsWithFoundationCapabilities = countOfCatalogueItemsWithFoundationCapability,
            };

            memoryCache.Set($"{CategoryCacheKey}-{frameworkId ?? AllSolutionsFrameworkKey}", response, memoryCacheOptions);

            return response;
        }

        private static Dictionary<string, List<string>> DecodeCapabilitiesFilter(string capabilities)
        {
            var output = new Dictionary<string, List<string>>();

            var splitCapabilities = capabilities.Split(CapabilitiesDelimiter);

            foreach (var capability in splitCapabilities)
            {
                if (capability == FoundationCapabilitiesKey || !capability.Contains(EpicStartingCharacter))
                {
                    output.Add(capability, new List<string>());
                }
                else
                {
                    var list = new List<string>();

                    var epics = capability.Split(EpicStartingCharacter);

                    list.AddRange(epics.Where(e => !e.StartsWith(CapabilitiesStartingCharacter)).Select(epic => $"{epics[0]}{EpicStartingCharacter}{epic}"));
                    output.Add(epics[0], list);
                }
            }

            return output;
        }

        /// <summary>
        /// loop through the capbilities and epics and generates an EF where clause from the list.
        /// </summary>
        /// <param name="dbContext">the dbContext.</param>
        /// <param name="selectedCapabilities">the pipe-deliminated string of selected capabilities and epics.</param>
        /// <returns>an Expression Starter Containing the Where Clause for the EF Query.</returns>
        private static ExpressionStarter<CatalogueItem> BuildCapabilitiesPredicate(BuyingCatalogueDbContext dbContext, string selectedCapabilities)
        {
            var capabilities = DecodeCapabilitiesFilter(selectedCapabilities);

            var predicateBuilder = PredicateBuilder.New<CatalogueItem>();

            var foundationSolutions = dbContext.FrameworkCapabilities.Where(fc => fc.IsFoundation);

            foreach (var capability in capabilities)
            {
                var capabilityPredicateBuilder = PredicateBuilder.New<CatalogueItem>();

                capabilityPredicateBuilder = capability.Key == FoundationCapabilitiesKey
                ? capabilityPredicateBuilder
                    .Or(ci => ci.CatalogueItemCapabilities.Where(cic => cic.Capability.FrameworkCapabilities.Any(fc => fc.IsFoundation)).Count()
                        == foundationSolutions.Count())
                : capabilityPredicateBuilder
                .Or(ci => ci.CatalogueItemCapabilities.Any(cic => cic.Capability.CapabilityRef == capability.Key));

                if (capability.Value.Any())
                {
                    var epicPredicateBuilder = BuildEpicsPredicate(capability.Value);

                    capabilityPredicateBuilder = capabilityPredicateBuilder.Extend(epicPredicateBuilder, PredicateOperator.And);
                }

                predicateBuilder = predicateBuilder.Extend(capabilityPredicateBuilder);
            }

            return predicateBuilder;
        }

        private static ExpressionStarter<CatalogueItem> BuildEpicsPredicate(List<string> selectedEpics)
        {
            var epicPredicateBuilder = PredicateBuilder.New<CatalogueItem>();

            foreach (var epic in selectedEpics)
            {
                epicPredicateBuilder = epicPredicateBuilder.And(ci => ci.CatalogueItemEpics.Any(cie => cie.EpicId == epic));
            }

            return epicPredicateBuilder;
        }
    }
}
