using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderingParty;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/ordering-party")]
    public sealed class OrderingPartyController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderingPartyService orderingPartyService;

        public OrderingPartyController(
            IOrderService orderService,
            IOrderingPartyService orderingPartyService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderingPartyService = orderingPartyService ?? throw new ArgumentNullException(nameof(orderingPartyService));
        }

        [HttpGet]
        public async Task<IActionResult> OrderingParty(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            var model = new OrderingPartyModel(internalOrgId, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderingParty(string internalOrgId, CallOffId callOffId, OrderingPartyModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contact = order.OrderingPartyContact ?? new Contact();

            contact.FirstName = model.FirstName.Trim();
            contact.LastName = model.LastName.Trim();
            contact.Email = model.EmailAddress.Trim();
            contact.Phone = model.TelephoneNumber.Trim();

            await orderingPartyService.SetOrderingPartyContact(callOffId, contact);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
