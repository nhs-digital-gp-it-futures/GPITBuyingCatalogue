﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}")]
    public class OrderController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;
        private readonly IOrderService orderService;
        private readonly IOrderDescriptionService orderDescriptionService;

        public OrderController(
            ILogWrapper<OrderController> logger,
            IOrderService orderService,
            IOrderDescriptionService orderDescriptionService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderDescriptionService = orderDescriptionService ?? throw new ArgumentNullException(nameof(orderDescriptionService));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await orderService.GetOrder(callOffId);

            return View(new OrderModel(odsCode, order));
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(string odsCode, string callOffId, string print = "false")
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await orderService.GetOrder(callOffId);

            // TODO - obey the print switch
            return View(new SummaryModel(odsCode, order));
        }

        [HttpGet("delete-order")]
        public async Task<IActionResult> DeleteOrder(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteOrderModel(odsCode, order));
        }

        [HttpPost("delete-order")]
        public async Task<IActionResult> DeleteOrder(string odsCode, string callOffId, DeleteOrderModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            await orderService.DeleteOrder(callOffId);

            return Redirect($"/order/organisation/{odsCode}");
        }

        [HttpGet("delete-order/confirmation")]
        public async Task<IActionResult> DeleteOrderConfirmation(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteConfirmationModel(odsCode, order));
        }

        [HttpGet("description")]
        public async Task<IActionResult> OrderDescription(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await orderService.GetOrder(callOffId);

            return View(new OrderDescriptionModel(odsCode, order));
        }

        [HttpPost("description")]
        public async Task<IActionResult> OrderDescription(string odsCode, string callOffId, OrderDescriptionModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            model.ValidateNotNull(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await orderDescriptionService.SetOrderDescription(callOffId, model.Description);

            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }

        [HttpGet("ordering-party")]
        public IActionResult OrderingParty(string odsCode, string callOffId)
        {
            return View(new OrderingPartyModel());
        }

        [HttpPost("ordering-party")]
        public IActionResult OrderingParty(string odsCode, string callOffId, OrderingPartyModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }

        [HttpGet("commencement-date")]
        public IActionResult CommencementDate(string odsCode, string callOffId)
        {
            return View(new CommencementDateModel());
        }

        [HttpPost("commencement-date")]
        public IActionResult CommencementDate(string odsCode, string callOffId, CommencementDateModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }

        [HttpGet("funding-source")]
        public IActionResult FundingSource(string odsCode, string callOffId)
        {
            return View(new FundingSourceModel());
        }

        [HttpPost("funding-source")]
        public IActionResult FundingSource(string odsCode, string callOffId, FundingSourceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }
    }
}
