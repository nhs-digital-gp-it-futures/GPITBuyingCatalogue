using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public sealed class CapabilityCategoryFilter
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public int CategoryId { get; init; }

        public List<CapabilitiesFilter> Capabilities { get; } = new();
    }
}
