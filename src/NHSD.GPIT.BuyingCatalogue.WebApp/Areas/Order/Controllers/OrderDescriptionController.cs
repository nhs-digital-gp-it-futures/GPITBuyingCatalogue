using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/description")]
    public sealed class OrderDescriptionController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderDescriptionService orderDescriptionService;

        public OrderDescriptionController(
            IOrderService orderService,
            IOrderDescriptionService orderDescriptionService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderDescriptionService = orderDescriptionService ?? throw new ArgumentNullException(nameof(orderDescriptionService));
        }

        [HttpGet]
        public async Task<IActionResult> OrderDescription(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var descriptionModel = new OrderDescriptionModel(odsCode, order)
            {
                BackLink = Url.Action(
                            nameof(OrderController.Order),
                            typeof(OrderController).ControllerName(),
                            new { odsCode, callOffId }),
            };

            return View(descriptionModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrderDescription(string odsCode, CallOffId callOffId, OrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await orderDescriptionService.SetOrderDescription(callOffId, model.Description);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("~/organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {
            var descriptionModel = new OrderDescriptionModel(odsCode, null)
            {
                BackLink = Url.Action(
                            nameof(OrderController.NewOrder),
                            typeof(OrderController).ControllerName(),
                            new { odsCode }),
            };

            return View("OrderDescription", descriptionModel);
        }

        [HttpPost("~/organisation/{odsCode}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string odsCode, OrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View("OrderDescription", model);

            var order = await orderService.CreateOrder(model.Description, model.OdsCode);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, order.CallOffId });
        }
    }
}
