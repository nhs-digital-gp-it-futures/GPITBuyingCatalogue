using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CapabilitiesFilter
    {
        public int CapabilityId { get; set; }

        public string CapabilityRef { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public bool Selected { get; set; }

        public string DisplayText => $"{Name} ({Count})";

        public List<EpicsFilter> Epics { get; set; } = new List<EpicsFilter>();
    }
}
