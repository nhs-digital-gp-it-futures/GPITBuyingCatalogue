using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

public class UpdateAdminCapability
{
    public int Id { get; set; }

    public string CapabilityRef { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Uri SourceUrl { get; set; }

    public CapabilityCategory CapabilityCategory { get; set; }

    public CapabilityStatus Status { get; set; }
}
