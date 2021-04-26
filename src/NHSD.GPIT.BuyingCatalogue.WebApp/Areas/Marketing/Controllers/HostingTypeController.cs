﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class HostingTypeController : Controller
    {
        private readonly ILogWrapper<HostingTypeController> _logger;
        private readonly ISolutionsService _solutionsService;

        public HostingTypeController(ILogWrapper<HostingTypeController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new PublicCloudModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(PublicCloudModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.PublicCloud = model.PublicCloud;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new PrivateCloudModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(PrivateCloudModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.PrivateCloud = model.PrivateCloud;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new HybridModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(HybridModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.HybridHostingType = model.HybridHostingType;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new OnPremiseModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(OnPremiseModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.OnPremise = model.OnPremise;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }        
    }
}
