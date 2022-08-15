using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("catalogue-solutions")]
    public class FilterController : Controller
    {
        private readonly ICapabilitiesService capabilitiesService;

        public FilterController(ICapabilitiesService capabilitiesService)
        {
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
        }

        [HttpGet("filter-capabilities")]
        public async Task<IActionResult> FilterCapabilities(string selectedIds = null)
        {
            var capabilities = await capabilitiesService.GetCapabilities();
            var model = new FilterCapabilitiesModel(capabilities, selectedIds);

            return View(model);
        }

        [HttpPost("filter-capabilities")]
        public async Task<IActionResult> FilterCapabilities(FilterCapabilitiesModel model)
        {
            if (!ModelState.IsValid)
            {
                var capabilities = await capabilitiesService.GetCapabilities();

                return View(new FilterCapabilitiesModel(capabilities));
            }

            var selectedIds = string.Join(
                FilterCapabilitiesModel.FilterDelimiter,
                model.Items.Where(x => x.Selected).Select(x => x.Id));

            return RedirectToAction(
                nameof(FilterCapabilities),
                typeof(FilterController).ControllerName(),
                new { selectedIds });
        }
    }
}
