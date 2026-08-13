using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;

public record UpdateAdminEpic
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public Uri? SourceUrl { get; set; }

    public bool IsActive { get; set; }
}
