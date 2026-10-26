using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

public class ManageCapabilityModel : NavBaseModel
{
    public ManageCapabilityModel()
    {
    }

    public ManageCapabilityModel(Capability capability)
    {
        Id = capability.Id;
        CapabilityRef = capability.CapabilityRef;
        Name = capability.Name;
        Description = capability.Description;
        SourceUrl = capability.SourceUrl;
        Status = capability.Status;
    }

    public int Id { get; set; }

    public string CapabilityRef { get; set; }

    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public string SourceUrl { get; set; }

    public CapabilityStatus Status { get; set; }
}
