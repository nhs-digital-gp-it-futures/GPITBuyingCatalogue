﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/associated-services/{associatedServiceId}/list-prices")]
    public class AssociatedServiceListPriceController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IListPriceService listPriceService;
        private readonly PriceTiersCapSettings priceTiersCapSettings;

        public AssociatedServiceListPriceController(
            ISolutionsService solutionsService,
            IAssociatedServicesService associatedServicesService,
            IListPriceService listPriceService,
            PriceTiersCapSettings priceTiersCapSettings)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
            this.priceTiersCapSettings = priceTiersCapSettings ?? throw new ArgumentNullException(nameof(priceTiersCapSettings));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId, CatalogueItemId associatedServiceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return NotFound();

            var service = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (service is null)
                return NotFound();

            var model = new ManageListPricesModel(solution, service, service.CataloguePrices)
            {
                BackLink = Url.Action(
                    nameof(AssociatedServicesController.EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { solutionId, associatedServiceId }),
                AddListPriceUrl = Url.Action(
                    nameof(AddFlatListPrice),
                    new { solutionId, associatedServiceId }),
            };

            return View("ListPrices/ManageListPrices", model);
        }

        [HttpGet("list-price-type")]
        public async Task<IActionResult> ListPriceType(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            CataloguePriceType? selectedPriceType = null)
        {
            var associatedService =
                await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new ListPriceTypeModel(associatedService)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId, associatedServiceId }),
                SelectedCataloguePriceType = selectedPriceType,
            };

            return View("ListPrices/ListPriceType", model);
        }

        [HttpPost("list-price-type")]
        public IActionResult ListPriceType(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
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

            return RedirectToAction(redirectAction, new { solutionId, associatedServiceId });
        }

        [HttpGet("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int? cataloguePriceId = null)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return NotFound();

            var associatedService =
                await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            AddTieredListPriceModel model = cataloguePriceId is not null
                ? new(solution, associatedService, associatedService.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId))
                {
                    DeleteListPriceUrl = Url.Action(
                            nameof(DeleteListPrice),
                            new { solutionId, associatedServiceId, cataloguePriceId }),
                }
                : new(solution, associatedService);

            model.BackLink = Url.Action(nameof(Index), new { solutionId, associatedServiceId });

            return View("ListPrices/AddTieredListPrice", model);
        }

        [HttpPost("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            AddTieredListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddTieredListPrice", model);

            var cataloguePrice = await AddOrUpdateCataloguePrice(associatedServiceId, model);

            return RedirectToAction(nameof(TieredPriceTiers), new { solutionId, associatedServiceId, cataloguePriceId = model.CataloguePriceId ?? cataloguePrice.CataloguePriceId });
        }

        [HttpGet("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(associatedService)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId, associatedServiceId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
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

            await listPriceService.AddListPrice(associatedServiceId, price);

            return RedirectToAction(nameof(Index), new { solutionId, associatedServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return NotFound();

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new TieredPriceTiersModel(solution, associatedService, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(nameof(AddTieredListPrice), new { solutionId, associatedServiceId, cataloguePriceId }),
                AddTieredPriceTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    new { solutionId, associatedServiceId, cataloguePriceId }),
            };

            return View("ListPrices/TieredPriceTiers", model);
        }

        [HttpPost("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            TieredPriceTiersModel model)
        {
            if (!ModelState.IsValid)
            {
                var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
                var price = associatedService.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

                model.Tiers = price.CataloguePriceTiers.ToList();

                model.MaximumNumberOfTiers = priceTiersCapSettings.MaximumNumberOfPriceTiers;

                return View("ListPrices/TieredPriceTiers", model);
            }

            await listPriceService.SetPublicationStatus(associatedServiceId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId, associatedServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            bool? isEditing)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var actionName = GetAddOrEditBacklink(isEditing);

            var model = new AddEditTieredPriceTierModel(associatedService, price)
            {
                BackLink = Url.Action(actionName, new { solutionId, associatedServiceId, cataloguePriceId }),
                IsEditing = isEditing,
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
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

            await listPriceService.AddListPriceTier(associatedServiceId, cataloguePriceId, priceTier);

            var actionName = GetAddOrEditBacklink(model.IsEditing);

            return RedirectToAction(actionName, new { solutionId, associatedServiceId, cataloguePriceId });
        }

        [HttpGet("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return NotFound();

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new EditTieredListPriceModel(solution, associatedService, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId, associatedServiceId }),
                AddPricingTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    new { solutionId, associatedServiceId, cataloguePriceId, isEditing = true }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { solutionId, associatedServiceId, cataloguePriceId }),
            };

            return View("ListPrices/EditTieredListPrice", model);
        }

        [HttpPost("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            EditTieredListPriceModel model)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.Tiers = price.CataloguePriceTiers.ToList();
                model.MaximumNumberOfTiers = priceTiersCapSettings.MaximumNumberOfPriceTiers;

                return View("ListPrices/EditTieredListPrice", model);
            }

            await listPriceService.UpdateListPrice(
                    associatedServiceId,
                    cataloguePriceId,
                    model.GetPricingUnit(),
                    model.SelectedProvisioningType!.Value,
                    model.SelectedCalculationType!.Value,
                    model.GetBillingPeriod(),
                    model.GetQuantityCalculationType());

            if (model.SelectedPublicationStatus != price.PublishedStatus)
                await listPriceService.SetPublicationStatus(associatedServiceId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId, associatedServiceId });
        }

        [HttpGet("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(associatedService, price)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId, associatedServiceId }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { solutionId, associatedServiceId, cataloguePriceId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            AddEditFlatListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddEditFlatListPrice", model);

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            await listPriceService.UpdateListPrice(
                associatedServiceId,
                cataloguePriceId,
                model.GetPricingUnit(),
                model.SelectedProvisioningType!.Value,
                model.SelectedCalculationType!.Value,
                model.GetBillingPeriod(),
                model.GetQuantityCalculationType(),
                model.Price!.Value);

            if (model.SelectedPublicationStatus!.Value != price.PublishedStatus)
                await listPriceService.SetPublicationStatus(associatedServiceId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(nameof(Index), new { solutionId, associatedServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            bool? isEditing = null)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var tier = price.CataloguePriceTiers.FirstOrDefault(p => p.Id == tierId);
            if (tier is null)
                return NotFound();

            var actionName = GetAddOrEditBacklink(isEditing);

            var model = new AddEditTieredPriceTierModel(associatedService, price, tier)
            {
                BackLink = Url.Action(actionName, new { solutionId, associatedServiceId, cataloguePriceId }),
                IsEditing = isEditing,
                DeleteTieredPriceTierUrl = Url.Action(
                    nameof(DeleteTieredPriceTier),
                    new { solutionId, associatedServiceId, cataloguePriceId, tierId, isEditing }),
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            AddEditTieredPriceTierModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddEditTieredPriceTier", model);

            await listPriceService.UpdateListPriceTier(
                associatedServiceId,
                cataloguePriceId,
                tierId,
                model.Price!.Value,
                model.LowerRange!.Value,
                model.IsInfiniteRange.HasValue && model.IsInfiniteRange!.Value ? null : model.UpperRange);

            var actionName = GetAddOrEditBacklink(model.IsEditing);

            return RedirectToAction(actionName, new { solutionId, associatedServiceId, cataloguePriceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            int tierIndex)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var tier = price.CataloguePriceTiers.FirstOrDefault(p => p.Id == tierId);
            if (tier is null)
                return NotFound();

            var model = new EditTierPriceModel(associatedService, price, tier)
            {
                BackLink = Url.Action(nameof(EditTieredListPrice), new { solutionId, associatedServiceId, cataloguePriceId }),
                TierIndex = tierIndex,
            };

            return View("ListPrices/EditTierPrice", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            EditTierPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/EditTierPrice", model);

            await listPriceService.UpdateTierPrice(associatedServiceId, cataloguePriceId, tierId, model.Price!.Value);

            return RedirectToAction(nameof(EditTieredListPrice), new { solutionId, associatedServiceId, cataloguePriceId });
        }

        [HttpGet("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId)
        {
            _ = cataloguePriceId;

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                associatedService.Name,
                "This list price will be deleted")
            {
                BackLink = HttpContext.Request.Headers.Referer.ToString(),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            DeleteItemConfirmationModel model)
        {
            _ = model;

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            if (price.PublishedStatus != PublicationStatus.Published)
                await listPriceService.DeleteListPrice(associatedServiceId, cataloguePriceId);

            return RedirectToAction(nameof(Index), new { solutionId, associatedServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            bool? isEditing = null)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new DeleteItemConfirmationModel(
                "Delete pricing tier",
                associatedService.Name,
                "This pricing tier will be deleted")
            {
                BackLink = Url.Action(nameof(EditTieredPriceTier), new { solutionId, associatedServiceId, cataloguePriceId, tierId, isEditing }),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            int cataloguePriceId,
            int tierId,
            DeleteItemConfirmationModel model,
            bool? isEditing = null)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            if (price.PublishedStatus == PublicationStatus.Published)
                return RedirectToAction(nameof(EditTieredListPrice), new { solutionId, associatedServiceId, cataloguePriceId });

            await listPriceService.DeletePriceTier(associatedServiceId, cataloguePriceId, tierId);

            var actionName = GetAddOrEditBacklink(isEditing);
            return RedirectToAction(actionName, new { solutionId, associatedServiceId, cataloguePriceId });
        }

        private static string GetAddOrEditBacklink(bool? isEditing)
            => isEditing.GetValueOrDefault()
                ? nameof(EditTieredListPrice)
                : nameof(TieredPriceTiers);

        private async Task<CataloguePrice> AddOrUpdateCataloguePrice(CatalogueItemId associatedServiceId, AddTieredListPriceModel model)
        {
            CataloguePrice cataloguePrice;
            if (model.CataloguePriceId is not null)
            {
                cataloguePrice = await listPriceService.UpdateListPrice(
                    associatedServiceId,
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

                await listPriceService.AddListPrice(associatedServiceId, cataloguePrice);
            }

            return cataloguePrice;
        }
    }
}
