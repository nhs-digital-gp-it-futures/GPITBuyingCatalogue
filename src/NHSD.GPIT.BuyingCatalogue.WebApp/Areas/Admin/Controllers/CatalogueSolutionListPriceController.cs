using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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
                CataloguePriceType.Flat => nameof(ListPriceType),
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
        public async Task<IActionResult> AddTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            if (solution is null)
                return NotFound();

            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);
            if (price is null)
                return NotFound();

            var model = new AddTieredPriceTierModel(solution, price)
            {
                BackLink = Url.Action(nameof(TieredPriceTiers), new { solutionId, cataloguePriceId }),
            };

            return View("ListPrices/AddTieredPriceTier", model);
        }

        [HttpPost("{cataloguePriceId}/tiers/add")]
        public async Task<IActionResult> AddTieredPriceTier(CatalogueItemId solutionId, int cataloguePriceId, AddTieredPriceTierModel model)
        {
            if (!ModelState.IsValid)
                return View("ListPrices/AddTieredPriceTier", model);

            var priceTier = new CataloguePriceTier
            {
                LowerRange = model.LowerRange!.Value,
                UpperRange = !model.IsInfiniteRange!.Value ? model.UpperRange : null,
                Price = model.Price!.Value,
            };

            await solutionListPriceService.AddListPriceTier(solutionId, cataloguePriceId, priceTier);

            return RedirectToAction(nameof(TieredPriceTiers), new { solutionId, cataloguePriceId });
        }

        private async Task<CataloguePrice> AddOrUpdateCataloguePrice(CatalogueItemId solutionId, AddTieredListPriceModel model)
        {
            var cataloguePrice = new CataloguePrice
            {
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = model.SelectedCalculationType!.Value,
                ProvisioningType = model.SelectedProvisioningType!.Value,
                TimeUnit = model.GetTimeUnit(),
                PricingUnit = model.GetPricingUnit(),
                CurrencyCode = "GBP",
            };

            if (model.CataloguePriceId is not null)
                await solutionListPriceService.UpdateListPrice(solutionId, model.CataloguePriceId!.Value, cataloguePrice);
            else
                await solutionListPriceService.AddListPrice(solutionId, cataloguePrice);

            return cataloguePrice;
        }
    }
}
