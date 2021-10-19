using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class SolutionsFilterService : ISolutionsFilterService
    {
        private const string FoundationCapabilitiesKey = "FC";
        private const string AllSolutionsFrameworkKey = "All";
        private const int UndefinedCategory = 0;
        private const int ProductivityCapability = 41;
        private const char CapabilitiesDelimiter = '|';
        private const char CapabilitiesStartingCharacter = 'C';
        private const char EpicStartingCharacter = 'E';
        private const char SupplierStartingCharacter = 'S';
        private const char SupplierMarkCharacter = 'X';
        private const char DfocvcMarkCharacter = 'D';

        private readonly BuyingCatalogueDbContext dbContext;

        public SolutionsFilterService(BuyingCatalogueDbContext dbContext) =>
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<PagedList<CatalogueItem>> GetAllSolutionsFiltered(
            PageOptions options,
            string frameworkId = null,
            string selectedCapabilities = null)
        {
            options ??= new PageOptions();

            var query = dbContext.CatalogueItems.AsNoTracking()
                .Include(i => i.Solution)
                .Include(i => i.Supplier)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Where(i =>
                i.CatalogueItemType == CatalogueItemType.Solution
                && (i.PublishedStatus != PublicationStatus.Draft
                    && i.PublishedStatus != PublicationStatus.Unpublished)
                && i.Supplier.IsActive);

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

            return results.ToDictionary(r => r.Key, r => r.Value);
        }

        public async Task<CategoryFilterModel> GetAllCategoriesAndCountForFilter(string frameworkId = null)
        {
            var query = dbContext.CatalogueItems.AsNoTracking()
                .Include(ci => ci.CatalogueItemCapabilities)
                .ThenInclude(cic => cic.Capability)
                .ThenInclude(c => c.Category)
                .Include(ci => ci.CatalogueItemEpics)
                .ThenInclude(cie => cie.Epic)
                .Where(ci =>
                ci.PublishedStatus == PublicationStatus.Published
                && ci.CatalogueItemType == CatalogueItemType.Solution
                && ci.Supplier.IsActive);

            if (!string.IsNullOrWhiteSpace(frameworkId) && frameworkId != AllSolutionsFrameworkKey)
                query = query.Where(ci => ci.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == frameworkId));

            var results = await query.ToListAsync();

            var categories = results.SelectMany(ci => ci.CatalogueItemCapabilities)
                .Select(cic => cic.Capability.Category)
                .DistinctBy(c => c.Id)
                .Where(c => c.Id != UndefinedCategory)
                .Select(c => new CapabilityCategoryFilter
                {
                    Name = c.Name,
                    Description = c.Description,
                    CategoryId = c.Id,
                }).ToList();

            var capabilities = results.SelectMany(ci => ci.CatalogueItemCapabilities)
                .Select(cic => cic.Capability)
                .DistinctBy(c => c.Id)
                .Where(c => c.Id != ProductivityCapability)
                .ToList();

            var foundationCapabilities = await dbContext.FrameworkCapabilities.AsNoTracking()
                .Include(fc => fc.Capability)
                .Where(fc => fc.IsFoundation)
                .OrderBy(fc => fc.Capability.Name)
                .ToListAsync();

            bool MeetsAllFoundationCapabilities(CatalogueItem item) => foundationCapabilities
                .All(fc => item.CatalogueItemCapabilities.Any(cic => cic.CapabilityId == fc.CapabilityId));

            var countOfCatalogueItemsWithAllFoundationCapabilities = results.Count(MeetsAllFoundationCapabilities);

            var foundationCapabilitiesFilter = foundationCapabilities
                .Select(c => new CapabilitiesFilter
                {
                    CapabilityId = c.CapabilityId,
                    Name = c.Capability.Name,
                    CapabilityRef = c.Capability.CapabilityRef,
                })
                .ToList();

            var epics = results.SelectMany(ci => ci.CatalogueItemEpics)
                .Select(cie => cie.Epic)
                .Where(e => e.CompliancyLevel == CompliancyLevel.May)
                .DistinctBy(e => e.Id)
                .ToList();

            // for each category, add to its list of capabilities the capabilities that reference that category, and count how many
            // catalogue items reference that capability
            categories.ForEach(c => c.Capabilities.AddRange(
                capabilities.Where(cap => cap.CategoryId == c.CategoryId)
                    .Select(cap => new CapabilitiesFilter
                    {
                        CapabilityId = cap.Id,
                        Name = cap.Name,
                        CapabilityRef = cap.CapabilityRef,
                        Count = results.Count(ci => ci.CatalogueItemCapabilities.Any(cic => cic.CapabilityId == cap.Id)),
                    })));

            // for each category, then each capability in that category, add to its list of epics the epics that reference that capability, and count how many
            // catalogue items reference that epic
            categories.ForEach(
                c => c.Capabilities.ForEach(
                    cap => cap.Epics.AddRange(
                        epics.Where(e => e.CapabilityId == cap.CapabilityId)
                            .Select(e => new EpicsFilter
                            {
                                Id = e.Id,
                                Name = e.Name,
                                Count = results.Count(ci => ci.CatalogueItemEpics.Any(cie => cie.EpicId == e.Id)),
                            })
                            .OrderByDescending(e => e.Count)
                            .ThenBy(e => e.Name))));

            var response = new CategoryFilterModel
            {
                CategoryFilters = categories,
                FoundationCapabilities = foundationCapabilitiesFilter,
                CountOfCatalogueItemsWithFoundationCapabilities = countOfCatalogueItemsWithAllFoundationCapabilities,
            };

            return response;
        }

        /// <summary>
        /// loop through the capabilities and epics and generates an EF where clause from the list.
        /// </summary>
        /// <param name="dbContext">the dbContext.</param>
        /// <param name="selectedCapabilities">the pipe-delimited string of selected capabilities and epics.</param>
        /// <returns>an Expression Starter Containing the Where Clause for the EF Query.</returns>
        private static ExpressionStarter<CatalogueItem> BuildCapabilitiesPredicate(BuyingCatalogueDbContext dbContext, string selectedCapabilities)
        {
            var capabilities = DecodeCapabilitiesFilter(selectedCapabilities);

            var predicateBuilder = PredicateBuilder.New<CatalogueItem>();

            var foundationSolutionsCount = dbContext.FrameworkCapabilities.Count(fc => fc.IsFoundation);

            foreach ((var capabilityId, List<string> epicIds) in capabilities)
            {
                var capabilityPredicateBuilder = PredicateBuilder.New<CatalogueItem>();

                capabilityPredicateBuilder = capabilityId == FoundationCapabilitiesKey
                    ? capabilityPredicateBuilder.Or(
                        ci => ci.CatalogueItemCapabilities.Count(
                            cic => cic.Capability.FrameworkCapabilities.Any(fc => fc.IsFoundation)) == foundationSolutionsCount)
                    : capabilityPredicateBuilder.Or(ci => ci.CatalogueItemCapabilities.Any(cic => cic.Capability.CapabilityRef == capabilityId));

                if (epicIds.Any())
                {
                    var epicPredicateBuilder = BuildEpicsPredicate(epicIds);

                    capabilityPredicateBuilder = capabilityPredicateBuilder.Extend(epicPredicateBuilder, PredicateOperator.And);
                }

                predicateBuilder = predicateBuilder.Extend(capabilityPredicateBuilder);
            }

            return predicateBuilder;
        }

        private static ExpressionStarter<CatalogueItem> BuildEpicsPredicate(IEnumerable<string> selectedEpics)
        {
            var epicPredicateBuilder = PredicateBuilder.New<CatalogueItem>();

            return selectedEpics.Aggregate(epicPredicateBuilder, (current, epic) => current.And(ci => ci.CatalogueItemEpics.Any(cie => cie.EpicId == epic)));
        }

        private static Dictionary<string, List<string>> DecodeCapabilitiesFilter(string capabilities)
        {
            var output = new Dictionary<string, List<string>>();

            var splitCapabilities = capabilities.Split(CapabilitiesDelimiter);

            foreach (var capability in splitCapabilities)
            {
                if (capability == FoundationCapabilitiesKey || !capability.ContainsIgnoreCase(EpicStartingCharacter))
                {
                    output.Add(capability, new List<string>());
                }
                else
                {
                    var epics = capability
                        .Split(SupplierStartingCharacter)
                        .SelectMany(s => s.Split(EpicStartingCharacter))
                        .ToList();

                    var list = epics
                    .Where(s => !s.StartsWith(CapabilitiesStartingCharacter))
                    .Select(s =>
                    {
                        if (s.ContainsIgnoreCase(DfocvcMarkCharacter))
                            return DecodeDfocvcEpic(s);

                        return s.ContainsIgnoreCase(SupplierMarkCharacter)
                            ? DecodeSupplierDefinedEpic(s)
                            : DecodeNormalEpic(epics[0], s);
                    })
                    .ToList();

                    output.Add(epics[0], list);
                }
            }

            return output;
        }

        private static string DecodeNormalEpic(string capabilityId, string encodedEpic) => $"{capabilityId}E{encodedEpic}";

        private static string DecodeDfocvcEpic(string encodedEpic) => $"E000{encodedEpic[..2]}";

        private static string DecodeSupplierDefinedEpic(string encodedEpic) => ("S0" + encodedEpic)
            .Replace("_", "E0", StringComparison.Ordinal).Replace("X", "X0", StringComparison.OrdinalIgnoreCase);
    }
}
