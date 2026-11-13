using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionsModel
    {
        public AdditionalFiltersModel AdditionalFilters { get; set; }

        public bool SearchCriteriaApplied => ResultsModel.Filters.SearchCriteriaApplied;

        public SolutionsResultsModel ResultsModel { get; set; }

        public SolutionType SolutionType { get; set; }

        public PageTitleModel GetPageTitle() => new()
        {
            Title = SolutionType switch
            {
                SolutionType.CommunityPharmacy => "Pharmacy solutions",
                _ => "Catalogue solutions",
            },
        };
    }
}
