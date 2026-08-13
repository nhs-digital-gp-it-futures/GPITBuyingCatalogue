using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

public record AdminManageCapability
{
    public required int Id { get; set; }

    public required string CapabilityRef { get; set; }

    public required string Name { get; set; }

    public string? CapabilityCategoryName { get; set; }

    public required CapabilityStatus Status { get; set; }
}
