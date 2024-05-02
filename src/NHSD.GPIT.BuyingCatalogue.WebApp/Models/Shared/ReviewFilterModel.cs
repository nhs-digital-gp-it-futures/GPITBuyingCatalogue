﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

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

    public FilterDetailsModel FilterDetails { get; set; }

    public FilterIdsModel FilterIds { get; set; }

    public string InternalOrgId { get; set; }

    public string OrganisationName { get; set; }

    public List<CatalogueItem> FilterResults { get; set; }

    public List<FrameworkFilterInfo> Frameworks { get; set; }

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
