using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/delete-order")]
    public sealed class DeleteOrderController : Controller
    {
        private readonly IOrderService orderService;

        public DeleteOrderController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOrder(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteOrderModel(odsCode, order));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(string odsCode, CallOffId callOffId, DeleteOrderModel model)
        {
            await orderService.DeleteOrder(callOffId);

            return RedirectToAction(
                nameof(DashboardController.Organisation),
                typeof(DashboardController).ControllerName(),
                new { odsCode });
        }

        [HttpGet("confirmation")]
        public async Task<IActionResult> DeleteOrderConfirmation(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteConfirmationModel(odsCode, order));
        }
    }
}
