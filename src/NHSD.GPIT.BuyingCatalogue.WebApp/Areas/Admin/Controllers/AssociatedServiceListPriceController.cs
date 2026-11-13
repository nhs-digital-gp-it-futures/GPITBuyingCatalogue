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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "ManageSupplierOrganisations")]
    [Area("Admin")]
    [Route("admin/manage-suppliers/{supplierId:int}/services/{associatedServiceId}")]
    public class AssociatedServiceListPriceController(
        IAssociatedServicesService associatedServicesService,
        IListPriceService listPriceService,
        PriceTiersCapSettings priceTiersCapSettings)
        : Controller
    {
        private readonly IAssociatedServicesService associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
        private readonly IListPriceService listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
        private readonly PriceTiersCapSettings priceTiersCapSettings = priceTiersCapSettings ?? throw new ArgumentNullException(nameof(priceTiersCapSettings));

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var service = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (service is null)
                return NotFound();

            var model = new ManageListPricesModel(supplierId, service, service.CataloguePrices)
            {
                SolutionType = solutionType,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
                SupplierId = supplierId,
                BackLink = Url.Action(
                    nameof(SupplierServicesController.EditAssociatedService),
                    typeof(SupplierServicesController).ControllerName(),
                    new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
                AddListPriceUrl = (service.AssociatedService.PracticeReorganisationType == PracticeReorganisationTypeEnum.None) ?
                    Url.Action(nameof(ListPriceType), new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }) :
                    Url.Action(nameof(AddFlatListPrice), new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
            };

            return View("ListPrices/ManageListPrices", model);
        }

        [HttpGet("list-price-type")]
        public async Task<IActionResult> ListPriceType(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            CataloguePriceType? selectedPriceType = null)
        {
            var associatedService =
                await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new ListPriceTypeModel(associatedService)
            {
                BackLink = Url.Action(
                    nameof(Index),
                    new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
                SelectedCataloguePriceType = selectedPriceType,
            };

            return View("ListPrices/ListPriceType", model);
        }

        [HttpPost("list-price-type")]
        public IActionResult ListPriceType(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            ListPriceTypeModel model,
            CataloguePriceType? selectedPriceType = null)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/ListPriceType", model);

            var redirectAction = model.SelectedCataloguePriceType switch
            {
                CataloguePriceType.Tiered => nameof(AddTieredListPrice),
                CataloguePriceType.Flat => nameof(AddFlatListPrice),
                _ => string.Empty,
            };

            return RedirectToAction(
                redirectAction,
                new
                {
                    supplierId,
                    associatedServiceId,
                    solutionType,
                    solutionId,
                    additionalServiceId,
                });
        }

        [HttpGet("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var associatedService =
                await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new AddTieredListPriceModel(supplierId, associatedService)
            {
                BackLink = Url.Action(
                    nameof(ListPriceType),
                    new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
            };

            return View("ListPrices/AddTieredListPrice", model);
        }

        [HttpPost("add-tiered-list-price")]
        public async Task<IActionResult> AddTieredListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            AddTieredListPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddTieredListPrice", model);

            var cataloguePrice = await AddOrUpdateCataloguePrice(associatedServiceId, model);

            return RedirectToAction(
                nameof(TieredPriceTiers),
                new
                {
                    supplierId,
                    associatedServiceId,
                    cataloguePriceId = model.CataloguePriceId ?? cataloguePrice.CataloguePriceId,
                    solutionType,
                    solutionId,
                    additionalServiceId,
                });
        }

        [HttpGet("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(associatedService)
            {
                PracticeReorganisation = associatedService.AssociatedService.PracticeReorganisationType,
                BackLink = (associatedService.AssociatedService.PracticeReorganisationType == PracticeReorganisationTypeEnum.None)
                ? Url.Action(
                    nameof(ListPriceType),
                    new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId })
                : Url.Action(
                    nameof(Index),
                    new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("add-flat-list-price")]
        public async Task<IActionResult> AddFlatListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
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

            await listPriceService.AddListPrice(associatedServiceId, price);

            return RedirectToAction(
                nameof(Index),
                new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new TieredPriceTiersModel(supplierId, associatedService, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(
                    nameof(EditTieredListPrice),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
                SolutionType = solutionType,
                AdditionalServiceId = additionalServiceId,
                AddTieredPriceTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
            };

            return View("ListPrices/TieredPriceTiers", model);
        }

        [HttpPost("{cataloguePriceId}/tiers")]
        public async Task<IActionResult> TieredPriceTiers(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            TieredPriceTiersModel model)
        {
            if (!ModelState.IsValid)
            {
                var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
                var price = associatedService.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

                model.Tiers = [.. price.CataloguePriceTiers];

                model.MaximumNumberOfTiers = priceTiersCapSettings.MaximumNumberOfPriceTiers;

                return View("ListPrices/TieredPriceTiers", model);
            }

            await listPriceService.SetPublicationStatus(associatedServiceId, cataloguePriceId, model.SelectedPublicationStatus!.Value);

            return RedirectToAction(
                nameof(Index),
                new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] bool? isEditing)
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
                BackLink = Url.Action(
                    actionName,
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
                IsEditing = isEditing,
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
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

            await listPriceService.AddListPriceTier(associatedServiceId, cataloguePriceId, priceTier);

            var actionName = GetAddOrEditBacklink(model.IsEditing);

            return RedirectToAction(
                actionName,
                new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new EditTieredListPriceModel(supplierId, associatedService, price, priceTiersCapSettings.MaximumNumberOfPriceTiers)
            {
                BackLink = Url.Action(nameof(Index), new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
                SolutionType = solutionType,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
                AddPricingTierUrl = Url.Action(
                    nameof(AddTieredPriceTier),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId, isEditing = true }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
            };

            return View("ListPrices/EditTieredListPrice", model);
        }

        [HttpPost("tiered/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditTieredListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
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
                model.Tiers = [.. price.CataloguePriceTiers];
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

            return RedirectToAction(nameof(Index), new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new AddEditFlatListPriceModel(associatedService, price)
            {
                PracticeReorganisation = associatedService.AssociatedService.PracticeReorganisationType,
                BackLink = Url.Action(nameof(Index), new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId }),
                DeleteListPriceUrl = Url.Action(
                    nameof(DeleteListPrice),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
            };

            return View("ListPrices/AddEditFlatListPrice", model);
        }

        [HttpPost("flat/{cataloguePriceId}/edit")]
        public async Task<IActionResult> EditFlatListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
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

            return RedirectToAction(nameof(Index), new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
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
                BackLink = Url.Action(
                    actionName,
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
                IsEditing = isEditing,
                DeleteTieredPriceTierUrl = Url.Action(
                    nameof(DeleteTieredPriceTier),
                    new
                    {
                        supplierId,
                        associatedServiceId,
                        cataloguePriceId,
                        tierId,
                        solutionType,
                        solutionId,
                        additionalServiceId,
                        isEditing,
                    }),
            };

            return View("ListPrices/AddEditTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit")]
        public async Task<IActionResult> EditTieredPriceTier(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
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

            return RedirectToAction(
                actionName,
                new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            [FromQuery] int tierIndex)
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
                BackLink = Url.Action(
                    nameof(EditTieredListPrice),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId }),
                TierIndex = tierIndex,
            };

            return View("ListPrices/EditTierPrice", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/edit-price")]
        public async Task<IActionResult> EditTierPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            EditTierPriceModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/EditTierPrice", model);

            await listPriceService.UpdateTierPrice(associatedServiceId, cataloguePriceId, tierId, model.Price!.Value);

            return RedirectToAction(
                nameof(EditTieredListPrice),
                new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId)
        {
            _ = supplierId;
            _ = cataloguePriceId;
            _ = solutionType;
            _ = solutionId;
            _ = additionalServiceId;

            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var model = new DeleteItemConfirmationModel(
                "Delete list price",
                associatedService.Name,
                "This list price will be deleted")
            {
                BackLink = new Uri(HttpContext.Request.Headers.Referer).AbsolutePath,
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/delete")]
        public async Task<IActionResult> DeleteListPrice(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
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

            return RedirectToAction(
                nameof(Index),
                new { supplierId, associatedServiceId, solutionType, solutionId, additionalServiceId });
        }

        [HttpGet("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
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
                BackLink = Url.Action(
                    nameof(EditTieredPriceTier),
                    new
                    {
                        supplierId,
                        associatedServiceId,
                        cataloguePriceId,
                        tierId,
                        solutionType,
                        solutionId,
                        additionalServiceId,
                        isEditing,
                    }),
            };

            return View("ListPrices/DeleteItem", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/{tierId}/delete")]
        public async Task<IActionResult> DeleteTieredPriceTier(
            [FromRoute] int supplierId,
            [FromRoute] CatalogueItemId associatedServiceId,
            [FromRoute] int cataloguePriceId,
            [FromRoute] int tierId,
            [FromQuery] SolutionType? solutionType,
            [FromQuery] CatalogueItemId? solutionId,
            [FromQuery] CatalogueItemId? additionalServiceId,
            DeleteItemConfirmationModel model,
            bool? isEditing = null)
        {
            _ = model;
            var associatedService = await associatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId);
            if (associatedService is null)
                return NotFound();

            var price = associatedService.CataloguePrices.FirstOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            if (price.PublishedStatus == PublicationStatus.Published)
            {
                return RedirectToAction(
                    nameof(EditTieredListPrice),
                    new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId });
            }

            await listPriceService.DeletePriceTier(associatedServiceId, cataloguePriceId, tierId);

            var actionName = GetAddOrEditBacklink(isEditing);
            return RedirectToAction(
                actionName,
                new { supplierId, associatedServiceId, cataloguePriceId, solutionType, solutionId, additionalServiceId });
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
