using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class AboutSolutionController : Controller
    {
        private readonly ILogWrapper<AboutSolutionController> _logger;
        private readonly IMapper _mapper;
        private readonly ISolutionsService _solutionsService;

        public AboutSolutionController(ILogWrapper<AboutSolutionController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
        
        [HttpGet("features")]
        public async Task<IActionResult> Features(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, FeaturesModel>(solution));
        }

        [HttpPost("features")]
        public async Task<IActionResult> Features(FeaturesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var features = _mapper.Map<FeaturesModel, string[]>(model);
            await _solutionsService.SaveSolutionFeatures(model.SolutionId, features);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }
        
        [HttpGet("implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [HttpPost("implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(ImplementationTimescalesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveImplementationDetail(model.SolutionId, model.Description);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }

        [HttpGet("integrations")]
        public async Task<IActionResult> Integrations(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, IntegrationsModel>(solution));
        }

        [HttpPost("integrations")]
        public async Task<IActionResult> Integrations(IntegrationsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveIntegrationLink(model.SolutionId, model.Link);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }

        [HttpGet("roadmap")]
        public async Task<IActionResult> Roadmap(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, RoadmapModel>(solution));
        }

        [HttpPost("roadmap")]
        public async Task<IActionResult> Roadmap(RoadmapModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveRoadmap(model.SolutionId, model.Summary);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }
        
        [HttpGet("solution-description")]
        public async Task<IActionResult> SolutionDescription(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution));
        }

        [HttpPost("solution-description")]
        public async Task<IActionResult> SolutionDescription(SolutionDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveSolutionDescription(model.SolutionId, model.Summary, model.Description,
                model.Link);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }
    }
}
