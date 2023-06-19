using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/application-type/browser-based")]
    public class BrowserBasedController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public BrowserBasedController(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> BrowserBased(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = solution.Solution.GetApplicationTypes();
            var model = new BrowserBasedModel(solution)
            {
                BackLink = clientApplication?.HasApplicationType(ApplicationType.BrowserBased) ?? false
                           ? Url.Action(nameof(CatalogueSolutionsController.ApplicationType), typeof(CatalogueSolutionsController).ControllerName(), new { solutionId })
                           : Url.Action(nameof(CatalogueSolutionsController.AddApplicationType), typeof(CatalogueSolutionsController).ControllerName(), new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("supported-browser")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new SupportedBrowsersModel(solution));
        }

        [HttpPost("supported-browser")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId, SupportedBrowsersModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var applicationType = await solutionsService.GetApplicationTypes(solutionId);
            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.BrowsersSupported.Clear();
            applicationType.BrowsersSupported = model.Browsers is null
                ? new HashSet<SupportedBrowser>()
                : model.Browsers.Where(b => b.Checked).Select(b =>
                new SupportedBrowser
                {
                    BrowserName = b.BrowserName,
                    MinimumBrowserVersion = b.MinimumBrowserVersion,
                }).ToHashSet();

            applicationType.MobileResponsive = string.IsNullOrWhiteSpace(model.MobileResponsive)
                ? null
                : model.MobileResponsive.EqualsIgnoreCase("Yes");

            applicationType.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new PlugInsOrExtensionsModel(solution));
        }

        [HttpPost("plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId, PlugInsOrExtensionsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var applicationTypes = await solutionsService.GetApplicationTypes(solutionId);
            if (applicationTypes is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationTypes.Plugins = new Plugins
            {
                AdditionalInformation = model.AdditionalInformation,
                Required = string.IsNullOrWhiteSpace(model.PlugInsRequired)
                    ? null
                    : model.PlugInsRequired.EqualsIgnoreCase("Yes"),
            };

            applicationTypes.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            await solutionsService.SaveApplicationType(solutionId, applicationTypes);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ConnectivityAndResolutionModel(solution));
        }

        [HttpPost("connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId, ConnectivityAndResolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                return View(new ConnectivityAndResolutionModel(solution));
            }

            var clientApplication = await solutionsService.GetApplicationTypes(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MinimumDesktopResolution = model.SelectedScreenResolution;
            clientApplication.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new HardwareRequirementsModel(solution));
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = await solutionsService.GetApplicationTypes(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.HardwareRequirements = model.Description;
            clientApplication.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new AdditionalInformationModel(solution));
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationTypes(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.AdditionalInformation = model.AdditionalInformation;
            clientApplication.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }
    }
}
