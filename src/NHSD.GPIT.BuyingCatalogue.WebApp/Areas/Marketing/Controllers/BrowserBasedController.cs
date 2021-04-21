using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class BrowserBasedController : Controller
    {
        private readonly ILogger<BrowserBasedController> _logger;
        private readonly ISolutionsService _solutionsService;

        public BrowserBasedController(ILogger<BrowserBasedController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/supported-browsers")]
        public async Task<IActionResult> BrowserBasedSupportedBrowsers(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/mobile-first-approach")]
        public async Task<IActionResult> BrowserBasedMobileFirstApproach(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> BrowserBasedPlugInsOrExtensions(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> BrowserBasedConnectivityAndResolution(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/hardware-requirements")]
        public async Task<IActionResult> BrowserBasedHardwareRequirements(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/additional-information")]
        public async Task<IActionResult> BrowserBasedAdditionalInformation(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }              
    }
}
