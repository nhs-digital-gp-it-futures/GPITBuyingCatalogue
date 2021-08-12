using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section")]
    public sealed class AboutSolutionController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;
        private readonly IInteroperabilityService interoperabilityService;

        public AboutSolutionController(
            IMapper mapper,
            ISolutionsService solutionsService,
            IInteroperabilityService interoperabilityService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.interoperabilityService = interoperabilityService ?? throw new ArgumentNullException(nameof(interoperabilityService));
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

            await interoperabilityService.SaveIntegrationLink(solutionId, model.Link);

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
