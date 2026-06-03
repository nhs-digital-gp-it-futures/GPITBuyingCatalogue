using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/item/{catalogueItemId}")]
    public class PricesController : Controller
    {
        private const string ConfirmPriceViewName = "PriceSelection/ConfirmPrice";

        private readonly IOrderPriceService orderPriceService;
        private readonly IOrderService orderService;
        private readonly IListPriceService listPriceService;
        private readonly IOrderItemService orderItemService;
        private readonly IRoutingService routingService;

        public PricesController(
            IOrderPriceService orderPriceService,
            IOrderService orderService,
            IListPriceService listPriceService,
            IOrderItemService orderItemService,
            IRoutingService routingService)
        {
            this.orderPriceService = orderPriceService ?? throw new ArgumentNullException(nameof(orderPriceService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("price/select")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SelectPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            int? selectedPriceId = null,
            RoutingSource? source = null)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var orderItem = order.OrderItem(orderItemId);
            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var caption = GetCaption(source, orderItem);

            var route = routingService.GetRoute(
                RoutingPoint.SelectPriceBackLink,
                await orderService.GetOrderWithOrderItems(callOffId, internalOrgId),
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source, OrderItemId = orderItemId });

            var model = new SelectPriceModel(catalogueItem)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                SelectedPriceId = selectedPriceId,
                Source = source,
                OrderItemId = orderItemId,
                Caption = caption,
            };

            return View("PriceSelection/SelectPrice", model);
        }

        [HttpPost("price/select")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SelectPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
                var solutionWithPrices = await listPriceService.GetCatalogueItemWithPublishedListPrices(order.GetSolutionOrderItem().CatalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View("PriceSelection/SelectPrice", model);
            }

            var priceId = model.SelectedPriceId!.Value;

            return RedirectToAction(
                nameof(ConfirmPrice),
                typeof(PricesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, priceId, model.OrderItemId, model.Source });
        }

        [HttpGet("price/{priceId}/confirm")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> ConfirmPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            int priceId,
            RoutingSource? source = null)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var orderItem = order.OrderItem(orderItemId);
            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var caption = GetCaption(source, orderItem);

            var route = routingService.GetRoute(
                RoutingPoint.ConfirmPriceBackLink,
                await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId),
                new RouteValues(internalOrgId, callOffId, catalogueItemId)
                {
                    SelectedPriceId = priceId,
                    OrderItemId = orderItemId,
                    Source = source,
                });

            var price = catalogueItem.CataloguePrices.First(x => x.CataloguePriceId == priceId);

            var existingPrice = orderItem?.OrderItemPrice?.CataloguePriceId == priceId
                ? orderItem.OrderItemPrice
                : null;

            var model = new ConfirmPriceModel(catalogueItem, price, existingPrice)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                Source = source,
                Caption = caption,
            };

            return View(ConfirmPriceViewName, model);
        }

        [HttpPost("price/{priceId}/confirm")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> ConfirmPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            int priceId,
            ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ConfirmPriceViewName, model);
            }

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var price = await GetCataloguePrice(priceId, catalogueItemId);
            var orderItem = order.OrderItem(orderItemId);

            await orderPriceService.UpsertPrice(orderItem.Id, price, model.AgreedPrices);

            await orderItemService.SetOrderItemEstimationPeriod(callOffId, internalOrgId, orderItemId, price);

            var route = routingService.GetRoute(
                RoutingPoint.ConfirmPrice,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source, OrderItemId = orderItemId });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("price/edit")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> EditPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            if (source == RoutingSource.TaskList
                && catalogueItem.CataloguePrices.Count > 1)
            {
                var selectedPriceId = order.OrderItem(catalogueItemId)?.OrderItemPrice?.CataloguePriceId;

                return RedirectToAction(
                    nameof(SelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, selectedPriceId, source });
            }

            var route = routingService.GetRoute(
                RoutingPoint.EditPriceBackLink,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source, OrderItemId = orderItemId });

            var orderItem = order.OrderItem(orderItemId);
            var price = orderItem.OrderItemPrice;

            var caption = GetCaption(source, orderItem);

            var catalogueItemPrice =
                catalogueItem.CataloguePrices.FirstOrDefault(x => x.CataloguePriceId == price.CataloguePriceId);

            var model = new ConfirmPriceModel(catalogueItem, catalogueItemPrice, price)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                Source = source,
                Caption = caption,
            };

            return View(ConfirmPriceViewName, model);
        }

        [HttpPost("price/edit")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> EditPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ConfirmPriceViewName, model);
            }

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var orderItem = order.OrderItem(orderItemId);

            await orderPriceService.UpdatePrice(orderItem.Id, model.AgreedPrices);

            await orderItemService.DetectChangesInFundingAndDelete(callOffId, internalOrgId, orderItemId);

            var route = routingService.GetRoute(
                RoutingPoint.EditPrice,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source, OrderItemId = orderItemId });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("price/view")]
        public async Task<IActionResult> ViewPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Previous;

            var orderItem = order.OrderItem(catalogueItemId);
            var price = orderItem.OrderItemPrice;

            var backLink = Url.Action(
                   nameof(TaskListController.TaskList),
                   typeof(TaskListController).ControllerName(),
                   new { internalOrgId, callOffId });

            var model = new ViewPriceModel(price, orderItem.CatalogueItem)
            {
                BackLink = backLink,
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                OnwardLink = backLink,
            };

            return View("PriceSelection/ViewPrice", model);
        }

        private static string GetCaption(RoutingSource? source, OrderItem orderItem)
        {
            var defaultCaption = $"{orderItem.CatalogueItem.Name}";
            var caption = source == RoutingSource.ManageAssociatedServices
                ? $"{orderItem.Parent.CatalogueItem.Name} - {defaultCaption}"
                : defaultCaption;
            return caption;
        }

        private async Task<CataloguePrice> GetCataloguePrice(int priceId, CatalogueItemId catalogueItemId)
        {
            var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            return catalogueItem.CataloguePrices.First(x => x.CataloguePriceId == priceId);
        }
    }
}
