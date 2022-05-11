using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public class PricesController : Controller
    {
        private const string ConfirmPriceViewName = "ConfirmPrice";
        private const string SelectPriceViewName = "SelectPrice";

        private readonly IOrderPriceService orderPriceService;
        private readonly IOrderService orderService;
        private readonly ISolutionListPriceService listPriceService;

        public PricesController(
            IOrderPriceService orderPriceService,
            IOrderService orderService,
            ISolutionListPriceService listPriceService)
        {
            this.orderPriceService = orderPriceService ?? throw new ArgumentNullException(nameof(orderPriceService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
        }

        [HttpGet("price/select")]
        public async Task<IActionResult> SelectPrice(string internalOrgId, CallOffId callOffId, int? selectedPriceId = null)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solutionWithPrices = await listPriceService.GetCatalogueItemWithPublishedListPrices(order.GetSolution().CatalogueItemId);

            var model = new SelectPriceModel(solutionWithPrices)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.SolutionRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                SelectedPriceId = selectedPriceId,
            };

            return View(model);
        }

        [HttpPost("price/select")]
        public async Task<IActionResult> SelectPrice(string internalOrgId, CallOffId callOffId, SelectPriceModel model)
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
                new { internalOrgId, callOffId, priceId });
        }

        [HttpGet("price/{priceId}/confirm")]
        public async Task<IActionResult> ConfirmPrice(string internalOrgId, CallOffId callOffId, int priceId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solution = await listPriceService.GetCatalogueItemWithPublishedListPrices(order.GetSolution().CatalogueItemId);

            var model = new ConfirmPriceModel(solution, priceId)
            {
                BackLink = Url.Action(
                    nameof(SelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, selectedPriceId = priceId }),
            };

            return View(model);
        }

        [HttpPost("price/{priceId}/confirm")]
        public async Task<IActionResult> ConfirmPrice(string internalOrgId, CallOffId callOffId, int priceId, ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (orderId, price) = await GetOrderIdAndCataloguePrice(internalOrgId, callOffId, priceId);
            var agreedPrices = GetAgreedPricesFromModel(model);

            await orderPriceService.AddPrice(orderId, price, agreedPrices);

            return RedirectToAction(
                nameof(QuantityController.SelectQuantity),
                typeof(QuantityController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("price/edit")]
        public async Task<IActionResult> EditPrice(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new ConfirmPriceModel(order.GetSolution())
            {
                BackLink = Url.Action(
                    nameof(SelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(ConfirmPriceViewName, model);
        }

        [HttpPost("price/edit")]
        public async Task<IActionResult> EditPrice(string internalOrgId, CallOffId callOffId, ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ConfirmPriceViewName, model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var agreedPrices = GetAgreedPricesFromModel(model);

            await orderPriceService.UpdatePrice(order.Id, order.GetSolution().CatalogueItemId, agreedPrices);

            return RedirectToAction(
                nameof(QuantityController.SelectQuantity),
                typeof(QuantityController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("additional-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AdditionalServiceSelectPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int? selectedPriceId = null)
        {
            var solutionWithPrices = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var model = new SelectPriceModel(solutionWithPrices)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.SolutionRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                SelectedPriceId = selectedPriceId,
            };

            return View(SelectPriceViewName, model);
        }

        [HttpPost("additional-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AdditionalServiceSelectPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var solutionWithPrices = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View(SelectPriceViewName, model);
            }

            return RedirectToAction(
                nameof(AdditionalServiceConfirmPrice),
                typeof(PricesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, priceId = model.SelectedPriceId });
        }

        [HttpGet("additional-service/{catalogueItemId}/price/{priceId}/confirm")]
        public async Task<IActionResult> AdditionalServiceConfirmPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, int priceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var model = new ConfirmPriceModel(solution, priceId)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServiceSelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, selectedPriceId = priceId }),
            };

            return View(ConfirmPriceViewName, model);
        }

        [HttpPost("additional-service/{catalogueItemId}/price/{priceId}/confirm")]
        public async Task<IActionResult> AdditionalServiceConfirmPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int priceId,
            ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ConfirmPriceViewName, model);
            }

            var (orderId, price) = await GetOrderIdAndCataloguePrice(internalOrgId, callOffId, priceId, catalogueItemId);
            var agreedPrices = GetAgreedPricesFromModel(model);

            await orderPriceService.AddPrice(orderId, price, agreedPrices);

            // TODO: Replace with version that goes to quantity
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("associated-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AssociatedServiceSelectPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int? selectedPriceId = null)
        {
            var associatedService = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var model = new SelectPriceModel(associatedService)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.SolutionRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                SelectedPriceId = selectedPriceId,
            };

            return View(SelectPriceViewName, model);
        }

        [HttpPost("associated-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AssociatedServiceSelectPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var solutionWithPrices = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View(SelectPriceViewName, model);
            }

            return RedirectToAction(
                nameof(AssociatedServiceConfirmPrice),
                typeof(PricesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, priceId = model.SelectedPriceId });
        }

        [HttpGet("associated-service/{catalogueItemId}/price/{priceId}/confirm")]
        public async Task<IActionResult> AssociatedServiceConfirmPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int priceId)
        {
            var solution = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId);

            var model = new ConfirmPriceModel(solution, priceId)
            {
                BackLink = Url.Action(
                    nameof(AssociatedServiceSelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, selectedPriceId = priceId }),
            };

            return View(ConfirmPriceViewName, model);
        }

        [HttpPost("associated-service/{catalogueItemId}/price/{priceId}/confirm")]
        public async Task<IActionResult> AssociatedServiceConfirmPrice(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int priceId,
            ConfirmPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ConfirmPriceViewName, model);
            }

            var (orderId, price) = await GetOrderIdAndCataloguePrice(internalOrgId, callOffId, priceId, catalogueItemId);
            var agreedPrices = GetAgreedPricesFromModel(model);

            await orderPriceService.AddPrice(orderId, price, agreedPrices);

            // TODO: Replace with version that goes to quantity
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private static List<OrderPricingTierDto> GetAgreedPricesFromModel(ConfirmPriceModel model)
        {
            return model.Tiers
                .Select(x => new OrderPricingTierDto
                {
                    Price = decimal.Parse(x.AgreedPrice),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToList();
        }

        private async Task<(int OrderId, CataloguePrice Price)> GetOrderIdAndCataloguePrice(
            string internalOrgId,
            CallOffId callOffId,
            int priceId,
            CatalogueItemId? catalogueItemId = null)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            catalogueItemId ??= order.GetSolution().CatalogueItemId;

            var service = await listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItemId.Value);
            var price = service.CataloguePrices.Single(x => x.CataloguePriceId == priceId);

            return (order.Id, price);
        }
    }
}
