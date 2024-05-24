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

        public AdditionalFiltersModel AdditionalFilters { get; set; }

        public bool SearchCriteriaApplied => ResultsModel.Filters.SearchCriteriaApplied;

        public SolutionsResultsModel ResultsModel { get; set; }

        public PageTitleModel GetPageTitle()
        {
            return SearchResultsPageTitle;
        }
    }
}
