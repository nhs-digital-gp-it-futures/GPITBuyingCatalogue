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
    [Authorize(Policy = "ManageCatalogueSolutions")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/list-prices")]
    public class CatalogueSolutionListPriceController(IListPriceService listPriceService, PriceTiersCapSettings tiersCapSettings) : Controller
    {
        private readonly IListPriceService listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
        private readonly PriceTiersCapSettings priceTiersCapSettings = tiersCapSettings ?? throw new ArgumentNullException(nameof(tiersCapSettings));

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new ManageListPricesModel(solution, solution.CataloguePrices)
            {
                SolutionType = solutionType,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionType, solutionId }),
                AddListPriceUrl = Url.Action(
                    nameof(ListPriceType),
                    new { solutionId, solutionType, additionalServiceId }),
            };

            return View("ListPrices/ManageListPrices", model);
        }

        [HttpGet("list-price-type")]
        public async Task<IActionResult> ListPriceType(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            CataloguePriceType? selectedPriceType = null)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new ListPriceTypeModel(solution)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId, solutionType, additionalServiceId }),
                SelectedCataloguePriceType = selectedPriceType,
            };

            return View("ListPrices/ListPriceType", model);
        }

        [HttpPost("list-price-type")]
        public IActionResult ListPriceType(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            ListPriceTypeModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/ListPriceType", model);

            var redirectAction = model.SelectedCataloguePriceType switch
            {
                CataloguePriceType.Tiered => nameof(AddTieredListPrice),
                CataloguePriceType.Flat => nameof(AddFlatListPrice),
                _ => string.Empty,
            };

            return RedirectToAction(redirectAction, new { solutionId, solutionType, additionalServiceId });
        }

        [HttpGet("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new AddTieredListPriceModel(solution)
            {
                BackLink = Url.Action(
                    nameof(ListPriceType),
                    new { solutionId, solutionType, additionalServiceId }),
            };

            return View("ListPrices/AddTieredListPrice", model);
        }

        [HttpPost("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            AddTieredListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddTieredListPrice", model);

            var cataloguePrice = await AddOrUpdateCataloguePrice(solutionId, model);

            return RedirectToAction(
                nameof(TieredPriceTiers),
                new
                {
                    solutionId,
                    cataloguePriceId = model.CataloguePriceId ?? cataloguePrice.CataloguePriceId,
                    solutionType,
                    additionalServiceId,
                });
        }

        [HttpGet("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(solution)
            {
                BackLink = Url.Action(
                    nameof(ListPriceType),
                    new { solutionId, solutionType, additionalServiceId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            AddEditFlatListPriceModel model)
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

            return RedirectToAction(
                nameof(Index),
                new { solutionId, solutionType, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new TieredPriceTiersModel(solution, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(
                    nameof(EditTieredListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId }),
                SolutionType = solutionType,
                AdditionalServiceId = additionalServiceId,
                AddTieredPriceTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    typeof(CatalogueSolutionListPriceController).ControllerName(),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId }),
            };

            return View("ListPrices/TieredPriceTiers", model);
        }

        [HttpPost("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            TieredPriceTiersModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
                var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

                model.Tiers = [.. price.CataloguePriceTiers];

                model.MaximumNumberOfTiers = priceTiersCapSettings.MaximumNumberOfPriceTiers;

                return View("ListPrices/TieredPriceTiers", model);
            }

            await listPriceService.SetPublicationStatus(solutionId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId, solutionType, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] int? supplierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] bool? isEditing)
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
                BackLink = Url.Action(actionName, new { solutionId, cataloguePriceId, supplierId, solutionType, additionalServiceId }),
                IsEditing = isEditing,
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] int? supplierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            AddEditTieredPriceTierModel model)
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

            return RedirectToAction(actionName, new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId });
        }

        [HttpGet("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new EditTieredListPriceModel(solution, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = solutionType is not null
                ? Url.Action(nameof(Index), new { solutionId, solutionType, additionalServiceId })
                : Url.Action(
                    nameof(SupplierServicesController.EditAssociatedService),
                    typeof(SupplierServicesController).ControllerName(),
                    new
                    {
                        supplierId,
                        associatedServiceId = solutionId,
                        solutionType,
                        additionalServiceId,
                    }),
                SolutionType = solutionType,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
                SupplierId = supplierId,
                AddPricingTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId, isEditing = true }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId, isTieredPrice = true }),
            };

            return View("ListPrices/EditTieredListPrice", model);
        }

        [HttpPost("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] int? supplierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            EditTieredListPriceModel model)
        {
            _ = supplierId;
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);

            if (!ModelState.IsValid)
            {
                model.Tiers = [.. price.CataloguePriceTiers];
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

            return RedirectToAction(nameof(Index), new { solutionId, solutionType, additionalServiceId });
        }

        [HttpGet("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(solution, price)
            {
                BackLink = solutionType is not null
                ? Url.Action(nameof(Index), new { solutionId, solutionType, additionalServiceId })
                : Url.Action(
                    nameof(AssociatedServiceListPriceController.Index),
                    typeof(AssociatedServiceListPriceController).ControllerName(),
                    new
                    {
                        supplierId,
                        associatedServiceId = solutionId,
                        solutionType,
                        additionalServiceId,
                    }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            AddEditFlatListPriceModel model)
        {
            _ = supplierId;
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

            return RedirectToAction(nameof(Index), new { solutionId, solutionType, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            bool? isEditing = null)
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
                BackLink = Url.Action(
                    actionName,
                    new { solutionId, cataloguePriceId, supplierId, solutionType, additionalServiceId }),
                IsEditing = isEditing,
                DeleteTieredPriceTierUrl = Url.Action(
                    nameof(DeleteTieredPriceTier),
                    new { solutionId, cataloguePriceId, tierId, supplierId, solutionType, additionalServiceId, isEditing }),
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            AddEditTieredPriceTierModel model)
        {
            _ = supplierId;

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

            return RedirectToAction(actionName, new { solutionId, cataloguePriceId, solutionType, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int tierIndex)
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
                BackLink = Url.Action(
                    nameof(EditTieredListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId }),
                TierIndex = tierIndex,
            };

            return View("ListPrices/EditTierPrice", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            EditTierPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/EditTierPrice", model);

            await listPriceService.UpdateTierPrice(solutionId, cataloguePriceId, tierId, model.Price!.Value);

            return RedirectToAction(
                nameof(EditTieredListPrice),
                new { solutionId, cataloguePriceId, solutionType, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            [FromQuery] bool? isTieredPrice = null)
        {
            _ = cataloguePriceId;
            _ = solutionType;

            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                solution.Name,
                "This list price will be deleted")
            {
                BackLink = isTieredPrice == true
                ? Url.Action(
                    nameof(EditTieredListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId })
                : Url.Action(
                    nameof(EditFlatListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId }),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            DeleteItemConfirmationModel model)
        {
            _ = model;
            _ = supplierId;
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

            if (price.PublishedStatus != PublicationStatus.Published)
                await listPriceService.DeleteListPrice(solutionId, cataloguePriceId);

            return RedirectToAction(nameof(Index), new { solutionId, solutionType, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            bool? isEditing = null)
        {
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                solution.Name,
                "This pricing tier will be deleted")
            {
                BackLink = Url.Action(
                    nameof(EditTieredPriceTier),
                    new { solutionId, cataloguePriceId, tierId, solutionType, additionalServiceId, supplierId, isEditing }),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(
            [FromRoute] CatalogueItemId solutionId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int? supplierId,
            DeleteItemConfirmationModel model,
            bool? isEditing = null)
        {
            _ = model;
            var solution = await listPriceService.GetCatalogueItemWithListPrices(solutionId);
            var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

            if (price.PublishedStatus == PublicationStatus.Published)
            {
                return RedirectToAction(
                    nameof(EditTieredListPrice),
                    new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId });
            }

            await listPriceService.DeletePriceTier(solutionId, cataloguePriceId, tierId);

            var actionName = GetAddOrEditBacklink(isEditing);
            return RedirectToAction(actionName, new { solutionId, cataloguePriceId, solutionType, additionalServiceId, supplierId });
        }

        private static string GetAddOrEditBacklink(bool? isEditing)
            => isEditing.GetValueOrDefault()
                ? nameof(EditTieredListPrice)
                : nameof(TieredPriceTiers);

        private async Task<CataloguePrice> AddOrUpdateCataloguePrice(
            CatalogueItemId solutionId,
            AddTieredListPriceModel model)
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
