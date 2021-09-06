using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CategoryFilterModel
    {
        public List<CapabilityCategoryFilter> CategoryFilters { get; set; }

        public List<CapabilitiesFilter> FoundationCapabilities { get; set; }

        public int CountOfCatalogueItemsWithFoundationCapabilities { get; set; }
    }
}
