using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionsModel
    {
        public static readonly PageTitleModel SearchResultsPageTitle = new()
        {
            Title = "Catalogue Solutions",
        };

        public static readonly PageTitleModel SearchNoResultsFilterPageTitle = new()
        {
            Title = "Catalogue Solutions found",
            Advice = "These are the Catalogue Solutions that meet your search criteria.",
        };

        public static readonly PageTitleModel SearchResultsFilterPageTitle = new()
        {
            Title = "Catalogue Solutions found",
            Advice = "These are the Catalogue Solutions currently available for your saved filter.",
        };

        public string FilterName { get; set; }

        public bool FilterResultView => !string.IsNullOrEmpty(FilterName);

        public AdditionalFiltersModel AdditionalFilters { get; set; }

        public RequestedFilters Filters { get; set; }

        public IList<CatalogueItem> CatalogueItems { get; init; }

        public PageOptions PageOptions { get; set; }

        public PageTitleModel GetPageTitle()
        {
            if (CatalogueItems?.Count == 0)
            {
                return string.IsNullOrEmpty(FilterName)
                    ? SearchResultsPageTitle
                    : SearchNoResultsFilterPageTitle with { Caption = FilterName };
            }

            return string.IsNullOrEmpty(FilterName)
                     ? SearchResultsPageTitle
                     : SearchResultsFilterPageTitle with { Caption = FilterName };
        }
    }
}
