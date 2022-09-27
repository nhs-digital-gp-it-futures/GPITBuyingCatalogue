using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/delivery-dates")]
    public class DeliveryDatesController : Controller
    {
        public const string DateFormat = "yyyyMMdd";

        private readonly IOrderService orderService;

        public DeliveryDatesController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, string deliveryDate = null)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var currentDate = deliveryDate == null
                ? order.DeliveryDate
                : DateTime.ParseExact(deliveryDate, DateFormat, CultureInfo.InvariantCulture);

            var model = new SelectDateModel(internalOrgId, callOffId, order.CommencementDate!.Value, currentDate)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, SelectDateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solution = order.GetSolution();

            if (order.DeliveryDate == null
                || order.DeliveryDate.Value == model.Date!.Value)
            {
                if (order.DeliveryDate == null)
                {
                    await orderService.SetDeliveryDate(internalOrgId, callOffId, model.Date!.Value);
                }

                return RedirectToAction(
                    nameof(EditDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, solution.CatalogueItemId });
            }

            var deliveryDate = model.Date?.ToString(DateFormat);

            return RedirectToAction(
                nameof(ConfirmChanges),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, deliveryDate });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, string deliveryDate)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var newDate = DateTime.ParseExact(deliveryDate, DateFormat, CultureInfo.InvariantCulture);

            var model = new ConfirmChangesModel(internalOrgId, callOffId, order.DeliveryDate!.Value, newDate)
            {
                BackLink = Url.Action(
                    nameof(SelectDate),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, deliveryDate }),
            };

            return View(model);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, ConfirmChangesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ConfirmChanges == false)
            {
                return RedirectToAction(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            await orderService.SetDeliveryDate(internalOrgId, callOffId, model.NewDeliveryDate);

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solution = order.GetSolution();

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, solution.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new EditDatesModel(order, catalogueItemId)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, EditDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var deliveryDates = model.Recipients
                .Select(x => new OrderDeliveryDateDto(x.OdsCode, x.Date!.Value))
                .ToList();

            await orderService.SetDeliveryDates(order.Id, catalogueItemId, deliveryDates);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
