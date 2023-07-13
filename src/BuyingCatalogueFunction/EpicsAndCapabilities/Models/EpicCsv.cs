using System.Collections.Generic;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Models
{
    public class EpicCsv
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<CapabilityEpicCsv> Capabilities { get; set; } = new List<CapabilityEpicCsv>();
    }
}
