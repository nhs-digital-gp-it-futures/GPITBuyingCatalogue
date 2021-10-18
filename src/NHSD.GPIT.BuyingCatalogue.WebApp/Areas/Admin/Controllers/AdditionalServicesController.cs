using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/additional-services")]
    public sealed class AdditionalServicesController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAdditionalServicesService additionalServicesService;

        public AdditionalServicesController(
            ISolutionsService solutionsService,
            IAdditionalServicesService additionalServicesService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(solutionId);

            return View(new AdditionalServicesModel(solution, additionalServices));
        }

        [HttpGet("{additionalServiceId}/edit-additional-service")]
        public async Task<IActionResult> EditAdditionalService(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var model = new EditAdditionalServiceModel(solution, additionalService)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("add-additional-service")]
        public async Task<IActionResult> AddAdditionalService(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new EditAdditionalServiceDetailsModel(solution)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
            };

            return View("EditAdditionalServiceDetails", model);
        }

        [HttpPost("add-additional-service")]
        public async Task<IActionResult> AddAdditionalService(CatalogueItemId solutionId, EditAdditionalServiceDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View("EditAdditionalServiceDetails", model);

            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalServiceDetailsModel = new AdditionalServicesDetailsModel
            {
                Name = model.Name,
                Description = model.Description,
                UserId = User.UserId(),
            };

            var additionalServiceId = await additionalServicesService.AddAdditionalService(solution, additionalServiceDetailsModel);

            return RedirectToAction(nameof(EditAdditionalService), new { solutionId, additionalServiceId });
        }

        [HttpGet("{additionalServiceId}/edit-additional-service-details")]
        public async Task<IActionResult> EditAdditionalServiceDetails(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var model = new EditAdditionalServiceDetailsModel(solution, additionalService)
            {
                BackLink = Url.Action(nameof(EditAdditionalService), new { solutionId, additionalServiceId }),
            };

            return View(model);
        }

        [HttpPost("{additionalServiceId}/edit-additional-service-details")]
        public async Task<IActionResult> EditAdditionalServiceDetails(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, EditAdditionalServiceDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var additionalServiceDetailsModel = new AdditionalServicesDetailsModel
            {
                Name = model.Name,
                Description = model.Description,
                UserId = User.UserId(),
            };

            await additionalServicesService.EditAdditionalService(solutionId, additionalServiceId, additionalServiceDetailsModel);

            return RedirectToAction(nameof(EditAdditionalService), new { solutionId, additionalServiceId });
        }
    }
}
