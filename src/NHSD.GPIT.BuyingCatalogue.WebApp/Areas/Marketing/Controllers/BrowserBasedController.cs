using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section/browser-based")]
    public sealed class BrowserBasedController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public BrowserBasedController(
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("supported-browsers")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, SupportedBrowsersModel>(solution));
        }

        [HttpPost("supported-browsers")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId, SupportedBrowsersModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            mapper.Map(model, clientApplication);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, MobileFirstApproachModel>(solution));
        }

        [HttpPost("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(CatalogueItemId solutionId, MobileFirstApproachModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MobileFirstDesign = mapper.Map<string, bool?>(model.MobileFirstApproach);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, PlugInsOrExtensionsModel>(solution));
        }

        [HttpPost("plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId, PlugInsOrExtensionsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.Plugins = mapper.Map<PlugInsOrExtensionsModel, Plugins>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(solution));
        }

        [HttpPost("connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId, ConnectivityAndResolutionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            mapper.Map(model, clientApplication);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, HardwareRequirementsModel>(solution));
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.HardwareRequirements = model.Description;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AdditionalInformationModel>(solution));
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.AdditionalInformation = model.AdditionalInformation;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        private RedirectToActionResult RedirectBack(CatalogueItemId solutionId)
        {
            return RedirectToAction(
                nameof(ClientApplicationTypeController.BrowserBased),
                typeof(ClientApplicationTypeController).ControllerName(),
                new { solutionId });
        }
    }
}
