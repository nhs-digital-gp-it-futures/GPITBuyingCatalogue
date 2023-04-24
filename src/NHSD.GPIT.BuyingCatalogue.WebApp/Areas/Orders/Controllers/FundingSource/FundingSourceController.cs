using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/funding-sources")]
    public sealed class FundingSourceController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderFrameworkService orderFrameworkService;
        private readonly IFrameworkService frameworkService;

        public FundingSourceController(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IOrderFrameworkService orderFrameworkService,
            IFrameworkService frameworkService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderFrameworkService = orderFrameworkService ?? throw new ArgumentNullException(nameof(orderFrameworkService));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        }

        [HttpGet("select-framework")]
        public async Task<IActionResult> SelectFramework(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            var availableFrameworks = await orderFrameworkService.GetFrameworksForOrder(callOffId, internalOrgId, order.AssociatedServicesOnly);

            if (availableFrameworks.Count == 1)
            {
                await orderFrameworkService.SetSelectedFrameworkForOrder(callOffId, internalOrgId, availableFrameworks.First().Id);

                return RedirectToAction(
                    nameof(FundingSources),
                    typeof(FundingSourceController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = new SelectFrameworkModel(order, availableFrameworks)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select-framework")]
        public async Task<IActionResult> SelectFramework(SelectFrameworkModel model, string internalOrgId, CallOffId callOffId)
        {
            if (!ModelState.IsValid)
            {
                model.SetFrameworks(await orderFrameworkService.GetFrameworksForOrder(callOffId, internalOrgId, model.AssociatedServicesOnly));
                return View(model);
            }

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.SelectedFramework != null && model.SelectedFramework != order.SelectedFrameworkId)
            {
                return RedirectToAction(
                    nameof(ConfirmFrameworkChange),
                    typeof(FundingSourceController).ControllerName(),
                    new { internalOrgId, callOffId, selectedFrameworkId = model.SelectedFramework });
            }

            if (order.SelectedFramework is null)
                await orderFrameworkService.SetSelectedFrameworkForOrder(callOffId, internalOrgId, model.SelectedFramework);

            return RedirectToAction(
                nameof(FundingSources),
                typeof(FundingSourceController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select-framework/confirm-new-framework")]
        public async Task<IActionResult> ConfirmFrameworkChange(
            string internalOrgId,
            CallOffId callOffId,
            [FromQuery] string selectedFrameworkId)
        {
            var selectedFramework = await frameworkService.GetFrameworksById(selectedFrameworkId);

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            var model = new ConfirmFrameworkChangeModel(order, selectedFramework)
            {
                BackLink = Url.Action(
                    nameof(SelectFramework),
                    typeof(FundingSourceController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select-framework/confirm-new-framework")]
        public async Task<IActionResult> ConfirmFrameworkChange(
            ConfirmFrameworkChangeModel model,
            string internalOrgId,
            CallOffId callOffId,
            [FromQuery] string selectedFrameworkId)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.ConfirmChanges is false)
            {
                return RedirectToAction(
                    nameof(SelectFramework),
                    typeof(FundingSourceController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            await orderFrameworkService.UpdateFundingSourceAndSetSelectedFrameworkForOrder(callOffId, internalOrgId, model.SelectedFramework.Id);

            return RedirectToAction(
                nameof(FundingSources),
                typeof(FundingSourceController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet]
        public async Task<IActionResult> FundingSources(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderWithOrderItemsForFunding(callOffId, internalOrgId)).Order;

            var availableFrameworks = await orderFrameworkService.GetFrameworksForOrder(callOffId, internalOrgId, order.AssociatedServicesOnly);

            var model = new FundingSources(internalOrgId, callOffId, order, availableFrameworks.Count)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FundingSourcesContinue(string internalOrgId, CallOffId callOffId)
        {
            await orderService.SetFundingSourceForForceFundedItems(internalOrgId, callOffId);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("{catalogueItemId}/funding-source")]
        public async Task<IActionResult> FundingSource(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItemsForFunding(callOffId, internalOrgId)).Order;

            var item = await orderItemService.GetOrderItem(callOffId, internalOrgId, catalogueItemId);

            var model = new Models.FundingSources.FundingSource(internalOrgId, callOffId, order, item)
            {
                BackLink = Url.Action(
                    nameof(FundingSources),
                    typeof(FundingSourceController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}/funding-source")]
        public async Task<IActionResult> FundingSource(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, Models.FundingSources.FundingSource model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await orderItemService.UpdateOrderItemFunding(callOffId, internalOrgId, catalogueItemId, model.SelectedFundingType);

            return RedirectToAction(
                nameof(FundingSources),
                typeof(FundingSourceController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
