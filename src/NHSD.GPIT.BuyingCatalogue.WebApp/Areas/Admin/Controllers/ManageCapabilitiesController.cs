using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/manage-capabilities")]
    public class ManageCapabilitiesController(ICapabilitiesAdminService capabilitiesAdminService) : Controller
    {
        private readonly ICapabilitiesAdminService capabilitiesAdminService = capabilitiesAdminService ??
            throw new ArgumentNullException(nameof(capabilitiesAdminService));

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page = "",
            [FromQuery] string search = "")
        {
            const int pageSize = 10;
            var pageOptions = new PageOptions(page, pageSize);

            var capabilities = await capabilitiesAdminService.GetPagedCapabilitiesAsync(pageOptions, search);

            var model = new ManageCapabilitiesModel(capabilities.Items, capabilities.Options)
            {
                BackLink = Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }

        [HttpGet("{capabilityId}/edit")]
        public async Task<IActionResult> Edit(int capabilityId)
        {
            var capability = await capabilitiesAdminService.GetCapabilityAsync(capabilityId);

            if (capability == null)
                return NotFound();

            var model = new ManageCapabilityModel(capability)
            {
                BackLink = Url.Action(nameof(Index)),
            };

            return View("CapabilityDetails", model);
        }

        [HttpPost("{capabilityId}/edit")]
        public async Task<IActionResult> Save(int capabilityId, ManageCapabilityModel updatedCapability)
        {
            if (updatedCapability == null)
                return BadRequest();

            var capability = await capabilitiesAdminService.GetCapabilityAsync(capabilityId);
            if (capability == null)
                return NotFound();

            var request = new UpdateAdminCapability()
            {
                Id = updatedCapability.Id,
                CapabilityRef = updatedCapability.CapabilityRef,
                Name = updatedCapability.Name,
                Description = updatedCapability.Description,
                SourceUrl = new Uri(updatedCapability.SourceUrl),
                Status = updatedCapability.Status,
            };

            await capabilitiesAdminService.UpdateCapabilityAsync(request);

            return RedirectToAction(nameof(Index));
        }
    }
}
