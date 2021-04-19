using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class SolutionController : Controller
    {
        private readonly ILogger<SolutionController> _logger;
        private readonly ISolutionsService _solutionsService;

        public SolutionController(ILogger<SolutionController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        [Route("marketing/supplier/solution/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/preview")]
        public IActionResult Preview(string id)
        {
            return RedirectToAction("preview", "solutions", new { id = id });
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
            await _solutionsService.SaveSolutionDescription(model.Id, model.Summary, model.Description, model.Link);

            return RedirectToAction("Index", new { id = model.Id });
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

            await _solutionsService.SaveSolutionFeatures(model.Id, jsonFeatures);

            return RedirectToAction("Index", new { id = model.Id });
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
            await _solutionsService.SaveIntegrationLink(model.Id, model.Link);

            return RedirectToAction("Index", new { id = model.Id });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new ImplementationTimescalesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/roadmap")]
        public async Task<IActionResult> Roadmap(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new RoadmapModel(solution);

            return View(model);
        }


    }
}
