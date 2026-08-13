using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Epics;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/manage-epics")]
    public class ManageEpicsController(IEpicsAdminService epicsAdminService) : Controller
    {
        private readonly IEpicsAdminService epicsAdminService = epicsAdminService ??
            throw new ArgumentNullException(nameof(epicsAdminService));

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page = "",
            [FromQuery] string search = "")
        {
            const int pageSize = 10;
            var pageOptions = new PageOptions(page, pageSize);

            var epics = await epicsAdminService.GetPagedEpics(pageOptions, search);

            var model = new ManageEpicsModel(epics.Items, epics.Options)
            {
                BackLink = Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }

        [HttpGet("{epicId}/edit")]
        public async Task<IActionResult> Edit(string epicId)
        {
            var epic = await epicsAdminService.GetEpic(epicId);

            if (epic == null)
                return NotFound();

            var model = new ManageEpicModel(epic)
            {
                BackLink = Url.Action(nameof(Index)),
            };

            return View("EpicDetails", model);
        }

        [HttpPost("{epicId}/edit")]
        public async Task<IActionResult> Save(string epicId, ManageEpicModel updatedEpic)
        {
            if (updatedEpic == null)
                return BadRequest();

            var epic = await epicsAdminService.GetEpic(epicId);
            if (epic == null)
                return NotFound();

            var request = new UpdateAdminEpic()
            {
                Id = updatedEpic.Id,
                Name = updatedEpic.Name,
                Description = updatedEpic.Description,
                IsActive = updatedEpic.IsActive,
                SourceUrl = updatedEpic.SourceUrl != null
                    ? new Uri(updatedEpic.SourceUrl)
                    : null,
            };

            await epicsAdminService.UpdateEpic(request);

            return RedirectToAction(nameof(Index));
        }
    }
}
