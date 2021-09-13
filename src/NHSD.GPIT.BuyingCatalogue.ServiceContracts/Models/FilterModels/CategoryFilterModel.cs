using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public sealed class CategoryFilterModel
    {
        public List<CapabilityCategoryFilter> CategoryFilters { get; init; }

        public List<CapabilitiesFilter> FoundationCapabilities { get; init; }

        public int CountOfCatalogueItemsWithFoundationCapabilities { get; init; }
    }
}
