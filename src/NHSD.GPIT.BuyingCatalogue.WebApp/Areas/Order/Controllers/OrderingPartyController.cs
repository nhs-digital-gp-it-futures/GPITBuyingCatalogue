using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/ordering-party")]
    public sealed class OrderingPartyController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderingPartyService orderingPartyService;
        private readonly IContactDetailsService contactDetailsService;

        public OrderingPartyController(
            IOrderService orderService,
            IOrderingPartyService orderingPartyService,
            IContactDetailsService contactDetailsService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderingPartyService = orderingPartyService ?? throw new ArgumentNullException(nameof(orderingPartyService));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
        }

        [HttpGet]
        public async Task<IActionResult> OrderingParty(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, odsCode);

            var model = new OrderingPartyModel(odsCode, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderingParty(string odsCode, CallOffId callOffId, OrderingPartyModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = await orderService.GetOrderThin(callOffId, odsCode);

            var contact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.OrderingPartyContact,
                model.Contact);

            await orderingPartyService.SetOrderingPartyContact(order, contact);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
