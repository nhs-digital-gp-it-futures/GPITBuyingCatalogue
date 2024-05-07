using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/commencement-date")]
    public sealed class CommencementDateController : Controller
    {
        public const string DateFormat = "yyyyMMdd";
        public const string Delimiter = ".";
        private const int DefaultMaximumTerm = 36;
        private readonly IOrderService orderService;
        private readonly ICommencementDateService commencementDateService;
        private readonly IDeliveryDateService deliveryDateService;

        public CommencementDateController(
            IOrderService orderService,
            ICommencementDateService commencementDateService,
            IDeliveryDateService deliveryDateService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.commencementDateService = commencementDateService ?? throw new ArgumentNullException(nameof(commencementDateService));
            this.deliveryDateService = deliveryDateService ?? throw new ArgumentNullException(nameof(deliveryDateService));
        }

        [HttpGet]
        public async Task<IActionResult> CommencementDate(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            var model = new CommencementDateModel(internalOrgId, order, order.SelectedFramework?.MaximumTerm ?? DefaultMaximumTerm)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CommencementDate(string internalOrgId, CallOffId callOffId, CommencementDateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var affectedDeliveryDates = order.OrderItems
                .SelectMany(x => order.OrderRecipients.Select(r => r.GetDeliveryDateForItem(x.CatalogueItemId)))
                .Count(x => x.HasValue && x.Value < model.Date);

            if (affectedDeliveryDates > 0)
            {
                var date = model.Date!.Value.ToString(DateFormat);
                var details = $"{date}{Delimiter}{model.InitialPeriodValue!.Value}{Delimiter}{model.MaximumTermValue!.Value}";

                return RedirectToAction(
                    nameof(ConfirmChanges),
                    typeof(CommencementDateController).ControllerName(),
                    new { internalOrgId, callOffId, details });
            }

            await commencementDateService.SetCommencementDate(
                callOffId,
                internalOrgId,
                model.Date!.Value,
                model.InitialPeriodValue!.Value,
                model.MaximumTermValue!.Value);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("{details}/confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, string details)
        {
            var parameters = details.Split(Delimiter);
            var newCommencementDate = DateTime.ParseExact(parameters[0], DateFormat, CultureInfo.InvariantCulture);
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var affectedDeliveryDates = order.OrderItems
                .SelectMany(x => order.OrderRecipients.Select(r => r.GetDeliveryDateForItem(x.CatalogueItemId)))
                .Count(x => x.HasValue && x.Value < newCommencementDate);
            var totalDeliveryDates = order.OrderItems
                .SelectMany(x => order.OrderRecipients.Select(r => r.GetDeliveryDateForItem(x.CatalogueItemId)))
                .Count(x => x.HasValue);

            var model = new ConfirmChangesModel
            {
                BackLink = Url.Action(
                    nameof(CommencementDate),
                    typeof(CommencementDateController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CurrentDate = order.CommencementDate!.Value,
                NewDate = newCommencementDate,
                AffectedPlannedDeliveryDates = affectedDeliveryDates,
                TotalPlannedDeliveryDates = totalDeliveryDates,
            };

            return View(model);
        }

        [HttpPost("{details}/confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, string details, ConfirmChangesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ConfirmChanges == false)
            {
                return RedirectToAction(
                    nameof(CommencementDate),
                    typeof(CommencementDateController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var parameters = details.Split(Delimiter);

            await commencementDateService.SetCommencementDate(
                callOffId,
                internalOrgId,
                model.NewDate,
                int.Parse(parameters[1]),
                int.Parse(parameters[2]));

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            await deliveryDateService.ResetDeliveryDates(order.Id, model.NewDate);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
