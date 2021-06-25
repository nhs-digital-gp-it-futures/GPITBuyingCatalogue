using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section")]
    public sealed class AboutSolutionController : Controller
    {
        private readonly ILogWrapper<AboutSolutionController> logger;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public AboutSolutionController(
            ILogWrapper<AboutSolutionController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, FeaturesModel>(solution));
        }

        [HttpPost("features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId, FeaturesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var features = mapper.Map<FeaturesModel, string[]>(model);
            await solutionsService.SaveSolutionFeatures(solutionId, features);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [HttpPost("implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId, ImplementationTimescalesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveImplementationDetail(solutionId, model.Description);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("integrations")]
        public async Task<IActionResult> Integrations(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, IntegrationsModel>(solution));
        }

        [HttpPost("integrations")]
        public async Task<IActionResult> Integrations(CatalogueItemId solutionId, IntegrationsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveIntegrationLink(solutionId, model.Link);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("roadmap")]
        public async Task<IActionResult> RoadMap(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, RoadmapModel>(solution));
        }

        [HttpPost("roadmap")]
        public async Task<IActionResult> RoadMap(CatalogueItemId solutionId, RoadmapModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveRoadMap(solutionId, model.Summary);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("solution-description")]
        public async Task<IActionResult> SolutionDescription(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution));
        }

        [HttpPost("solution-description")]
        public async Task<IActionResult> SolutionDescription(CatalogueItemId solutionId, SolutionDescriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await solutionsService.SaveSolutionDescription(
                solutionId,
                model.Summary,
                model.Description,
                model.Link);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }
    }
}
