using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

public class EpicCapabilitiesExpanderModel
{
    public IEnumerable<AdminManageCapability> Capabilities { get; set; }
}
