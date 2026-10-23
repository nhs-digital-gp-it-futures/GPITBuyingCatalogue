using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.Date!.Value);

                return RedirectToAction(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            if (order.DeliveryDate.HasValue && order.DeliveryDate.Value != model.Date!.Value)
            {
                var deliveryDate = model.Date?.ToString(DateFormat);

                return RedirectToAction(
                    nameof(ConfirmChanges),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, deliveryDate, model.ApplyToAll });
            }

            await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.Date!.Value);

            return RedirectToAction(
                nameof(SelectOrganisations),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, string deliveryDate, bool applyToAll)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var newDate = DateTime.ParseExact(deliveryDate, DateFormat, CultureInfo.InvariantCulture);

            var model = new ConfirmChangesModel(internalOrgId, callOffId, order.DeliveryDate!.Value, newDate, applyToAll)
            {
                BackLink = Url.Action(
                    nameof(SelectDate),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, applyToAll }),
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

            if (model.ConfirmChanges == true)
            {
                if (!model.ApplyToAll)
                {
                    var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

                    await deliveryDateService.SetDeliveryDate(internalOrgId, callOffId, model.NewDeliveryDate);
                    await deliveryDateService.ResetRecipientDeliveryDates(order.Id);

                    int? orderItemId = order.GetOrderItemIds().Count > 0 ? order.GetOrderItemIds()[0] : null;

                    return RedirectToAction(
                        nameof(EditDates),
                        typeof(DeliveryDatesController).ControllerName(),
                        new { model.InternalOrgId, model.CallOffId, orderItemId });
                }

                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.NewDeliveryDate);
            }

            return RedirectToAction(
                nameof(Review),
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

        [HttpGet("{orderItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, int orderItemId, RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderItem = orderWrapper.Order.OrderItem(orderItemId);
            var item = orderItem.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                ? orderItem.Parent
                : orderItem;

            // If there are no new recipients for this item (e.g. the original solution in an amend)
            if (orderWrapper.DetermineOrderRecipients(item) is null or { Count: 0 })
            {
                RoutingResult next = routingService.GetRoute(
                    RoutingPoint.EditDeliveryDates,
                    orderWrapper,
                    new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { Source = source, OrderItemId = orderItemId });

                return RedirectToAction(next.ActionName, next.ControllerName, next.RouteValues);
            }

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDatesBackLink,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { Source = source, OrderItemId = orderItemId });

            var model = new EditDatesModel(orderWrapper, orderItemId, source)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
            };

            return View(model);
        }

        [HttpPost("{orderItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, int orderItemId, EditDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var orderItem = order.OrderItem(orderItemId);

            var recipients = model.Recipients.SelectMany(x => x.Value).ToList();

            var deliveryDates = recipients
                .Select(x => new RecipientDeliveryDateDto(x.OdsCode, x.Date!.Value))
                .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, orderItem, deliveryDates);

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDates,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { Source = model.Source, OrderItemId = orderItemId });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("{orderItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, int orderItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var orderItem = order.OrderItem(orderItemId);
            var catalogueItem = orderItem.CatalogueItem;
            var previousCatalogueItemId = order.GetPreviousOrderItemId(orderItemId)!.Value;

            var model = new MatchDatesModel(internalOrgId, callOffId, catalogueItem)
            {
                BackLink = Url.Action(
                    nameof(EditDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, orderItemId = previousCatalogueItemId }),
            };

            return View(model);
        }

        [HttpPost("{orderItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, int orderItemId, MatchDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var solutionOrderItem = order.GetSolutionOrderItem();
            var orderItem = order.OrderItem(orderItemId);

            var recipients = wrapper.DetermineOrderRecipients(orderItem);
            List<RecipientDeliveryDateDto> dates = model.MatchDates == true && solutionOrderItem is not null
                ? recipients
                    .Select(x => new RecipientDeliveryDateDto(
                        x.RecipientOdsCode,
                        x.GetDeliveryDateForItem(solutionOrderItem.Id).GetValueOrDefault()))
                    .ToList()
                : recipients
                    .Select(x => new RecipientDeliveryDateDto(x.RecipientOdsCode, order.DeliveryDate!.Value))
                    .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, orderItem, dates);

            if (orderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                && orderItem.Services.Count > 0)
            {
                foreach (var service in orderItem.Services)
                {
                    await deliveryDateService.SetDeliveryDates(order.Id, service, dates);
                }
            }

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, orderItemId });
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
