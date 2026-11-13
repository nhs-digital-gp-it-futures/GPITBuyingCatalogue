using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "ManageSupplierOrganisations")]
[Area("Admin")]
[Route("admin/manage-suppliers/{supplierId:int}/services")]
public class SupplierServicesController(
    ISuppliersService suppliersService,
    IAssociatedServicesService associatedServicesService,
    IPublicationStatusService publicationStatusService) : Controller
{
    private readonly ISuppliersService suppliersService =
        suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));

    private readonly IAssociatedServicesService associatedServicesService =
        associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));

    private readonly IPublicationStatusService publicationStatusService =
        publicationStatusService ?? throw new ArgumentNullException(nameof(publicationStatusService));

    [HttpGet]
    public async Task<IActionResult> AssociatedServices(
        [FromRoute] int supplierId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}, and solution type {solutionType}");

        var associatedServices = await associatedServicesService.GetAllAssociatedServicesForSupplier(supplierId);

        var model = new AssociatedServicesModel(supplier, associatedServices)
        {
            BackLink = solutionType is null && solutionId is null
                ? Url.Action(
                    nameof(SuppliersController.EditSupplier),
                    typeof(SuppliersController).ControllerName(),
                    new { supplierId })
                : additionalServiceId is not null && solutionId == additionalServiceId
                    ? Url.Action(
                        nameof(CatalogueSolutionsController.AssociatedServices),
                        typeof(CatalogueSolutionsController).ControllerName(),
                        new { solutionType, solutionId })
                    : Url.Action(
                        nameof(AdditionalServicesController.AssociatedServices),
                        typeof(AdditionalServicesController).ControllerName(),
                        new { solutionType, solutionId, additionalServiceId }),
            SolutionType = solutionType,
            SolutionId = solutionId,
            AdditionalServiceId = additionalServiceId,
        };

        return View(model);
    }

    [HttpGet("add-associated-service")]
    public async Task<IActionResult> AddAssociatedService(
        [FromRoute] int supplierId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var model = new AddAssociatedServiceModel(supplier)
        {
            BackLink = Url.Action(
                nameof(AssociatedServices),
                new { supplierId, solutionType, solutionId, additionalServiceId }),
        };

        return View(model);
    }

    [HttpPost("add-associated-service")]
    public async Task<IActionResult> AddAssociatedService(
        [FromRoute] int supplierId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId,
        AddAssociatedServiceModel model)
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

        var associatedServiceId = await associatedServicesService.AddAssociatedService(supplierId, newModel);

        return RedirectToAction(
            nameof(EditAssociatedService),
            new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
    }

    [HttpGet("{associatedServiceId}/edit-associated-service")]
    public async Task<IActionResult> EditAssociatedService(
        [FromRoute] int supplierId,
        [FromRoute] CatalogueItemId associatedServiceId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var associatedService =
            await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
        if (associatedService is null)
            return BadRequest($"No associated service found for Id: {associatedServiceId}");

        var relatedCatalogueItems = await associatedServicesService.GetAssociatedServiceReferences(associatedServiceId);
        var model = new EditAssociatedServiceModel(supplier, associatedService, relatedCatalogueItems)
        {
            BackLink = Url.Action(
                nameof(AssociatedServices),
                new { supplierId, solutionType, solutionId, additionalServiceId }),
            SolutionId = solutionId,
            AdditionalServiceId = additionalServiceId,
        };

        return View(model);
    }

    [HttpPost("{associatedServiceId}/edit-associated-service")]
    public async Task<IActionResult> SetPublicationStatus(
        [FromRoute] int supplierId,
        [FromRoute] CatalogueItemId associatedServiceId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId,
        EditAssociatedServiceModel model)
    {
        if (!ModelState.IsValid)
        {
            var relatedCatalogueItems =
                await associatedServicesService.GetAssociatedServiceReferences(associatedServiceId);

            model
                .WithSolutions(relatedCatalogueItems)
                .WithAdditionalServices(relatedCatalogueItems);

            return View("EditAssociatedService", model);
        }

        await publicationStatusService.SetPublicationStatus(associatedServiceId, model.SelectedPublicationStatus);

        return RedirectToAction(
            nameof(AssociatedServices),
            new { supplierId, solutionType, solutionId, additionalServiceId });
    }

    [HttpGet("{associatedServiceId}/edit-associated-service-details")]
    public async Task<IActionResult> EditAssociatedServiceDetails(
        [FromRoute] int supplierId,
        [FromRoute] CatalogueItemId associatedServiceId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var associatedService =
            await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
        if (associatedService is null)
            return BadRequest($"No associated service found for Id: {associatedServiceId}");

        var solutionMergersAndSplits =
            await associatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(
                associatedServiceId);

        var model = new EditAssociatedServiceDetailsModel(supplier, associatedService, solutionMergersAndSplits)
        {
            BackLink = Url.Action(
                nameof(EditAssociatedService),
                new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
        };

        return View(model);
    }

    [HttpPost("{associatedServiceId}/edit-associated-service-details")]
    public async Task<IActionResult> EditAssociatedServiceDetails(
        [FromRoute] int supplierId,
        [FromRoute] CatalogueItemId associatedServiceId,
        [FromQuery] SolutionType? solutionType,
        [FromQuery] CatalogueItemId? solutionId,
        [FromQuery] CatalogueItemId? additionalServiceId,
        EditAssociatedServiceDetailsModel model)
    {
        var associatedService = await associatedServicesService.GetAssociatedService(associatedServiceId);
        if (associatedService is null)
            return BadRequest($"No associated service found for Id: {associatedServiceId}");

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
            new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
    }
}
