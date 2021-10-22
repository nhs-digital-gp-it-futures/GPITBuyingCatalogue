using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class SaveCatalogueItemCapabilitiesModel
    {
        public IList<(int Id, List<string> EpicIds)> Capabilities { get; init; }

        public int UserId { get; init; }
    }
}
