using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CapabilityCategoryFilter
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public List<CapabilitiesFilter> Capabilities { get; set; } = new List<CapabilitiesFilter>();
    }
}
