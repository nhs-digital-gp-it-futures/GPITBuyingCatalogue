using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

public class ViewEpicsModel(IEnumerable<Epic> epics) : NavBaseModel
{
    public IEnumerable<Epic> Epics { get; set; } = epics;

    public PageOptions PageOptions { get; init; }
}
