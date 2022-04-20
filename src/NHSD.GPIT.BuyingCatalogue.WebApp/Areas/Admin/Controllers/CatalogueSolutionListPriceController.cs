using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/list-prices")]
    public class CatalogueSolutionListPriceController : Controller
    {
        private readonly ISolutionListPriceService solutionListPriceService;

        public CatalogueSolutionListPriceController(ISolutionListPriceService solutionListPriceService)
        {
            this.solutionListPriceService = solutionListPriceService ?? throw new ArgumentNullException(nameof(solutionListPriceService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new ManageListPricesModel(solution, solution.CataloguePrices)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View("ListPrices/ManageListPrices", model);
        }

        [HttpGet("list-price-type")]
        public async Task<IActionResult> ListPriceType(CatalogueItemId solutionId, CataloguePriceType? selectedPriceType = null)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new ListPriceTypeModel(solution)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
                SelectedCataloguePriceType = selectedPriceType,
            };

            return View("ListPrices/ListPriceType", model);
        }

        [HttpPost("list-price-type")]
        public IActionResult ListPriceType(CatalogueItemId solutionId, ListPriceTypeModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/ListPriceType", model);

            var redirectAction = model.SelectedCataloguePriceType switch
            {
                CataloguePriceType.Tiered => nameof(AddTieredListPrice),
                CataloguePriceType.Flat => nameof(AddFlatListPrice),
                _ => string.Empty,
            };

            return RedirectToAction(redirectAction, new { solutionId });
        }

        [HttpGet("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(CatalogueItemId solutionId, int? cataloguePriceId = null)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            AddTieredListPriceModel model = cataloguePriceId is not null
                ? new AddTieredListPriceModel(solution, solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId))
                : new AddTieredListPriceModel(solution);

            model.BackLink = Url.Action(nameof(ListPriceType), new { solutionId, selectedPriceType = CataloguePriceType.Tiered });

            return View("ListPrices/AddTieredListPrice", model);
        }

        [HttpPost("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(CatalogueItemId solutionId, AddTieredListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddTieredListPrice", model);

            var cataloguePrice = await AddOrUpdateCataloguePrice(solutionId, model);

            return RedirectToAction(nameof(TieredPriceTiers), new { solutionId, cataloguePriceId = model.CataloguePriceId ?? cataloguePrice.CataloguePriceId });
        }

        [HttpGet("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(CatalogueItemId solutionId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(solution)
            {
                BackLink = Url.Action(nameof(ListPriceType), new { solutionId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(CatalogueItemId solutionId, AddEditFlatListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddEditFlatListPrice", model);

            var price = new CataloguePrice
            {
                CataloguePriceType = CataloguePriceType.Flat,
                ProvisioningType = model.SelectedProvisioningType!.Value,
                TimeUnit = model.GetTimeUnit(),
                PricingUnit = model.GetPricingUnit(),
                CataloguePriceCalculationType = model.SelectedCalculationType!.Value,
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = model.Price!.Value,
                        LowerRange = 1,
                        UpperRange = null,
                    },
                },
                CurrencyCode = "GBP",
                PublishedStatus = model.SelectedPublicationStatus!.Value,
            };

            await solutionListPriceService.AddListPrice(solutionId, price);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new TieredPriceTiersModel(solution, price)
            {
                BackLink = Url.Action(nameof(AddTieredListPrice), new { solutionId, cataloguePriceId }),
            };

            return View("ListPrices/TieredPriceTiers", model);
        }

        [HttpPost("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(CatalogueItemId solutionId, int cataloguePriceId, TieredPriceTiersModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
                var price = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);

                model.Tiers = price.CataloguePriceTiers.ToList();

                return View("ListPrices/TieredPriceTiers", model);
            }

            await solutionListPriceService.SetPublicationStatus(solutionId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, bool? isEditing)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var actionName = GetAddOrEditBacklink(isEditing);

            var model = new AddEditTieredPriceTierModel(solution, price)
            {
                BackLink = Url.Action(actionName, new { solutionId, cataloguePriceId }),
                IsEditing = isEditing,
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, AddEditTieredPriceTierModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddEditTieredPriceTier", model);

            var priceTier = new CataloguePriceTier
            {
                LowerRange = model.LowerRange!.Value,
                UpperRange = !model.IsInfiniteRange!.Value ? model.UpperRange : null,
                Price = model.Price!.Value,
            };

            await solutionListPriceService.AddListPriceTier(solutionId, cataloguePriceId, priceTier);

            var actionName = GetAddOrEditBacklink(model.IsEditing);

            return RedirectToAction(actionName, new { solutionId, cataloguePriceId });
        }

        [HttpGet("{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new EditTieredListPriceModel(solution, price)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
            };

            return View("ListPrices/EditTieredListPrice", model);
        }

        [HttpPost("{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(CatalogueItemId solutionId, int cataloguePriceId, EditTieredListPriceModel model)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);

            if (!ModelState.IsValid)
            {
                model.Tiers = price.CataloguePriceTiers.ToList();

                return View("ListPrices/EditTieredListPrice", model);
            }

            await solutionListPriceService.UpdateListPrice(
                    solutionId,
                    cataloguePriceId,
                    model.GetPricingUnit(),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetTimeUnit());

            if (model.SelectedPublicationStatus != price.PublishedStatus)
                await solutionListPriceService.SetPublicationStatus(solutionId, cataloguePriceId, model.SelectedPublicationStatus);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId, bool? isEditing = null)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var tier = price.CataloguePriceTiers.SingleOrDefault(p => p.Id == tierId);
            if (tier is null)
                return NotFound();

            var actionName = GetAddOrEditBacklink(isEditing);

            var model = new AddEditTieredPriceTierModel(solution, price, tier)
            {
                BackLink = Url.Action(actionName, new { solutionId, cataloguePriceId }),
                IsEditing = isEditing,
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            AddEditTieredPriceTierModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddEditTieredPriceTier", model);

            await solutionListPriceService.UpdateListPriceTier(
                solutionId,
                cataloguePriceId,
                tierId,
                model.Price!.Value,
                model.LowerRange!.Value,
                model.IsInfiniteRange.HasValue && model.IsInfiniteRange!.Value ? null : model.UpperRange);

            var actionName = GetAddOrEditBacklink(model.IsEditing);

            return RedirectToAction(actionName, new { solutionId, cataloguePriceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(CatalogueItemId solutionId, int cataloguePriceId, int tierId, int tierIndex)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var tier = price.CataloguePriceTiers.SingleOrDefault(p => p.Id == tierId);
            if (tier is null)
                return NotFound();

            var model = new EditTierPriceModel(solution, price, tier)
            {
                BackLink = Url.Action(nameof(EditTieredListPrice), new { solutionId, cataloguePriceId }),
                TierIndex = tierIndex,
            };

            return View("ListPrices/EditTierPrice", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(CatalogueItemId solutionId, int cataloguePriceId, int tierId, EditTierPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/EditTierPrice", model);

            await solutionListPriceService.UpdateTierPrice(solutionId, cataloguePriceId, tierId, model.Price!.Value);

            return RedirectToAction(nameof(EditTieredListPrice), new { solutionId, cataloguePriceId });
        }

        [HttpGet("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            _ = cataloguePriceId;

            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                solution.Name,
                "This list price will be deleted")
            {
                BackLink = HttpContext.Request.Headers.Referer.ToString(),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int cataloguePriceId, DeleteItemConfirmationModel model)
        {
            await solutionListPriceService.DeleteListPrice(solutionId, cataloguePriceId);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId, bool? isEditing = null)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                solution.Name,
                "This pricing tier will be deleted")
            {
                BackLink = Url.Action(nameof(EditTieredPriceTier), new { solutionId, cataloguePriceId, tierId, isEditing = isEditing }),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId, DeleteItemConfirmationModel model, bool? isEditing = null)
        {
            await solutionListPriceService.DeletePriceTier(solutionId, cataloguePriceId, tierId);

            var actionName = GetAddOrEditBacklink(isEditing);
            return RedirectToAction(actionName, new { solutionId, cataloguePriceId });
        }

        private static string GetAddOrEditBacklink(bool? isEditing)
            => isEditing.GetValueOrDefault()
                ? nameof(EditTieredListPrice)
                : nameof(TieredPriceTiers);

        private async Task<CataloguePrice> AddOrUpdateCataloguePrice(CatalogueItemId solutionId, AddTieredListPriceModel model)
        {
            CataloguePrice cataloguePrice;
            if (model.CataloguePriceId is not null)
            {
                cataloguePrice = await solutionListPriceService.UpdateListPrice(
                    solutionId,
                    model.CataloguePriceId!.Value,
                    model.GetPricingUnit(),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetTimeUnit());
            }
            else
            {
                cataloguePrice = new CataloguePrice
                {
                    CataloguePriceType = CataloguePriceType.Tiered,
                    CataloguePriceCalculationType = model.SelectedCalculationType!.Value,
                    ProvisioningType = model.SelectedProvisioningType!.Value,
                    TimeUnit = model.GetTimeUnit(),
                    PricingUnit = model.GetPricingUnit(),
                    CurrencyCode = "GBP",
                };

                await solutionListPriceService.AddListPrice(solutionId, cataloguePrice);
            }

            return cataloguePrice;
        }
    }
}
