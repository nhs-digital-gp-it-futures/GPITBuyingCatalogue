using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
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
    public async Task<IActionResult> AssociatedServices(int supplierId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var associatedServices = await associatedServicesService.GetAllAssociatedServicesForSupplier(supplierId);

        var model = new AssociatedServicesModel(supplier, associatedServices)
        {
            BackLink = Url.Action(
                nameof(SuppliersController.EditSupplier),
                typeof(SuppliersController).ControllerName(),
                new { supplierId }),
        };

        return View(model);
    }

    [HttpGet("add-associated-service")]
    public async Task<IActionResult> AddAssociatedService(int supplierId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var model = new AddAssociatedServiceModel(supplier)
        {
            BackLink = Url.Action(nameof(AssociatedServices), new { supplierId }),
        };

        return View(model);
    }

    [HttpPost("add-associated-service")]
    public async Task<IActionResult> AddAssociatedService(int supplierId, AddAssociatedServiceModel model)
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
            new { supplierId, associatedServiceId, });
    }

    [HttpGet("{associatedServiceId}/edit-associated-service")]
    public async Task<IActionResult> EditAssociatedService(
        int supplierId,
        CatalogueItemId associatedServiceId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var associatedService =
            await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
        if (associatedService is null)
            return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

        var relatedCatalogueItems = await associatedServicesService.GetAssociatedServiceReferences(associatedServiceId);
        var model = new EditAssociatedServiceModel(supplier, associatedService, relatedCatalogueItems)
        {
            BackLink = Url.Action(nameof(AssociatedServices), new { supplierId }),
        };

        return View(model);
    }

    [HttpPost("{associatedServiceId}/edit-associated-service")]
    public async Task<IActionResult> SetPublicationStatus(
        int supplierId,
        CatalogueItemId associatedServiceId,
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

        return RedirectToAction(nameof(AssociatedServices), new { supplierId });
    }

    [HttpGet("{associatedServiceId}/edit-associated-service-details")]
    public async Task<IActionResult> EditAssociatedServiceDetails(
        int supplierId,
        CatalogueItemId associatedServiceId)
    {
        var supplier = await suppliersService.GetSupplier(supplierId);
        if (supplier is null)
            return BadRequest($"No Supplier found for Id: {supplierId}");

        var associatedService =
            await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
        if (associatedService is null)
            return BadRequest($"No Associated Service found for Id: {associatedServiceId}");

        var solutionMergersAndSplits =
            await associatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(
                associatedServiceId);

        var model = new EditAssociatedServiceDetailsModel(supplier, associatedService, solutionMergersAndSplits)
        {
            BackLink = Url.Action(nameof(EditAssociatedService), new { supplierId, associatedServiceId }),
        };

        return View(model);
    }

    [HttpPost("{associatedServiceId}/edit-associated-service-details")]
    public async Task<IActionResult> EditAssociatedServiceDetails(
        int supplierId,
        CatalogueItemId associatedServiceId,
        EditAssociatedServiceDetailsModel model)
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
            new { supplierId, associatedServiceId, });
    }
}
