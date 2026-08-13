using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

public record AdminManageCapabilityCatalogueItem
{
    public required CatalogueItemId Id { get; set; }

    public required string Name { get; set; }

    public required string SupplierName { get; set; }

    public required CatalogueItemType CatalogueItemType { get; set; }

    public required PublicationStatus PublishedStatus { get; set; }
}
