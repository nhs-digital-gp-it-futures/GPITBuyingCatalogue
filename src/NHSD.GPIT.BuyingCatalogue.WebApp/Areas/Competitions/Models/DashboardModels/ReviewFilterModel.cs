using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public class ReviewFilterModel : NavBaseModel
{
    public ReviewFilterModel()
    {
    }

    public ReviewFilterModel(
        FilterDetailsModel model)
    {
        FilterDetails = model;
    }

    public FilterDetailsModel FilterDetails { get; set; }

    public bool HasEpics() => FilterDetails.Capabilities.Any(x => x.Value.Any());

    public bool HasFramework() => !string.IsNullOrEmpty(FilterDetails.FrameworkName);

    public bool HasHostingTypes() => FilterDetails.HostingTypes.Any();

    public bool HasClientApplicationTypes() => FilterDetails.ClientApplicationTypes.Any();

    public bool HasAdditionalFilters() => HasFramework()
        || HasHostingTypes() || HasClientApplicationTypes();
}
