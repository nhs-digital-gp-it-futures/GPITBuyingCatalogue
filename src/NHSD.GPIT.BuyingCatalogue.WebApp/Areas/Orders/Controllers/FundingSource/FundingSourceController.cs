﻿using System;
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

        [Obsolete("Orders should be given a framework on creation going forward, however this remains to allow the framework to be set on any existing orders as a form of data migration. This is ONLY used when an order doesn't already have a framework set")]
        [HttpGet("select-framework")]
        public async Task<IActionResult> SelectFramework(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.SelectedFramework != null)
            {
                return BadRequest();
            }

            var availableFrameworks = await orderFrameworkService.GetFrameworksForOrder(callOffId, internalOrgId, order.OrderType.AssociatedServicesOnly);

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

        [Obsolete("Orders should be given a framework on creation going forward, however this remains to allow the framework to be set on any existing orders as a form of data migration. This is ONLY used when an order doesn't already have a framework set")]
        [HttpPost("select-framework")]
        public async Task<IActionResult> SelectFramework(SelectFrameworkModel model, string internalOrgId, CallOffId callOffId)
        {
            if (!ModelState.IsValid)
            {
                model.SetFrameworks(await orderFrameworkService.GetFrameworksForOrder(callOffId, internalOrgId, model.AssociatedServicesOnly));
                return View(model);
            }

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.SelectedFramework != null)
            {
                return BadRequest();
            }

            await orderFrameworkService.SetSelectedFrameworkForOrder(callOffId, internalOrgId, model.SelectedFramework);

            return RedirectToAction(
                nameof(FundingSources),
                typeof(FundingSourceController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet]
        public async Task<IActionResult> FundingSources(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItemsForFunding(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            var model = new FundingSources(internalOrgId, callOffId, orderWrapper)
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
            var orderWrapper = await orderService.GetOrderWithOrderItemsForFunding(callOffId, internalOrgId);

            var item = await orderItemService.GetOrderItem(callOffId, internalOrgId, catalogueItemId);

            var model = new Models.FundingSources.FundingSource(internalOrgId, callOffId, orderWrapper, item)
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
            {
                var orderWrapper = await orderService.GetOrderWithOrderItemsForFunding(callOffId, internalOrgId);
                model.SetFundingTypes(orderWrapper.Order.SelectedFramework.FundingTypes);
                return View(model);
            }

            await orderItemService.UpdateOrderItemFunding(callOffId, internalOrgId, catalogueItemId, model.SelectedFundingType);

            return RedirectToAction(
                nameof(FundingSources),
                typeof(FundingSourceController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
