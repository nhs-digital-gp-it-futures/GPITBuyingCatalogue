using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/list-prices")]
    public sealed class ListPriceController : Controller
    {
        private readonly IListPricesService listPricesService;

        public ListPriceController(IListPricesService listPricesService) =>
            this.listPricesService = listPricesService ?? throw new ArgumentNullException(nameof(listPricesService));

        [HttpGet]
        public async Task<IActionResult> ManageListPrices(CatalogueItemId solutionId)
        {
            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ManageListPricesModel(solution, solutionId)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
                AddLink = Url.Action(
                    nameof(AddListPrice),
                    typeof(ListPriceController).ControllerName(),
                    new { solutionId }),
                EditPriceStatusActionName = nameof(EditListPriceStatus),
                EditPriceActionName = nameof(EditListPrice),
                ControllerName = typeof(ListPriceController).ControllerName(),
            };

            return View(model);
        }

        [HttpGet("add-list-price")]
        public async Task<IActionResult> AddListPrice(CatalogueItemId solutionId)
        {
            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new EditListPriceModel(solution)
            {
                BackLink = Url.Action(nameof(ManageListPrices), new { solutionId }),
            };

            return View("EditListPrice", model);
        }

        [HttpPost("add-list-price")]
        public async Task<IActionResult> AddListPrice(CatalogueItemId solutionId, EditListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("EditListPrice", model);

            var provisioningType = model.SelectedProvisioningType!.Value;
            var saveSolutionListPriceModel = new SaveListPriceModel
            {
                Price = model.Price,
                ProvisioningType = provisioningType,
                PricingUnit = model.GetPricingUnit(),
                TimeUnit = model.GetTimeUnit(provisioningType),
            };

            var listPriceId = await listPricesService.SaveListPrice(solutionId, saveSolutionListPriceModel);

            return RedirectToAction(nameof(PublishListPrice), new { solutionId, listPriceId });
        }

        [HttpGet("{listPriceId}")]
        public async Task<IActionResult> EditListPrice(CatalogueItemId solutionId, int listPriceId)
        {
            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId });

            if (cataloguePrice.IsLocked)
                return RedirectToAction(nameof(EditListPriceStatus), new { solutionId, listPriceId });

            var editListPriceModel = new EditListPriceModel(solution, cataloguePrice)
            {
                BackLink = Url.Action(nameof(ManageListPrices), new { solutionId }),
                DeleteLink = Url.Action(nameof(DeleteListPrice), new { solutionId, listPriceId }),
            };

            return View("EditListPrice", editListPriceModel);
        }

        [HttpPost("{listPriceId}")]
        public async Task<IActionResult> EditListPrice(CatalogueItemId solutionId, int listPriceId, EditListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("EditListPrice", model);

            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId });

            if (cataloguePrice.IsLocked)
                throw new ArgumentException($"List price {listPriceId} cannot be edited due to being locked");

            var provisioningType = model.SelectedProvisioningType!.Value;
            var saveSolutionListPriceModel = new SaveListPriceModel
            {
                CataloguePriceId = listPriceId,
                Price = model.Price,
                ProvisioningType = provisioningType,
                PricingUnit = model.GetPricingUnit(),
                TimeUnit = model.GetTimeUnit(provisioningType),
            };

            await listPricesService.UpdateListPrice(solutionId, saveSolutionListPriceModel);

            return RedirectToAction(nameof(PublishListPrice), new { solutionId, listPriceId });
        }

        [HttpGet("{listPriceId}/publish-list-price")]
        public async Task<IActionResult> PublishListPrice(CatalogueItemId solutionId, int listPriceId)
        {
            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId });

            var editListPriceModel = new EditListPriceStatus(solution, cataloguePrice)
            {
                BackLink = Url.Action(nameof(EditListPrice), new { solutionId, listPriceId }),
                Title = "Publish list price",
                Advice = "Are you sure you want to publish this list price? Once you do, you'll no longer be able to edit or delete it.",
            };

            return View("EditListPriceStatus", editListPriceModel);
        }

        [HttpPost("{listPriceId}/publish-list-price")]
        public async Task<IActionResult> PublishListPrice(CatalogueItemId solutionId, int listPriceId, EditListPriceStatus model)
        {
            if (!ModelState.IsValid)
                return View("EditListPriceStatus", model);

            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId });

            if (cataloguePrice.IsLocked)
                return RedirectToAction(nameof(EditListPriceStatus), new { solutionId, listPriceId });

            var saveSolutionListPriceModel = new SaveListPriceModel
            {
                Status = model.Status,
                CataloguePriceId = listPriceId,
            };

            await listPricesService.UpdateListPriceStatus(solutionId, saveSolutionListPriceModel);

            return RedirectToAction(nameof(ManageListPrices), new { solutionId });
        }

        [HttpGet("{listPriceId}/edit-status")]
        public async Task<IActionResult> EditListPriceStatus(CatalogueItemId solutionId, int listPriceId)
        {
            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId });

            var editListPriceModel = new EditListPriceStatus(solution, cataloguePrice)
            {
                BackLink = Url.Action(nameof(ManageListPrices), new { solutionId }),
                Title = "Edit list price",
                Advice = "Change the publication status for this list price.",
            };

            return View("EditListPriceStatus", editListPriceModel);
        }

        [HttpPost("{listPriceId}/edit-status")]
        public async Task<IActionResult> EditListPriceStatus(CatalogueItemId solutionId, int listPriceId, EditListPriceStatus model)
        {
            if (!ModelState.IsValid)
                return View("EditListPriceStatus", model);

            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(ManageListPrices), new { solutionId });

            if (!cataloguePrice.IsLocked)
                return RedirectToAction(nameof(EditListPriceStatus), new { solutionId, listPriceId });

            var saveSolutionListPriceModel = new SaveListPriceModel
            {
                Status = model.Status,
                CataloguePriceId = listPriceId,
            };

            await listPricesService.UpdateListPriceStatus(solutionId, saveSolutionListPriceModel);

            return RedirectToAction(nameof(ManageListPrices), new { solutionId });
        }

        [HttpGet("{listPriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int listPriceId)
        {
            var solution = await listPricesService.GetCatalogueItemWithPrices(solutionId);

            var model = new DeleteListPriceModel(solution)
            {
                BackLink = Url.Action(nameof(EditListPrice), new { solutionId, listPriceId }),
            };

            return View(model);
        }

        [HttpPost("{listPriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int listPriceId, DeleteListPriceModel model)
        {
            _ = model;

            await listPricesService.DeleteListPrice(solutionId, listPriceId);

            return RedirectToAction(nameof(ManageListPrices), new { solutionId });
        }
    }
}
