using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

public class ManageEpicsModel : NavBaseModel
{
    public ManageEpicsModel()
    {
    }

    public ManageEpicsModel(IList<AdminManageEpic> epics, PageOptions options)
    {
        Epics = [.. epics];
        Options = options;
    }

    public IList<AdminManageEpic> Epics { get; set; }

    public PageOptions Options { get; set; }
}
