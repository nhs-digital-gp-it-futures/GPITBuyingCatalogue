using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/manage-capabilities")]
    public class ManageCapabilitiesController : Controller
    {
        private readonly ICapabilitiesAdminService capabilitiesAdminService;

        public ManageCapabilitiesController(ICapabilitiesAdminService capabilitiesAdminService)
        {
            this.capabilitiesAdminService = capabilitiesAdminService ?? throw new ArgumentNullException(nameof(capabilitiesAdminService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page = "")
        {
            const int pageSize = 20;
            var pageOptions = new PageOptions(page, pageSize);

            var capabilities = await capabilitiesAdminService.GetPagedCapabilitiesAsync(pageOptions);

            var model = new ManageCapabilitiesModel(capabilities.Items, capabilities.Options)
            {
                BackLink = Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }
    }
}
