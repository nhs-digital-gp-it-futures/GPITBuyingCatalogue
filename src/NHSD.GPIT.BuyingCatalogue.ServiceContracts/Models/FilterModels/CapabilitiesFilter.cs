using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels
{
    public sealed class CapabilitiesFilter
    {
        public int CapabilityId { get; init; }

        public string CapabilityRef { get; init; }

        public string Name { get; init; }

        public int Count { get; init; }

        public bool Selected { get; init; }

        public string DisplayText => $"{Name} ({Count})";

        public List<EpicsFilter> Epics { get; } = new();
    }
}
