using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/delete-order")]
    public sealed class DeleteOrderController : Controller
    {
        private readonly IOrderService orderService;

        public DeleteOrderController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOrder(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order == null)
            {
                return RedirectToAction(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId });
            }

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

            if (model.IsAmendment)
            {
                await orderService.HardDeleteOrder(callOffId, internalOrgId);
            }
            else
            {
                await orderService.SoftDeleteOrder(callOffId, internalOrgId);
            }

            return RedirectToAction(
                nameof(DashboardController.Organisation),
                typeof(DashboardController).ControllerName(),
                new { internalOrgId });
        }
    }
}
