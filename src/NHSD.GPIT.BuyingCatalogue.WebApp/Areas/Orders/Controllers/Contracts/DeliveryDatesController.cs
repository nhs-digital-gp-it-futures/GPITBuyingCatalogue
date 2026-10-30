using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/delivery-dates")]
    public class DeliveryDatesController : Controller
    {
        public const string DateFormat = "yyyyMMdd";

        private readonly IDeliveryDateService deliveryDateService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public DeliveryDatesController(
            IDeliveryDateService deliveryDateService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.deliveryDateService = deliveryDateService ?? throw new ArgumentNullException(nameof(deliveryDateService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, string returnUrl = null, bool? setAllPDD = null)
        {
            var orderWrapper = await orderService.GetOrderThin(callOffId, internalOrgId);

            var model = new SelectDateModel(internalOrgId, callOffId, orderWrapper.Order, setAllPDD, orderWrapper.Previous?.DeliveryDate)
            {
                BackLink = returnUrl ?? Url.Action(
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

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (model.ApplyToAll == true)
            {
                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.Date.Value);

                return RedirectToAction(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.Date.Value);

            return RedirectToAction(
                nameof(SelectOrganisations),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select-organisations")]
        public async Task<IActionResult> SelectOrganisations(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            int? orderItemId = orderWrapper.Order.GetOrderItemIds().Count > 0 ? orderWrapper.Order.GetOrderItemIds()[0] : null;
            if (orderItemId is null)
            {
                return NotFound();
            }

            var model = new SelectOrganisationsModel(orderWrapper, internalOrgId, callOffId, orderItemId.Value)
            {
                BackLink = Url.Action(
                    nameof(SelectDate),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select-organisations")]
        public async Task<IActionResult> SelectOrganisations(string internalOrgId, CallOffId callOffId, SelectOrganisationsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var selectedRecipients = string.Join(
                ",",
                model.Recipients.Where(x => x.Selected).Select(x => x.RecipientOrganisationId));

            return RedirectToAction(
                nameof(ChangeOrganisationsDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, selectedRecipients });
        }

        [HttpGet("change-organisations-dates")]
        public async Task<IActionResult> ChangeOrganisationsDates(string internalOrgId, CallOffId callOffId, string selectedRecipients)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            int? orderItemId = orderWrapper.Order.GetOrderItemIds().Count > 0 ? orderWrapper.Order.GetOrderItemIds()[0] : null;
            if (orderItemId is null)
            {
                return NotFound();
            }

            var selectedRecipientsList = selectedRecipients.Split(',').ToList();
            var recipients = await deliveryDateService.GetSelectedRecipientsAsync(callOffId.OrderNumber, selectedRecipientsList);
            var model = new SelectOrganisationsModel()
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                OrderItemId = orderItemId,
                Recipients = [.. recipients.Select(x => new SelectRecipientToEditModel()
                {
                    RecipientOrganisationId = x.RecipientOdsOrganisation.Id,
                    RecipientName = x.RecipientOdsOrganisation.Name,
                    RecipientOdsCode = x.RecipientOdsCode,
                    ParentSublocationName = x.ParentSublocation.SublocationOrganisation.Name,
                    ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                    Selected = true,
                    Status = x.OrderItemSublocationRecipients.Any(x => x.DeliveryDate != orderWrapper.Order.DeliveryDate) ? "Updated" : string.Empty,
                })],
                BackLink = Url.Action(
                    nameof(SelectOrganisations),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("change-organisations-dates")]
        public async Task<IActionResult> ChangeOrganisationsDates(string internalOrgId, CallOffId callOffId)
        {
            return RedirectToAction(
                nameof(Review),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("set-organisation-delivery-dates/{recipientOrganisationId}")]
        public async Task<IActionResult> SetOrganisationDeliveryDates(
            string internalOrgId,
            CallOffId callOffId,
            string recipientOrganisationId,
            string selectedRecipients)
        {
            var orderWrapper = await orderService.GetOrderThin(callOffId, internalOrgId);
            int? orderItemId = orderWrapper.Order.GetOrderItemIds().Count > 0 ? orderWrapper.Order.GetOrderItemIds()[0] : null;
            if (orderItemId is null)
            {
                return NotFound();
            }

            var orderItemRecipients = await deliveryDateService.GetRecipientOrderItemsDeliveryDatesAsync(callOffId.OrderNumber, recipientOrganisationId);

            if (orderItemRecipients is null || !orderItemRecipients.Any())
            {
                return NotFound();
            }

            var model = new SetOrganisationDeliveryDatesModel(
                internalOrgId,
                callOffId,
                orderItemId.Value,
                recipientOrganisationId,
                orderItemRecipients,
                orderWrapper.Order)
            {
                BackLink = string.IsNullOrEmpty(selectedRecipients)
                ? Url.Action(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId })
                : Url.Action(
                    nameof(ChangeOrganisationsDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, selectedRecipients }),
            };

            return View(model);
        }

        [HttpPost("set-organisation-delivery-dates/{recipientOrganisationId}")]
        public async Task<IActionResult> SetOrganisationDeliveryDates(
            string internalOrgId,
            CallOffId callOffId,
            string recipientOrganisationId,
            SetOrganisationDeliveryDatesModel model)
        {
            model.RecipientOrganisationId = recipientOrganisationId;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var recipientOrderItemDeliveryDates = model.OrderItemsDeliveryDates.Select(x => new RecipientOrderItemDeliveryDateDto
            {
                OrderSublocationRecipientId = x.OrderSublocationRecipientId,
                DeliveryDate = x.Date,
            }).ToList();

            await deliveryDateService.SetRecipientOrderItemDeliveryDatesAsync(recipientOrderItemDeliveryDates);

            if (model.SelectedRecipients is null || model.SelectedRecipients.Count == 0)
            {
                return RedirectToAction(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(ChangeOrganisationsDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, model.SelectedRecipients });
        }

        [HttpGet("review")]
        public async Task<IActionResult> Review(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new ReviewModel(orderWrapper)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }
    }
}
