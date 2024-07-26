using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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
        private readonly IOdsService odsService;

        public DeliveryDatesController(
            IDeliveryDateService deliveryDateService,
            IOrderService orderService,
            IRoutingService routingService,
            IOdsService odsService)
        {
            this.deliveryDateService = deliveryDateService ?? throw new ArgumentNullException(nameof(deliveryDateService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, string returnUrl = null, bool? setAllPDD = null)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            var model = new SelectDateModel(internalOrgId, callOffId, order, setAllPDD)
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

            if (order.DeliveryDate.HasValue && order.DeliveryDate.Value != model.Date!.Value)
            {
                var deliveryDate = model.Date?.ToString(DateFormat);

                return RedirectToAction(
                    nameof(ConfirmChanges),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, deliveryDate, model.ApplyToAll });
            }

            if (model.ApplyToAll == true)
            {
                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.Date!.Value);

                return RedirectToAction(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            if (order.DeliveryDate == null)
            {
                await deliveryDateService.SetDeliveryDate(internalOrgId, callOffId, model.Date!.Value);
            }
            else
            {
                await deliveryDateService.ResetRecipientDeliveryDates(order.Id);
            }

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, CatalogueItemId = order.GetOrderItemIds().First() });
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

                    var catalogueItemId = order.GetOrderItemIds().First();

                    return RedirectToAction(
                        nameof(EditDates),
                        typeof(DeliveryDatesController).ControllerName(),
                        new { model.InternalOrgId, model.CallOffId, catalogueItemId });
                }

                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.NewDeliveryDate);
            }

            return RedirectToAction(
                nameof(Review),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("{catalogueItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            // If there are no new recipients for this item (e.g. the original solution in a amend)
            if (orderWrapper.DetermineOrderRecipients(catalogueItemId).IsNullOrEmpty())
            {
                var next = routingService.GetRoute(
                    RoutingPoint.EditDeliveryDates,
                    orderWrapper,
                    new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

                return RedirectToAction(next.ActionName, next.ControllerName, next.RouteValues);
            }

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDatesBackLink,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });
            
            var orderRecipients = orderWrapper.DetermineOrderRecipients(catalogueItemId);
            var organisations = await odsService.GetServiceRecipientsById(internalOrgId, orderRecipients.Select(x => x.OdsCode));

            var model = new EditDatesModel(orderWrapper, catalogueItemId, organisations, source)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
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

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            var recipients = model.Recipients.SelectMany(x => x.Value).ToList();
                
             var deliveryDates   = recipients
                .Select(x => new RecipientDeliveryDateDto(x.OdsCode, x.Date!.Value))
                .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, catalogueItemId, deliveryDates);

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDates,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("{catalogueItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var catalogueItem = order.OrderItem(catalogueItemId).CatalogueItem;
            var previousCatalogueItemId = order.GetPreviousOrderItemId(catalogueItemId)!.Value;

            var model = new MatchDatesModel(internalOrgId, callOffId, catalogueItem)
            {
                BackLink = Url.Action(
                    nameof(EditDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId = previousCatalogueItemId }),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, MatchDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var solutionId = order.GetSolutionId();

            var recipients = wrapper.DetermineOrderRecipients(catalogueItemId);
            var dates = (model.MatchDates == true && solutionId is not null)
                ? recipients
                    .Select(
                        x => new RecipientDeliveryDateDto(
                            x.OdsCode,
                            x.GetDeliveryDateForItem(solutionId.Value)!.Value))
                    .ToList()
                : recipients
                    .Select(x => new RecipientDeliveryDateDto(x.OdsCode, order.DeliveryDate!.Value))
                    .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, catalogueItemId, dates);

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
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
