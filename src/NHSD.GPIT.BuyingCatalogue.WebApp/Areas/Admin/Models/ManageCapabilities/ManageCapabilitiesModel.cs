using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

public class ManageCapabilitiesModel : NavBaseModel
{
    public ManageCapabilitiesModel()
    {
    }

    public ManageCapabilitiesModel(IList<AdminManageCapability> capabilities, PageOptions options)
    {
        Capabilities = [.. capabilities];
        Options = options;
    }

    public IList<AdminManageCapability> Capabilities { get; set; }

    public PageOptions Options { get; set; }
}
