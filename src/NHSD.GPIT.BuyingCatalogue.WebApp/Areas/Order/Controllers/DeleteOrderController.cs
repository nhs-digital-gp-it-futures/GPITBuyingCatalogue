using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/delete-order")]
    public sealed class DeleteOrderController : Controller
    {
        internal const string AdviceText = "The order will be permanently deleted from your organisation’s dashboard.";

        internal const string WarningText =
            "Deleting an order is permanent and any information you’ve already inputted will be lost. Once you’ve deleted your order, you’ll not be able to retrieve it and will have to start a new one.";

        private readonly IOrderService orderService;

        public DeleteOrderController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOrder(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            if (order.OrderStatus == OrderStatus.Completed)
            {
                return RedirectToAction(
                    nameof(OrderController.Summary),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = new DeleteOrderModel(order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                AdviceText = AdviceText,
                WarningText = WarningText,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(string internalOrgId, CallOffId callOffId, DeleteOrderModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.SelectedOption!.Value)
            {
                return RedirectToAction(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            await orderService.DeleteOrder(callOffId, internalOrgId);

            return RedirectToAction(
                nameof(DashboardController.Organisation),
                typeof(DashboardController).ControllerName(),
                new { internalOrgId });
        }
    }
}
