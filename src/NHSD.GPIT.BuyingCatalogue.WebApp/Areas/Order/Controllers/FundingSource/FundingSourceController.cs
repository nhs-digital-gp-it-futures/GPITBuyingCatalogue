﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/funding-sources")]
    public sealed class FundingSourceController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderItemService orderItemService;

        public FundingSourceController(IOrderService orderService, IOrderItemService orderItemService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet]
        public async Task<IActionResult> FundingSources(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithOrderItemsForFunding(callOffId, internalOrgId);

            var model = new FundingSources(internalOrgId, callOffId, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpGet("{catalogueItemId}/funding-source")]
        public async Task<IActionResult> FundingSource(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

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
