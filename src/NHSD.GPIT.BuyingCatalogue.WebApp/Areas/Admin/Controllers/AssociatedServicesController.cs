using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CatalogueItems;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/associated-services")]
    public sealed class AssociatedServicesController(
        ICatalogueItemService catalogueItemsService,
        IAssociatedServicesService associatedServicesService,
        IPublicationStatusService publicationStatusService,
        ISuppliersService suppliersService)
        : Controller
    {
        private readonly ICatalogueItemService catalogueItemsService = catalogueItemsService ?? throw new ArgumentNullException(nameof(catalogueItemsService));
        private readonly IAssociatedServicesService associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
        private readonly IPublicationStatusService publicationStatusService = publicationStatusService ?? throw new ArgumentNullException(nameof(publicationStatusService));
        private readonly ISuppliersService suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));

        [HttpGet]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var catalogueItem = await catalogueItemsService.GetCatalogueItemWithSupplierServiceAssociations(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var associatedServices = await associatedServicesService.GetAllAssociatedServicesForSupplier(catalogueItem.Supplier.Id);

            var model = new AssociatedServicesModel(catalogueItem, associatedServices)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId, AssociatedServicesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.SelectableAssociatedServices is null)
            {
                return RedirectToAction(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId });
            }

            var associatedServices = model.SelectableAssociatedServices.Where(a => a.Selected).Select(a => a.CatalogueItemId);
            await associatedServicesService.RelateAssociatedServicesToSolution(solutionId, associatedServices);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("add-associated-service")]
        public async Task<IActionResult> AddAssociatedService(CatalogueItemId solutionId)
        {
            var supplier = await suppliersService.GetSupplier(solutionId.SupplierId);
            if (supplier is null)
                return BadRequest($"No Supplier found for Id: {solutionId.SupplierId}");

            var model = new AddAssociatedServiceModel(supplier)
            {
                BackLink = Url.Action(nameof(AssociatedServices), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("add-associated-service")]
        public async Task<IActionResult> AddAssociatedService(CatalogueItemId solutionId, AddAssociatedServiceModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var newModel = new AssociatedServicesDetailsModel
            {
                Name = model.Name,
                Description = model.Description,
                OrderGuidance = model.OrderGuidance,
                UserId = User.UserId(),
                PracticeReorganisationType = model.PracticeReorganisation,
            };

            var associatedServiceId = await associatedServicesService.AddAssociatedService(solutionId, newModel);

            return RedirectToAction(
                nameof(EditAssociatedService),
                new
                {
                    solutionId,
                    associatedServiceId,
                });
        }

        [HttpGet("{associatedServiceId}/edit-associated-service")]
        public async Task<IActionResult> EditAssociatedService(CatalogueItemId solutionId, CatalogueItemId associatedServiceId)
        {
            var supplier = await suppliersService.GetSupplier(solutionId.SupplierId);
            if (supplier is null)
                return BadRequest($"No Supplier found for Id: {solutionId.SupplierId}");

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

            var relatedSolutions = await associatedServicesService.GetAllSolutionsForAssociatedService(associatedServiceId);
            var model = new EditAssociatedServiceModel(supplier, solutionId, associatedService, relatedSolutions)
            {
                BackLink = Url.Action(nameof(AssociatedServices), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("{associatedServiceId}/edit-associated-service")]
        public async Task<IActionResult> SetPublicationStatus(CatalogueItemId solutionId, CatalogueItemId associatedServiceId, EditAssociatedServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                var relatedSolutions = await associatedServicesService.GetAllSolutionsForAssociatedService(associatedServiceId);
                model.RelatedSolutions = relatedSolutions;

                return View("EditAssociatedService", model);
            }

            await publicationStatusService.SetPublicationStatus(associatedServiceId, model.SelectedPublicationStatus);

            return RedirectToAction(nameof(AssociatedServices), new { solutionId });
        }

        [HttpGet("{associatedServiceId}/edit-associated-service-details")]
        public async Task<IActionResult> EditAssociatedServiceDetails(CatalogueItemId solutionId, CatalogueItemId associatedServiceId)
        {
            var supplier = await suppliersService.GetSupplier(solutionId.SupplierId);
            if (supplier is null)
                return BadRequest($"No Supplier found for Id: {solutionId.SupplierId}");

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

            var solutionMergersAndSplits = await associatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(associatedServiceId);

            var model = new EditAssociatedServiceDetailsModel(supplier, associatedService, solutionMergersAndSplits)
            {
                BackLink = Url.Action(nameof(EditAssociatedService), new { solutionId, associatedServiceId }),
            };

            return View(model);
        }

        [HttpPost("{associatedServiceId}/edit-associated-service-details")]
        public async Task<IActionResult> EditAssociatedServiceDetails(CatalogueItemId solutionId, CatalogueItemId associatedServiceId, EditAssociatedServiceDetailsModel model)
        {
            var associatedService = await associatedServicesService.GetAssociatedService(associatedServiceId);
            if (associatedService is null)
                return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

            if (!ModelState.IsValid)
                return View(model);

            await associatedServicesService.EditDetails(
                associatedServiceId,
                new AssociatedServicesDetailsModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    OrderGuidance = model.OrderGuidance,
                    UserId = User.UserId(),
                    PracticeReorganisationType = model.PracticeReorganisation,
                });

            return RedirectToAction(
                nameof(EditAssociatedService),
                new
                {
                    solutionId,
                    associatedServiceId,
                });
        }
    }
}
