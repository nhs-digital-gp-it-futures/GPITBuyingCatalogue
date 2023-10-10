using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/associated-services")]
    public sealed class AssociatedServicesController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IPublicationStatusService publicationStatusService;

        public AssociatedServicesController(
            ISolutionsService solutionsService,
            IAssociatedServicesService associatedServicesService,
            IPublicationStatusService publicationStatusService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.publicationStatusService = publicationStatusService ?? throw new ArgumentNullException(nameof(publicationStatusService));
        }

        [HttpGet]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithServiceAssociations(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var associatedServices = await associatedServicesService.GetAllAssociatedServicesForSupplier(solution.Supplier.Id);

            var model = new AssociatedServicesModel(solution, associatedServices)
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

            if (model.SelectableAssociatedServices is not null)
            {
                var associatedServices = model.SelectableAssociatedServices.Where(a => a.Selected).Select(a => a.CatalogueItemId);
                await associatedServicesService.RelateAssociatedServicesToSolution(solutionId, associatedServices);
            }

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("add-associated-service")]
        public async Task<IActionResult> AddAssociatedService(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AddAssociatedServiceModel(solution)
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

            var solution = await solutionsService.GetSolutionThin(solutionId);

            var newModel = new AssociatedServicesDetailsModel
            {
                Name = model.Name,
                Description = model.Description,
                OrderGuidance = model.OrderGuidance,
                UserId = User.UserId(),
                PracticeReorganisationType = model.PracticeReorganisation,
            };

            var associatedServiceId = await associatedServicesService.AddAssociatedService(solution, newModel);

            return RedirectToAction(
                nameof(EditAssociatedService),
                new
                {
                    solutionId = solution.Id,
                    associatedServiceId,
                });
        }

        [HttpGet("{associatedServiceId}/edit-associated-service")]
        public async Task<IActionResult> EditAssociatedService(CatalogueItemId solutionId, CatalogueItemId associatedServiceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

            var relatedSolutions = await associatedServicesService.GetAllSolutionsForAssociatedService(associatedServiceId);
            var model = new EditAssociatedServiceModel(solution, associatedService, relatedSolutions)
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
                var solution = await solutionsService.GetSolutionThin(solutionId);
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
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var associatedService = await associatedServicesService.GetAssociatedService(associatedServiceId);
            if (associatedService is null)
                return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

            var solutionMergersAndSplits = await associatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(associatedServiceId);

            var model = new EditAssociatedServiceDetailsModel(solution.SupplierId, solution.Supplier.Name, associatedService, solutionMergersAndSplits)
            {
                BackLink = Url.Action(nameof(EditAssociatedService), new { solutionId, associatedServiceId }),
            };

            return View(model);
        }

        [HttpPost("{associatedServiceId}/edit-associated-service-details")]
        public async Task<IActionResult> EditAssociatedServiceDetails(CatalogueItemId solutionId, CatalogueItemId associatedServiceId, EditAssociatedServiceDetailsModel model)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

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
