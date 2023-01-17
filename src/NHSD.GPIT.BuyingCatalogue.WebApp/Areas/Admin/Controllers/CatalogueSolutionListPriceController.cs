using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/list-prices")]
    public class CatalogueSolutionListPriceController : Controller
    {
        private readonly IListPriceService listPriceService;
        private readonly PriceTiersCapSettings priceTiersCapSettings;

        public CatalogueSolutionListPriceController(IListPriceService listPriceService, PriceTiersCapSettings tiersCapSettings)
        {
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
            priceTiersCapSettings = tiersCapSettings ?? throw new ArgumentNullException(nameof(tiersCapSettings));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new ManageListPricesModel(solution, solution.CataloguePrices)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
                AddListPriceUrl = Url.Action(
                    nameof(ListPriceType),
                    new { solutionId }),
            };

            return View("ListPrices/ManageListPrices", model);
        }

        [HttpGet("list-price-type")]
        public async Task<IActionResult> ListPriceType(CatalogueItemId solutionId, CataloguePriceType? selectedPriceType = null)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
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
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            AddTieredListPriceModel model = cataloguePriceId is not null
                ? new(solution, solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId))
                {
                    DeleteListPriceUrl = Url.Action(
                        nameof(DeleteListPrice),
                        new { solutionId, cataloguePriceId }),
                }
                : new(solution);

            model.BackLink = Url.Action(
                nameof(ListPriceType),
                new { solutionId });

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
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
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
                TimeUnit = model.GetBillingPeriod(),
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
                CataloguePriceQuantityCalculationType = model.GetQuantityCalculationType(),
            };

            await listPriceService.AddListPrice(solutionId, price);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new TieredPriceTiersModel(solution, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(nameof(AddTieredListPrice), new { solutionId, cataloguePriceId }),
                AddTieredPriceTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    typeof(CatalogueSolutionListPriceController).ControllerName(),
                    new { solutionId, cataloguePriceId }),
            };

            return View("ListPrices/TieredPriceTiers", model);
        }

        [HttpPost("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(CatalogueItemId solutionId, int cataloguePriceId, TieredPriceTiersModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
                var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

                model.Tiers = price.CataloguePriceTiers.ToList();

                model.MaximumNumberOfTiers = priceTiersCapSettings.MaximumNumberOfPriceTiers;

                return View("ListPrices/TieredPriceTiers", model);
            }

            await listPriceService.SetPublicationStatus(solutionId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, bool? isEditing)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
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

            await listPriceService.AddListPriceTier(solutionId, cataloguePriceId, priceTier);

            var actionName = GetAddOrEditBacklink(model.IsEditing);

            return RedirectToAction(actionName, new { solutionId, cataloguePriceId });
        }

        [HttpGet("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new EditTieredListPriceModel(solution, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
                AddPricingTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    new { solutionId, cataloguePriceId, isEditing = true }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { solutionId, cataloguePriceId }),
            };

            return View("ListPrices/EditTieredListPrice", model);
        }

        [HttpPost("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(CatalogueItemId solutionId, int cataloguePriceId, EditTieredListPriceModel model)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);

            if (!ModelState.IsValid)
            {
                model.Tiers = price.CataloguePriceTiers.ToList();
                model.MaximumNumberOfTiers = priceTiersCapSettings.MaximumNumberOfPriceTiers;

                return View("ListPrices/EditTieredListPrice", model);
            }

            await listPriceService.UpdateListPrice(
                    solutionId,
                    cataloguePriceId,
                    model.GetPricingUnit(),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            if (model.SelectedPublicationStatus != price.PublishedStatus)
                await listPriceService.SetPublicationStatus(solutionId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(solution, price)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { solutionId, cataloguePriceId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(CatalogueItemId solutionId, int cataloguePriceId, AddEditFlatListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddEditFlatListPrice", model);

            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);

            await listPriceService.UpdateListPrice(
                solutionId,
                cataloguePriceId,
                model.GetPricingUnit(),
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.GetBillingPeriod(),
                model.GetQuantityCalculationType(),
                model.Price!.Value);

            if (model.SelectedPublicationStatus!.Value != price.PublishedStatus)
                await listPriceService.SetPublicationStatus(solutionId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId, bool? isEditing = null)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var tier = price.CataloguePriceTiers.FirstOrDefault(p => p.Id == tierId);
            if (tier is null)
                return NotFound();

            var actionName = GetAddOrEditBacklink(isEditing);

            var model = new AddEditTieredPriceTierModel(solution, price, tier)
            {
                BackLink = Url.Action(actionName, new { solutionId, cataloguePriceId }),
                IsEditing = isEditing,
                DeleteTieredPriceTierUrl = Url.Action(
                    nameof(DeleteTieredPriceTier),
                    new { solutionId, cataloguePriceId, tierId, isEditing }),
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

            await listPriceService.UpdateListPriceTier(
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
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var tier = price.CataloguePriceTiers.FirstOrDefault(p => p.Id == tierId);
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

            await listPriceService.UpdateTierPrice(solutionId, cataloguePriceId, tierId, model.Price!.Value);

            return RedirectToAction(nameof(EditTieredListPrice), new { solutionId, cataloguePriceId });
        }

        [HttpGet("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            _ = cataloguePriceId;

            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);

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
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

            if (price.PublishedStatus != PublicationStatus.Published)
                await listPriceService.DeleteListPrice(solutionId, cataloguePriceId);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId, bool? isEditing = null)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);

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
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

            if (price.PublishedStatus == PublicationStatus.Published)
                return RedirectToAction(nameof(EditTieredListPrice), new { solutionId, cataloguePriceId });

            await listPriceService.DeletePriceTier(solutionId, cataloguePriceId, tierId);

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
                cataloguePrice = await listPriceService.UpdateListPrice(
                    solutionId,
                    model.CataloguePriceId!.Value,
                    model.GetPricingUnit(),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());
            }
            else
            {
                cataloguePrice = new CataloguePrice
                {
                    CataloguePriceType = CataloguePriceType.Tiered,
                    CataloguePriceCalculationType = model.SelectedCalculationType!.Value,
                    ProvisioningType = model.SelectedProvisioningType!.Value,
                    TimeUnit = model.GetBillingPeriod(),
                    PricingUnit = model.GetPricingUnit(),
                    CataloguePriceQuantityCalculationType = model.GetQuantityCalculationType(),
                    CurrencyCode = "GBP",
                };

                await listPriceService.AddListPrice(solutionId, cataloguePrice);
            }

            return cataloguePrice;
        }
    }
}
