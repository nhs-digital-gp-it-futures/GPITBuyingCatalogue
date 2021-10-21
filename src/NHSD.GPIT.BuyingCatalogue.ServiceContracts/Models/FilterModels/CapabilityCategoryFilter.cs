using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class CapabilityCategoryFilter
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public int CategoryId { get; init; }

        public List<CapabilitiesFilter> Capabilities { get; } = new();
    }
}
