using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/additional-services")]
    public sealed class AdditionalServicesController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IListPricesService listPricesService;
        private readonly ICapabilitiesService capabilitiesService;

        public AdditionalServicesController(
            ISolutionsService solutionsService,
            IAdditionalServicesService additionalServicesService,
            IListPricesService listPricesService,
            ICapabilitiesService capabilitiesService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
            this.listPricesService = listPricesService ?? throw new ArgumentNullException(nameof(listPricesService));
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

        [HttpPost("{additionalServiceId}/edit-additional-service")]
        public async Task<IActionResult> SetPublicationStatus(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, EditAdditionalServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    nameof(EditAdditionalService),
                    model);
            }

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (model.SelectedPublicationStatus == additionalService.PublishedStatus)
                return RedirectToAction(nameof(Index), new { solutionId });

            await additionalServicesService.SavePublicationStatus(solutionId, additionalServiceId, model.SelectedPublicationStatus);

            return RedirectToAction(nameof(Index), new { solutionId });
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

        [HttpGet("{additionalServiceId}/list-prices")]
        public async Task<IActionResult> ManageListPrices(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service found for Id: {additionalServiceId}");

            var model = new ManageListPricesModel(additionalService, solutionId)
            {
                BackLink = Url.Action(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { solutionId, additionalServiceId }),
            };

            return View(model);
        }

        [HttpGet("{additionalServiceId}/list-price/add-list-price")]
        public async Task<IActionResult> AddListPrice(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service found for Id: {additionalServiceId}");

            var model = new EditListPriceModel(additionalService)
            {
                Title = "Additional Service price",
                BackLink = Url.Action(
                    nameof(ManageListPrices),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { solutionId, additionalServiceId }),
            };

            return View("EditListPrice", model);
        }

        [HttpPost("{additionalServiceId}/list-price/add-list-price")]
        public async Task<IActionResult> AddListPrice(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, EditListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("EditListPrice", model);

            var provisioningType = model.SelectedProvisioningType!.Value;
            var saveListPriceModel = new SaveListPriceModel
            {
                Price = model.Price,
                ProvisioningType = provisioningType,
                PricingUnit = model.GetPricingUnit(),
                TimeUnit = model.GetTimeUnit(provisioningType),
            };

            await listPricesService.SaveListPrice(additionalServiceId, saveListPriceModel);

            return RedirectToAction(nameof(ManageListPrices), new { solutionId, additionalServiceId });
        }

        [HttpGet("{additionalServiceId}/list-price/{listPriceId}")]
        public async Task<IActionResult> EditListPrice(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, int listPriceId)
        {
            var additionalService = await listPricesService.GetCatalogueItemWithPrices(additionalServiceId);
            var cataloguePrice = additionalService.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId, additionalServiceId });

            var editListPriceModel = new EditListPriceModel(additionalService, cataloguePrice, solutionId)
            {
                BackLink = Url.Action(nameof(ManageListPrices), new { solutionId, additionalServiceId }),
                Title = $"{additionalService.Name} price",
            };

            return View("EditListPrice", editListPriceModel);
        }

        [HttpPost("{additionalServiceId}/list-price/{listPriceId}")]
        public async Task<IActionResult> EditListPrice(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, int listPriceId, EditListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("EditListPrice", model);

            var provisioningType = model.SelectedProvisioningType!.Value;
            var saveListPriceModel = new SaveListPriceModel
            {
                CataloguePriceId = listPriceId,
                Price = model.Price,
                ProvisioningType = provisioningType,
                PricingUnit = model.GetPricingUnit(),
                TimeUnit = model.GetTimeUnit(provisioningType),
            };

            await listPricesService.UpdateListPrice(additionalServiceId, saveListPriceModel);

            return RedirectToAction(nameof(ManageListPrices), new { solutionId, additionalServiceId });
        }

        [HttpGet("{additionalServiceId}/list-price/{listPriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, int listPriceId)
        {
            var additionalService = await listPricesService.GetCatalogueItemWithPrices(additionalServiceId);

            var model = new DeleteListPriceModel(additionalService)
            {
                BackLink = Url.Action(nameof(EditListPrice), new { solutionId, additionalServiceId, listPriceId }),
            };

            return View(model);
        }

        [HttpPost("{additionalServiceId}/list-price/{listPriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, CatalogueItemId additionalServiceId, int listPriceId, DeleteListPriceModel model)
        {
            _ = model;

            await listPricesService.DeleteListPrice(additionalServiceId, listPriceId);

            return RedirectToAction(nameof(ManageListPrices), new { solutionId, additionalServiceId });
        }

        [HttpGet("{additionalServiceId}/edit-capabilities")]
        public async Task<IActionResult> EditCapabilities(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var capabilities = await capabilitiesService.GetCapabilitiesByCategory();

            var model = new EditCapabilitiesModel(additionalService, capabilities)
            {
                BackLink = Url.Action(nameof(EditAdditionalService), new { solutionId, additionalServiceId }),
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
