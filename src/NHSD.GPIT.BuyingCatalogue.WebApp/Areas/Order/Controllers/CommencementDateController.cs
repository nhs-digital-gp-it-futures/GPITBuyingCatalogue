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
            var order = await orderService.GetOrder(callOffId);

            return View(new CommencementDateModel(odsCode, callOffId, order.CommencementDate));
        }

        [HttpPost]
        public async Task<IActionResult> CommencementDate(string odsCode, CallOffId callOffId, CommencementDateModel model)
        {
            (DateTime? date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            await commencementDateService.SetCommencementDate(callOffId, date);

            return RedirectToAction(
                nameof(Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
