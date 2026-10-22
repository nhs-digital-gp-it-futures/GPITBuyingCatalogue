using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapabilities;

public class AdminManageCapabilityCatalogueItem
{
    [ExcludeFromCodeCoverage]
    public required CatalogueItemId Id { get; set; }

    [ExcludeFromCodeCoverage]
    public required string Name { get; set; }

    [ExcludeFromCodeCoverage]
    public required string SupplierName { get; set; }

    [ExcludeFromCodeCoverage]
    public required CatalogueItemType CatalogueItemType { get; set; }

    [ExcludeFromCodeCoverage]
    public required PublicationStatus PublishedStatus { get; set; }
}
