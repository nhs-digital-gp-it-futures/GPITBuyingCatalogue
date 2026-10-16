using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "ManageEpics")]
[Area("Admin")]
[Route("admin/manage-epics")]
public class ManageEpicsController(IEpicsService epicsService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string page)
    {
        var inputOptions = new PageOptions(page)
        {
            PageSize = 20,
        };

        var pagedList = await epicsService.GetEpics(inputOptions);
        var model = new ViewEpicsModel(pagedList.Items.Select(epic => new Epic(
                epic.Name,
                epic.Id,
                epic.IsActive)))
        {
            Title = "Manage Epics",
            BackLink = Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName()),
            Advice = "View existing epics",
            PageOptions = pagedList.Options,
        };

        return View(model);
    }
}
