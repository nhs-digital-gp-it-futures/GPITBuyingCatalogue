using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2CatalogueItemMappingModel
{
    public Gen2CatalogueItemMappingModel(
        string catalogueItemId,
        IEnumerable<Gen2CapabilityMappingModel> capabilities)
    {
        Id = CatalogueItemId.ParseExact(catalogueItemId);
        Capabilities = capabilities.DistinctBy(x => x.CapabilityId).ToList();
    }

    public CatalogueItemId Id { get; set; }

    public ICollection<Gen2CapabilityMappingModel> Capabilities { get; set; }
}
