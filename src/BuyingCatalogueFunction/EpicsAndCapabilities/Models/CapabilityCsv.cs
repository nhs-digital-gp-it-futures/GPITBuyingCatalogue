using System.Collections.Generic;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Models
{
    public class CapabilityCsv
    {
        public CapabilityIdCsv Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }
    }
}
