using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/item/{catalogueItemId}")]
    public class PricesController : Controller
    {
        private const string ConfirmPriceViewName = "ConfirmPrice";

        private readonly IOrderPriceService orderPriceService;
        private readonly IOrderService orderService;
        private readonly ISolutionListPriceService listPriceService;
        private readonly IRoutingService routingService;

        public PricesController(
            IOrderPriceService orderPriceService,
            IOrderService orderService,
            ISolutionListPriceService listPriceService,
            IRoutingService routingService)
        {
            this.orderPriceService = orderPriceService ?? throw new ArgumentNullException(nameof(orderPriceService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("price/select")]
        public async Task<IActionResult> SelectPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, int? selectedPriceId = null)
        {
            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var model = new SelectPriceModel(catalogueItem)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.ServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId }),
                SelectedPriceId = selectedPriceId,
            };

            return View(model);
        }

        [HttpPost("price/select")]
        public async Task<IActionResult> SelectPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var order = await orderService.GetOrderThin(callOffId, internalOrgId);
                var solutionWithPrices = await listPriceService.GetCatalogueItemWithPublishedListPrices(order.GetSolution().CatalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View(model);
            }

            var priceId = model.SelectedPriceId!.Value;

            return RedirectToAction(
                nameof(ConfirmPrice),
                typeof(PricesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, priceId });
        }

        [HttpGet("price/{priceId}/confirm")]
        public async Task<IActionResult> ConfirmPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, int priceId)
        {
            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var route = routingService.GetRoute(
                RoutingSource.ConfirmPriceBackLink,
                await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId),
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { SelectedPriceId = priceId });

            var model = new ConfirmPriceModel(catalogueItem, priceId)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
            };

            return View(model);
        }

        [HttpPost("price/{priceId}/confirm")]
        public async Task<IActionResult> ConfirmPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int priceId,
            ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var price = await GetCataloguePrice(priceId, catalogueItemId);

            await orderPriceService.AddPrice(order.Id, price, model.AgreedPrices);

            return RedirectToAction(
                nameof(QuantityController.SelectQuantity),
                typeof(QuantityController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        [HttpGet("price/edit")]
        public async Task<IActionResult> EditPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new ConfirmPriceModel(order.OrderItem(catalogueItemId))
            {
                BackLink = Url.Action(
                    nameof(SelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId }),
            };

            return View(ConfirmPriceViewName, model);
        }

        [HttpPost("price/edit")]
        public async Task<IActionResult> EditPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ConfirmPriceViewName, model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            await orderPriceService.UpdatePrice(order.Id, catalogueItemId, model.AgreedPrices);

            return RedirectToAction(
                nameof(QuantityController.SelectQuantity),
                typeof(QuantityController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        private async Task<CataloguePrice> GetCataloguePrice(int priceId, CatalogueItemId catalogueItemId)
        {
            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            return catalogueItem.CataloguePrices.Single(x => x.CataloguePriceId == priceId);
        }
    }
}
