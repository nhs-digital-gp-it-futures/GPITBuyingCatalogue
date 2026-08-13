using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

public class ManageEpicModel : NavBaseModel
{
    public ManageEpicModel()
    {
    }

    public ManageEpicModel(Epic epic)
    {
        Id = epic.Id;
        Name = epic.Name;
        Description = epic.Description;
        SourceUrl = epic.SourceUrl;
        IsActive = epic.IsActive;
        CapabilitiesExpanderModel = new EpicCapabilitiesExpanderModel
        {
            Capabilities = epic.Capabilities.Select(x => new AdminManageCapability
            {
                Id = x.Id,
                CapabilityRef = x.CapabilityRef,
                Name = x.Name,
                CapabilityCategoryName = x.Category.Name,
                Status = x.Status,
            }),
        };
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string SourceUrl { get; set; }

    public bool IsActive { get; set; }

    public EpicCapabilitiesExpanderModel CapabilitiesExpanderModel { get; set; }

    public IList<SelectOption<string>> IsActiveOptions =>
        [
            new(true.ToYesNo(), true.ToString()),
            new(false.ToYesNo(), false.ToString()),
        ];
}
