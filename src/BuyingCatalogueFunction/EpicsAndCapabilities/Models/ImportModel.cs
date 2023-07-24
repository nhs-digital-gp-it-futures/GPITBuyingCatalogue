using System.Collections.Generic;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Models
{
    public record ImportModel(
        List<CapabilityCsv> Capabilities,
        List<StandardCsv> Standards,
        List<EpicCsv> Epics,
        List<StandardCapabilityCsv> StandardCapabilities);
}
