using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class HostingTypeController : Controller
    {
        private readonly ILogWrapper<HostingTypeController> logger;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public HostingTypeController(
            ILogWrapper<HostingTypeController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"hosting-type-public-cloud-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, PublicCloudModel>(solution));
        }

        [HttpPost("hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(PublicCloudModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(model.SolutionId);
            if (hosting == null)
                return BadRequest($"No Hosting found for Solution Id: {model.SolutionId}");

            hosting.PublicCloud = model.PublicCloud;

            await solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"hosting-type-private-cloud-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, PrivateCloudModel>(solution));
        }

        [HttpPost("hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(PrivateCloudModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(model.SolutionId);
            if (hosting == null)
                return BadRequest($"No Hosting found for Solution Id: {model.SolutionId}");

            hosting.PrivateCloud = model.PrivateCloud;

            await solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"hosting-type-hybrid-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, HybridModel>(solution));
        }

        [HttpPost("hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(HybridModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(model.SolutionId);
            if (hosting == null)
                return BadRequest($"No Hosting found for Solution Id: {model.SolutionId}");

            hosting.HybridHostingType = model.HybridHostingType;

            await solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"hosting-type-onpremise-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, OnPremiseModel>(solution));
        }

        [HttpPost("hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(OnPremiseModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(model.SolutionId);
            if (hosting == null)
                return BadRequest($"No Hosting found for Solution Id: {model.SolutionId}");

            hosting.OnPremise = model.OnPremise;

            await solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }
    }
}
