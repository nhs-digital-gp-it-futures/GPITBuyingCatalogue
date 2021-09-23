using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/list-prices")]
    public sealed class ListPriceController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public ListPriceController(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet()]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ManageListPricesModel(solution)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
                BackLinkText = "Go back",
            };

            return View(model);
        }

        [HttpGet("add-list-price")]
        public async Task<IActionResult> AddListPrice(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new EditListPriceModel(solution)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
                BackLinkText = "Go back",
            };

            return View("EditListPrice", model);
        }

        [HttpPost("add-list-price")]
        public async Task<IActionResult> AddListPrice(CatalogueItemId solutionId, EditListPriceModel model)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            DoModelStateValidation(solution, model, out var price, out var provisioningType);

            if (!ModelState.IsValid)
                return View("EditListPrice", model);

            var saveSolutionListPriceModel = new SaveSolutionListPriceModel
            {
                Price = price,
                ProvisioningType = provisioningType,
                PricingUnit = model.GetPricingUnit(),
                TimeUnit = model.GetTimeUnit(provisioningType),
            };

            await solutionsService.SaveSolutionListPrice(solutionId, saveSolutionListPriceModel);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{listPriceId}")]
        public async Task<IActionResult> EditListPrice(CatalogueItemId solutionId, int listPriceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            var cataloguePrice = solution.CataloguePrices.FirstOrDefault(cp => cp.CataloguePriceId == listPriceId);
            if (cataloguePrice is null)
                return RedirectToAction(nameof(Index), new { solutionId });

            var editListPriceModel = new EditListPriceModel(solution, cataloguePrice)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
                BackLinkText = "Go back",
            };

            return View("EditListPrice", editListPriceModel);
        }

        [HttpPost("{listPriceId}")]
        public async Task<IActionResult> EditListPrice(CatalogueItemId solutionId, int listPriceId, EditListPriceModel model)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            DoModelStateValidation(solution, model, out var price, out var provisioningType);

            if (!ModelState.IsValid)
                return View("EditListPrice", model);

            var saveSolutionListPriceModel = new SaveSolutionListPriceModel
            {
                CataloguePriceId = listPriceId,
                Price = price,
                ProvisioningType = provisioningType,
                PricingUnit = model.GetPricingUnit(),
                TimeUnit = model.GetTimeUnit(provisioningType),
            };

            await solutionsService.UpdateSolutionListPrice(solutionId, saveSolutionListPriceModel);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{listPriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int listPriceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            var model = new DeleteListPriceModel(solution)
            {
                BackLink = Url.Action(nameof(EditListPrice), new { solutionId, listPriceId }),
                BackLinkText = "Go back",
            };

            return View(model);
        }

        [HttpPost("{listPriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int listPriceId, DeleteListPriceModel model)
        {
            await solutionsService.DeleteSolutionListPrice(solutionId, listPriceId);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        private static bool IsPricingUnitDuplicateOfExistingPricingUnit(
            IEnumerable<CataloguePrice> cataloguePrices,
            decimal? price,
            ProvisioningType provisioningType,
            EditListPriceModel model)
            => cataloguePrices.Any(cp =>
                cp.PricingUnit.Description == model.Unit
                && cp.Price == price
                && cp.ProvisioningType == provisioningType
                && cp.TimeUnit == model.GetTimeUnit(provisioningType));

        private void DoModelStateValidation(
            CatalogueItem solution,
            EditListPriceModel model,
            out decimal price,
            out ProvisioningType provisioningType)
        {
            if (!model.TryParsePrice(out price))
                ModelState.AddModelError(nameof(model.Price), "A valid price must be entered.");

            if (!model.TryGetProvisioningType(out provisioningType))
                ModelState.AddModelError("edit-list-price", "A provisioning type must be selected.");

            if (model.DeclarativeTimeUnit is null && provisioningType == ProvisioningType.Declarative)
            {
                ModelState.AddModelError(nameof(model.DeclarativeTimeUnit), "A time unit must be specified when using a Declarative Provisioning type.");
                return;
            }

            if (model.OnDemandTimeUnit is null && provisioningType == ProvisioningType.OnDemand)
            {
                ModelState.AddModelError(nameof(model.OnDemandTimeUnit), "A time unit must be specified when using an On-Demand Provisioning type.");
                return;
            }

            if (IsPricingUnitDuplicateOfExistingPricingUnit(
                solution.CataloguePrices,
                price,
                provisioningType,
                model))
                ModelState.AddModelError("duplicate", "A list price with these details already exists for this Catalogue Solution.");
        }
    }
}
