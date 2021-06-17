using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class AboutSolutionController : Controller
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
        public async Task<IActionResult> Features(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"Features-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, FeaturesModel>(solution));
        }

        [HttpPost("features")]
        public async Task<IActionResult> Features(FeaturesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var features = mapper.Map<FeaturesModel, string[]>(model);
            await solutionsService.SaveSolutionFeatures(model.SolutionId, features);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"implementation-timescales-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [HttpPost("implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(ImplementationTimescalesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveImplementationDetail(model.SolutionId, model.Description);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("integrations")]
        public async Task<IActionResult> Integrations(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"integrations-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, IntegrationsModel>(solution));
        }

        [HttpPost("integrations")]
        public async Task<IActionResult> Integrations(IntegrationsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveIntegrationLink(model.SolutionId, model.Link);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("roadmap")]
        public async Task<IActionResult> Roadmap(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"roadmap-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, RoadmapModel>(solution));
        }

        [HttpPost("roadmap")]
        public async Task<IActionResult> Roadmap(RoadmapModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveRoadmap(model.SolutionId, model.Summary);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("solution-description")]
        public async Task<IActionResult> SolutionDescription(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"solution-description-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution));
        }

        [HttpPost("solution-description")]
        public async Task<IActionResult> SolutionDescription(SolutionDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await solutionsService.SaveSolutionDescription(
                model.SolutionId,
                model.Summary,
                model.Description,
                model.Link);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }
    }
}
