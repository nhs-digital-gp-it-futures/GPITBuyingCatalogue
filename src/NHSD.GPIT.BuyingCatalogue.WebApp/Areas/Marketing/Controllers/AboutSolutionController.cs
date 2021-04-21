using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class AboutSolutionController : Controller
    {
        private readonly ILogger<AboutSolutionController> _logger;
        private readonly ISolutionsService _solutionsService;

        public AboutSolutionController(ILogger<AboutSolutionController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
        
        [HttpGet("marketing/supplier/solution/{id}/section/solution-description")]
        public async Task<IActionResult> SolutionDescription(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDescriptionModel(solution);
            
            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/solution-description")]
        public async Task<IActionResult> SolutionDescription(SolutionDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveSolutionDescription(model.SolutionId, model.Summary, model.Description, model.Link);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/features")]
        public async Task<IActionResult> Features(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/features")]
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

            var jsonFeatures = JsonConvert.SerializeObject(features);

            await _solutionsService.SaveSolutionFeatures(model.SolutionId, jsonFeatures);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/integrations")]
        public async Task<IActionResult> Integrations(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new IntegrationsModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/integrations")]
        public async Task<IActionResult> Integrations(IntegrationsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveIntegrationLink(model.SolutionId, model.Link);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new ImplementationTimescalesModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(ImplementationTimescalesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveImplementationDetail(model.SolutionId, model.Description);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/roadmap")]
        public async Task<IActionResult> Roadmap(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new RoadmapModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/roadmap")]
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
