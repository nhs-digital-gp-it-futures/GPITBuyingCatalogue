using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using static NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.PageOptions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public static readonly PageTitleModel NoSearchPageTitle = new()
        {
            Title = "Find Buying Catalogue Solutions",
            Advice = "Search for Catalogue Solutions that match the needs of your organisation.",
        };

        public static readonly PageTitleModel SearchNoResultsPageTitle = new()
        {
            Title = "No Catalogue Solutions found",
            Advice = "There are no Catalogue Solutions that meet your search criteria.",
        };

        public static readonly PageTitleModel SearchNoResultsFilterPageTitle = new()
        {
            Title = "Catalogue Solutions found",
            Advice = "These are the Catalogue Solutions that meet your search criteria.",
        };

        public static readonly PageTitleModel SearchResultsPageTitle = new()
        {
            Title = "Catalogue Solutions found",
            Advice = "These are the Catalogue Solutions that meet your search criteria. You can apply additional filters to refine your results further, or compare these results.",
        };

        public static readonly PageTitleModel SearchResultsFilterPageTitle = new()
        {
            Title = "Catalogue Solutions found",
            Advice = "These are the Catalogue Solutions currently available for your saved filter.",
        };

        public string FilterName { get; set; }

        public bool FilterResultView => !string.IsNullOrEmpty(FilterName);

        public AdditionalFiltersModel AdditionalFilters { get; set; }

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

        public bool HasCapabilities =>
            !string.IsNullOrWhiteSpace(SearchSummary?.SelectedCapabilityIds);

        public PageTitleModel GetPageTitle()
        {
            if (!SearchCriteriaApplied)
            {
                return NoSearchPageTitle;
            }

            if (CatalogueItems?.Count == 0)
            {
                return string.IsNullOrEmpty(FilterName)
                    ? SearchNoResultsPageTitle
                    : SearchNoResultsFilterPageTitle with { Caption = FilterName };
            }

            return string.IsNullOrEmpty(FilterName)
                     ? SearchResultsPageTitle
                     : SearchResultsFilterPageTitle with { Caption = FilterName };
        }
    }
}
