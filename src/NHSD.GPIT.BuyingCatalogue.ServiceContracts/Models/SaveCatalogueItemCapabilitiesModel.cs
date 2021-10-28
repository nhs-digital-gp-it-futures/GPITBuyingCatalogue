using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class SaveCatalogueItemCapabilitiesModel
    {
        public Dictionary<int, string[]> Capabilities { get; init; }

        public int UserId { get; init; }
    }
}
