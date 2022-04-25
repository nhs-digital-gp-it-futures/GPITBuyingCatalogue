using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/price")]
    public class PricesController : Controller
    {
        private const string SelectPriceViewName = "SelectPrice";
        private readonly IOrderService orderService;
        private readonly ISolutionListPriceService listPriceService;

        public PricesController(
            IOrderService orderService,
            ISolutionListPriceService listPriceService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
        }

        [HttpGet("select-price")]
        public async Task<IActionResult> SelectPrice(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solutionWithPrices = await listPriceService.GetSolutionWithPublishedListPrices(order.GetSolution().CatalogueItemId);

            var model = new SelectPriceModel(solutionWithPrices)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.SolutionRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select-price")]
        public async Task<IActionResult> SelectPrice(string internalOrgId, CallOffId callOffId, SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var order = await orderService.GetOrderThin(callOffId, internalOrgId);
                var solutionWithPrices = await listPriceService.GetSolutionWithPublishedListPrices(order.GetSolution().CatalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View(model);
            }

            // TODO: Replace with version that goes to price
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("additional-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AdditionalServiceSelectPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solutionWithPrices = await listPriceService.GetSolutionWithPublishedListPrices(catalogueItemId);

            var model = new SelectPriceModel(solutionWithPrices)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.SolutionRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(SelectPriceViewName, model);
        }

        [HttpPost("additional-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AdditionalServiceSelectPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var order = await orderService.GetOrderThin(callOffId, internalOrgId);
                var solutionWithPrices = await listPriceService.GetSolutionWithPublishedListPrices(catalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View(SelectPriceViewName, model);
            }

            // TODO: Replace with version that goes to price
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("associated-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AssociatedServiceSelectPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solutionWithPrices = await listPriceService.GetSolutionWithPublishedListPrices(catalogueItemId);

            var model = new SelectPriceModel(solutionWithPrices)
            {
                BackLink = Url.Action(
                    nameof(ServiceRecipientsController.SolutionRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(SelectPriceViewName, model);
        }

        [HttpPost("associated-service/{catalogueItemId}/select-price")]
        public async Task<IActionResult> AssociatedServiceSelectPrice(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                var order = await orderService.GetOrderThin(callOffId, internalOrgId);
                var solutionWithPrices = await listPriceService.GetSolutionWithPublishedListPrices(catalogueItemId);
                model.Prices = solutionWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
                return View(SelectPriceViewName, model);
            }

            // TODO: Replace with version that goes to price
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
