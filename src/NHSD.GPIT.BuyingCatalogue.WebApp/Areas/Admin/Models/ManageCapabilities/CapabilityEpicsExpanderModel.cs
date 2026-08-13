using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

public class CapabilityEpicsExpanderModel
{
    public IEnumerable<AdminManageEpic> Epics { get; set; }
}
