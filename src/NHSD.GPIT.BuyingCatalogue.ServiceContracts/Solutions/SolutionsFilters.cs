using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

public class SolutionsFilters
{
    public Dictionary<int, string[]> CapabilitiesAndEpics { get; init; }

    public string Search { get; set; }

    public string SelectedFrameworkId { get; set; }

    public string SelectedApplicationTypeIds { get; set; }

    public string SelectedHostingTypeIds { get; set; }

    public Dictionary<SupportedIntegrations, int[]> SelectedIntegrationsAndTypes { get; init; }

    public bool? IsCommunityPharmacy { get; set; }
}
