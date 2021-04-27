using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly ISolutionsService _solutionsService;

        public AboutSolutionController(ILogWrapper<AboutSolutionController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
        
        [HttpGet("solution-description")]
        public async Task<IActionResult> SolutionDescription(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
                        
            return View(new SolutionDescriptionModel(solution));
        }

        [HttpPost("solution-description")]
        public async Task<IActionResult> SolutionDescription(SolutionDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveSolutionDescription(model.SolutionId, model.Summary, model.Description, model.Link);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("features")]
        public async Task<IActionResult> Features(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new FeaturesModel(solution));
        }

        [HttpPost("features")]
        public async Task<IActionResult> Features(FeaturesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var features = new List<string>();

            if (!string.IsNullOrEmpty(model.Listing1)) features.Add(model.Listing1);
            if (!string.IsNullOrEmpty(model.Listing2)) features.Add(model.Listing2);
            if (!string.IsNullOrEmpty(model.Listing3)) features.Add(model.Listing3);
            if (!string.IsNullOrEmpty(model.Listing4)) features.Add(model.Listing4);
            if (!string.IsNullOrEmpty(model.Listing5)) features.Add(model.Listing5);
            if (!string.IsNullOrEmpty(model.Listing6)) features.Add(model.Listing6);
            if (!string.IsNullOrEmpty(model.Listing7)) features.Add(model.Listing7);
            if (!string.IsNullOrEmpty(model.Listing8)) features.Add(model.Listing8);
            if (!string.IsNullOrEmpty(model.Listing9)) features.Add(model.Listing9);
            if (!string.IsNullOrEmpty(model.Listing10)) features.Add(model.Listing10);
            
            await _solutionsService.SaveSolutionFeatures(model.SolutionId, features.ToArray());

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("integrations")]
        public async Task<IActionResult> Integrations(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new IntegrationsModel(solution));
        }

        [HttpPost("integrations")]
        public async Task<IActionResult> Integrations(IntegrationsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveIntegrationLink(model.SolutionId, model.Link);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new ImplementationTimescalesModel(solution));
        }

        [HttpPost("implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(ImplementationTimescalesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveImplementationDetail(model.SolutionId, model.Description);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("roadmap")]
        public async Task<IActionResult> Roadmap(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new RoadmapModel(solution));
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
    }
}
