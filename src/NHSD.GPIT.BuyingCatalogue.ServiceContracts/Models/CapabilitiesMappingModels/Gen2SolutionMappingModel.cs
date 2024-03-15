using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2SolutionMappingModel(
    string catalogueItemId,
    ICollection<Gen2CapabilityMappingModel> capabilities,
    ICollection<Gen2CatalogueItemMappingModel> additionalServices)
    : Gen2CatalogueItemMappingModel(catalogueItemId, capabilities)
{
    public ICollection<Gen2CatalogueItemMappingModel> AdditionalServices { get; set; } = additionalServices;
}
