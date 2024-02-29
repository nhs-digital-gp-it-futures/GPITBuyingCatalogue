using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/additional-services")]
    public sealed class AdditionalServicesController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly ICapabilitiesService capabilitiesService;
        private readonly IPublicationStatusService publicationStatusService;

        public AdditionalServicesController(
            ISolutionsService solutionsService,
            IAdditionalServicesService additionalServicesService,
            ICapabilitiesService capabilitiesService,
            IPublicationStatusService publicationStatusService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
            this.publicationStatusService = publicationStatusService ?? throw new ArgumentNullException(nameof(publicationStatusService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(solutionId);

            return View(new AdditionalServicesModel(solution, additionalServices));
        }

        [HttpGet("{additionalServiceId}/edit-additional-service")]
        public async Task<IActionResult> EditAdditionalService(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
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

        [HttpPost("{additionalServiceId}/edit-additional-service")]
        public async Task<IActionResult> SetPublicationStatus(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, EditAdditionalServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    nameof(EditAdditionalService),
                    model);
            }

            await publicationStatusService.SetPublicationStatus(additionalServiceId, model.SelectedPublicationStatus);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("add-additional-service")]
        public async Task<IActionResult> AddAdditionalService(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
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

            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalServiceDetailsModel = new AdditionalServicesDetailsModel
            {
                Id = model.Id.GetValueOrDefault(),
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
            var solution = await solutionsService.GetSolutionThin(solutionId);
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

            var solution = await solutionsService.GetSolutionThin(solutionId);
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

        [HttpGet("{additionalServiceId}/edit-capabilities")]
        public async Task<IActionResult> EditCapabilities(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var capabilities = await capabilitiesService.GetCapabilitiesByCategory();

            var model = new EditCapabilitiesModel(additionalService, capabilities)
            {
                BackLink = Url.Action(nameof(EditAdditionalService), new { solutionId, additionalServiceId }),
                SolutionName = solution.Name,
            };

            return View(model);
        }

        [HttpPost("{additionalServiceId}/edit-capabilities")]
        public async Task<IActionResult> EditCapabilities(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, EditCapabilitiesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var saveRequestModel = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = User.UserId(),
                Capabilities = model.CapabilityCategories
                    .SelectMany(cc => cc.Capabilities.Where(c => c.Selected))
                    .ToDictionary(c => c.Id, c => c.Epics.Where(e => e.Selected).Select(e => e.Id).ToArray()),
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(additionalServiceId, saveRequestModel);

            return RedirectToAction(nameof(EditAdditionalService), new { solutionId, additionalServiceId });
        }
    }
}
