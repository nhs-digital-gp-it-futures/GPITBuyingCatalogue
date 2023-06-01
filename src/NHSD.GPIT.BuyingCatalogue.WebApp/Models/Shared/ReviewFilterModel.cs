using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

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
