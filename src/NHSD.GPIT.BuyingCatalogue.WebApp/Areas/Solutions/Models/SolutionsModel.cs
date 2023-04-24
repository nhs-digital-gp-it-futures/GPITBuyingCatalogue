using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using static NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.PageOptions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public const string TitleNoSearch = "Find Buying Catalogue Solutions";
        public const string TitleSearchNoResults = "No Catalogue Solutions found";
        public const string TitleSearchResults = "Catalogue Solutions found";

        public const string AdviceTextNosearch = "Search for Catalogue Solutions that match the needs of your organisation.";
        public const string AdviceTextSearchNoresults = "There are no Catalogue Solutions that meet your search criteria.";
        public const string AdviceTextSearchResults = "These are the Catalogue Solutions that meet your search criteria. You can apply additional filters to refine your results further, or compare these results.";

        public SolutionsModel()
        {
        }

        public IncludeAdditionalfiltersModel AdditionalFilters { get; set; }

        public IList<CatalogueItem> CatalogueItems { get; init; }

        public CatalogueFilterSearchSummary SearchSummary { get; init; }

        // only used for the UI component
        public SortOptions SelectedSortOption { get; init; }

        public PageOptions PageOptions { get; set; }

        public IEnumerable<SelectOption<string>> SortOptions => Enum.GetValues(typeof(SortOptions))
            .Cast<SortOptions>()
            .Where(e => !e.Equals(PageOptions.SortOptions.None))
            .Select(e => new SelectOption<string>(e.Name(), e.ToString(), e == SelectedSortOption));

        public bool SearchCriteriaApplied =>
            !string.IsNullOrWhiteSpace(SearchSummary?.SelectedCapabilityIds)
            || !string.IsNullOrWhiteSpace(SearchSummary?.SearchTerm);

        public bool AdditionalFiltersApplicable =>
            !string.IsNullOrWhiteSpace(SearchSummary?.SelectedCapabilityIds);

        public string TitleText =>
            !SearchCriteriaApplied
                ? TitleNoSearch
                : CatalogueItems?.Count == 0
                   ? TitleSearchNoResults
                   : TitleSearchResults;

        public string AdviceText =>
            !SearchCriteriaApplied
            ? AdviceTextNosearch
            : CatalogueItems?.Count == 0
                ? AdviceTextSearchNoresults
                : AdviceTextSearchResults;
    }
}
