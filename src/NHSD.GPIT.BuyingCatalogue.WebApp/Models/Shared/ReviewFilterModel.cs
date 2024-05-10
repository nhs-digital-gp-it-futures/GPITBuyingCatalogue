using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Shorlists;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

public class ReviewFilterModel : NavBaseModel
{
    public ReviewFilterModel()
    {
    }

    public ReviewFilterModel(
        FilterDetailsModel filterDetails, FilterIdsModel filterIds = null)
    {
        FilterDetails = filterDetails;
        FilterIds = filterIds;
    }

    public ReviewFilterModel(FilterDetailsModel filterDetails, string internalOrgId, List<CatalogueItem> filterResults, bool inCompetition, FilterIdsModel filterIds = null)
        : this(filterDetails, filterIds)
    {
        ResultsCount = filterResults.Count;
        InCompetition = inCompetition;
        ResultsForFrameworks = new List<ResultsForFrameworkModel>();

        var selectedFrameworks = filterResults
            .SelectMany(x => x.Solution.FrameworkSolutions)
            .GroupBy(x => (x.Framework.ShortName, x.Framework.Id));

        if (!string.IsNullOrWhiteSpace(filterIds.FrameworkId))
            selectedFrameworks = selectedFrameworks.Where(x => string.Equals(x.Key.Id, filterIds.FrameworkId));

        ResultsForFrameworks = selectedFrameworks.Select(
            x => new ResultsForFrameworkModel(
                internalOrgId,
                filterDetails.Id,
                x.Key.Id,
                x.Key.ShortName,
                x.Select(y => y.Solution.CatalogueItem).ToList(),
                !inCompetition))
        .ToList();
    }

    public FilterDetailsModel FilterDetails { get; set; }

    public FilterIdsModel FilterIds { get; set; }

    public string InternalOrgId { get; set; }

    public string OrganisationName { get; set; }

    public int ResultsCount { get; set; }

    public List<ResultsForFrameworkModel> ResultsForFrameworks { get; set; }

    public bool InExpander { get; set; }

    public bool InCompetition { get; set; }

    public bool HasEpics() => FilterDetails.Capabilities.Any(x => x.Value.Any());

    public bool HasFramework() => !string.IsNullOrEmpty(FilterDetails.FrameworkName);

    public bool HasHostingTypes() => FilterDetails.HostingTypes.Any();

    public bool HasApplicationTypes() => FilterDetails.ApplicationTypes.Any();

    public bool HasInteroperabilityIntegrationTypes() => FilterDetails.InteropIntegrationTypes.Any();

    public bool HasAdditionalFilters() => HasFramework()
        || HasHostingTypes() || HasApplicationTypes() || HasInteroperabilityIntegrationTypes();
}
