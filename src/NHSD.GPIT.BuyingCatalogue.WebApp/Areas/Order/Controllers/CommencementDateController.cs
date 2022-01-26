using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/commencement-date")]
    public sealed class CommencementDateController : Controller
    {
        private readonly IOrderService orderService;

        private readonly ICommencementDateService commencementDateService;

        public CommencementDateController(
            IOrderService orderService,
            ICommencementDateService commencementDateService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.commencementDateService = commencementDateService ?? throw new ArgumentNullException(nameof(commencementDateService));
        }

        [HttpGet]
        public async Task<IActionResult> CommencementDate(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, odsCode);

            var model = new CommencementDateModel(odsCode, callOffId, order.CommencementDate)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CommencementDate(string odsCode, CallOffId callOffId, CommencementDateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await commencementDateService.SetCommencementDate(callOffId, odsCode, model.CommencementDate!.Value);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
