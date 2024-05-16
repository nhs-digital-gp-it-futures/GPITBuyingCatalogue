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

        public AdditionalFiltersModel AdditionalFilters { get; set; }

        public bool SearchCriteriaApplied => ResultsModel.Filters.SearchCriteriaApplied;

        public SolutionsResultsModel ResultsModel { get; set; }

        public PageTitleModel GetPageTitle()
        {
            return SearchResultsPageTitle;
        }
    }
}
