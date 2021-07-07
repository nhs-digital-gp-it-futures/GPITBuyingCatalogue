using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section")]
    public sealed class HostingTypeController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public HostingTypeController(
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, PublicCloudModel>(solution));
        }

        [HttpPost("hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(CatalogueItemId solutionId, PublicCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            if (hosting is null)
                return BadRequest($"No Hosting found for Solution Id: {solutionId}");

            hosting.PublicCloud = model.PublicCloud;

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, PrivateCloudModel>(solution));
        }

        [HttpPost("hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(CatalogueItemId solutionId, PrivateCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            if (hosting is null)
                return BadRequest($"No Hosting found for Solution Id: {solutionId}");

            hosting.PrivateCloud = model.PrivateCloud;

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, HybridModel>(solution));
        }

        [HttpPost("hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(CatalogueItemId solutionId, HybridModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            if (hosting is null)
                return BadRequest($"No Hosting found for Solution Id: {solutionId}");

            hosting.HybridHostingType = model.HybridHostingType;

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, OnPremiseModel>(solution));
        }

        [HttpPost("hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(CatalogueItemId solutionId, OnPremiseModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            if (hosting is null)
                return BadRequest($"No Hosting found for Solution Id: {solutionId}");

            hosting.OnPremise = model.OnPremise;

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }
    }
}
