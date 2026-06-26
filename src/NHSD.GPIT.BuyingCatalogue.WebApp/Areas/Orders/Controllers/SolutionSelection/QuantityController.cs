using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/quantity/{catalogueItemId}/sublocations")]
    public class QuantityController : Controller
    {
        private const string SublocationHubViewName = "QuantitySelection/SublocationHub";
        private const string ServiceSublocationRecipientViewName = "QuantitySelection/SelectServiceSublocationRecipientQuantity";

        private readonly IGpPracticeService gpPracticeService;
        private readonly IOrderService orderService;
        private readonly IOrderQuantityService orderQuantityService;
        private readonly IOrderItemService orderItemService;

        public QuantityController(
            IGpPracticeService gpPracticeService,
            IOrderService orderService,
            IOrderQuantityService orderQuantityService,
            IOrderItemService orderItemService)
        {
            this.gpPracticeService = gpPracticeService ?? throw new ArgumentNullException(nameof(gpPracticeService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderQuantityService = orderQuantityService ?? throw new ArgumentNullException(nameof(orderQuantityService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SublocationHub(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(orderItemId);

            var item = source == RoutingSource.ManageAssociatedServices ? orderItem.Parent : orderItem;
            var caption = GetCaption(source, orderItem, callOffId);

            var orderRecipients = wrapper.DetermineOrderRecipients(item);

            List<ServiceRecipientQuantityDto> recipientDtos = GetRecipientDtos(orderRecipients, orderItem);

            var model = new SublocationQuantityHubModel(
                order.OrderingParty,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice)
            {
                BackLink = source == RoutingSource.ManageAssociatedServices
                ? Url.Action(
                    nameof(AssociatedServicesController.ManageAssociatedServices),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { internalOrgId, callOffId, item.CatalogueItemId })
                : Url.Action(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId }),
                Caption = caption,
                Source = source,
                SubLocations = CreateSublocationHelper.CreateSubLocations(recipientDtos ?? [])
                    .Select(sublocation => new SubLocationModel(sublocation)
                    {
                        ForwardingLink = Url.Action(
                            nameof(SelectServiceSublocationRecipientQuantity),
                            typeof(QuantityController).ControllerName(),
                            new { internalOrgId, sublocation.OdsCode, callOffId, catalogueItemId, orderItemId, source }),
                    }).ToArray(),
            };

            return View(SublocationHubViewName, model);
        }

        [HttpPost]
        public async Task<IActionResult> SublocationHub(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            SublocationQuantityHubModel model)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(orderItemId);
            var item = model.Source == RoutingSource.ManageAssociatedServices ? orderItem.Parent : orderItem;

            var orderRecipients = wrapper.DetermineOrderRecipients(item);

            if (orderRecipients.All(x => x.GetQuantityForItem(orderItemId) is not null))
                return RedirectToAction(nameof(ConfirmQuantities), new { internalOrgId, callOffId, catalogueItemId, orderItemId, source = model.Source });

            return model.Source == RoutingSource.ManageAssociatedServices
            ? RedirectToAction(
                nameof(AssociatedServicesController.ManageAssociatedServices),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId = orderItem.Parent.CatalogueItemId })
            : RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("{odsCode}")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SelectServiceSublocationRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string odsCode,
            int orderItemId,
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(orderItemId);
            var item = source == RoutingSource.ManageAssociatedServices ? orderItem.Parent : orderItem;
            var caption = GetCaption(source, orderItem, callOffId);

            var orderRecipients = wrapper.DetermineOrderRecipients(item);

            List<ServiceRecipientQuantityDto> recipientDtos = GetRecipientDtos(orderRecipients, orderItem, odsCode);

            IEnumerable<ServiceRecipientQuantityDto> previousRecipients =
                GetPreviousRecipients(wrapper, orderItem, odsCode);

            var practiceReorganisation = order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient;

            var model = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                practiceReorganisation,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipientDtos,
                previousRecipients)
            {
                BackLink = Url.Action(
                    nameof(SublocationHub),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, orderItemId, source }),
                Source = source,
                Caption = caption,
            };

            if (orderItem.OrderItemPrice.ProvisioningType != ProvisioningType.Patient)
            {
                return View(ServiceSublocationRecipientViewName, model);
            }

            var solution = order.GetSolutionOrderItem();
            if (solution?.OrderItemPrice?.ProvisioningType is ProvisioningType.Patient
                && solution.CatalogueItemId != catalogueItemId)
            {
                await SetPracticeSizes(model, odsCode, solution, wrapper.DetermineOrderRecipients(solution));
            }
            else
            {
                await SetPracticeSizes(model, odsCode);
            }

            return View(ServiceSublocationRecipientViewName, model);
        }

        [HttpPost("{odsCode}")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SelectServiceSublocationRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string odsCode,
            int orderItemId,
            SelectServiceRecipientQuantityModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ServiceSublocationRecipientViewName, model);
            }

            var orderWrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            List<OrderItemRecipientQuantityDto> quantities = model.SubLocations[0].ServiceRecipients
                .Select(x => new OrderItemRecipientQuantityDto
                {
                    ParentSublocationOdsCode = odsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = string.IsNullOrWhiteSpace(x.InputQuantity)
                        ? null
                        : int.Parse(x.InputQuantity),
                })
                .ToList();

            await orderQuantityService.SetServiceRecipientQuantities(order.Id, orderItemId, quantities);

            await orderItemService.DetectChangesInFundingAndDelete(callOffId, internalOrgId, orderItemId);

            return RedirectToAction(
                nameof(SublocationHub),
                typeof(QuantityController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, orderItemId, model.Source });
        }

        [HttpGet("view")]
        public async Task<IActionResult> ViewServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            RoutingSource? source = null)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).PreviousOrders.LastOrDefault();

            if (order == null) return NotFound();

            IEnumerable<OrderSublocationRecipient> recipients = order.FlattenedRecipients;
            var orderItem = source == RoutingSource.ManageAssociatedServices
                ? order.OrderItem(orderItemId)
                : order.OrderItem(catalogueItemId);

            var model = new ViewServiceRecipientQuantityModel(orderItem, recipients)
            {
                BackLink = source == RoutingSource.ManageAssociatedServices
                    ? Url.Action(
                        nameof(AssociatedServicesController.ManageAssociatedServices),
                        typeof(AssociatedServicesController).ControllerName(),
                        new { internalOrgId, callOffId, catalogueItemId })
                    : Url.Action(
                        nameof(TaskListController.TaskList),
                        typeof(TaskListController).ControllerName(),
                        new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            return View(model);
        }

        [HttpGet("view/{quantityViewCallOffId}")]
        public async Task<IActionResult> ViewServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            CallOffId quantityViewCallOffId,
            RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(quantityViewCallOffId, internalOrgId);
            var orderItem = source == RoutingSource.ManageAssociatedServices
                ? orderWrapper.Order.OrderItem(orderItemId)
                : orderWrapper.Order.OrderItem(catalogueItemId);

            var recipients = orderWrapper.DetermineOrderRecipients(orderItem);

            var model = new ViewServiceRecipientQuantityModel(orderItem, recipients)
            {
                BackLink = source == RoutingSource.ManageAssociatedServices
                    ? Url.Action(
                        nameof(AssociatedServicesController.ManageAssociatedServices),
                        typeof(AssociatedServicesController).ControllerName(),
                        new { internalOrgId, callOffId, catalogueItemId })
                    : Url.Action(
                        nameof(TaskListController.TaskList),
                        typeof(TaskListController).ControllerName(),
                        new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            return View(model);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmQuantities(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(orderItemId);
            var item = source == RoutingSource.ManageAssociatedServices ? orderItem.Parent : orderItem;
            var caption = GetCaption(source, orderItem, callOffId);

            var orderRecipients = wrapper.DetermineOrderRecipients(item);

            List<ServiceRecipientQuantityDto> recipientDtos = GetRecipientDtos(orderRecipients, orderItem);

            var model = new ConfirmQuantitiesModel(
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipientDtos)
            {
                Caption = caption,
                BackLink = Url.Action(
                    nameof(SublocationHub),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, orderItemId, source }),
                ContinueLink = source == RoutingSource.ManageAssociatedServices
                    ? Url.Action(
                        nameof(AssociatedServicesController.ManageAssociatedServices),
                        typeof(AssociatedServicesController).ControllerName(),
                        new { internalOrgId, callOffId, catalogueItemId = item.CatalogueItemId })
                    : Url.Action(
                        nameof(TaskListController.TaskList),
                        typeof(TaskListController).ControllerName(),
                        new { internalOrgId, callOffId }),
            };

            return View("QuantitySelection/ConfirmQuantities", model);
        }

        internal static List<ServiceRecipientQuantityDto> GetRecipientDtos(
            ICollection<OrderSublocationRecipient> orderRecipients,
            OrderItem orderItem,
            string parentOdsCode = null)
        {
            return orderRecipients
                .Where(orderRecipient => parentOdsCode is null || orderRecipient.ParentSublocationOdsCode == parentOdsCode)
                .Select(orderRecipient =>
                    new ServiceRecipientQuantityDto(
                        orderRecipient.ParentSublocationOdsCode,
                        orderRecipient.RecipientOdsCode,
                        orderRecipient.RecipientOdsOrganisation?.Name,
                        orderRecipient.GetQuantityForItem(orderItem.Id),
                        orderRecipient.ParentSublocation.SublocationOrganisation?.Name))
                .ToList();
        }

        private static string GetCaption(RoutingSource? source, OrderItem orderItem, CallOffId callOffId)
        {
            return source == RoutingSource.ManageAssociatedServices
                ? $"{orderItem.Parent.CatalogueItem.Name} - {orderItem.CatalogueItem.Name}"
                : $"Order {callOffId}";
        }

        private static IEnumerable<ServiceRecipientQuantityDto> GetPreviousRecipients(
            OrderWrapper wrapper,
            OrderItem orderItem,
            string parentOdsCode = null)
        {
            return wrapper.Previous?.FlattenedRecipients
                ?.Where(x =>
                    x.OrderItemSublocationRecipients.Any(y => y.OrderItemId == orderItem.Id) &&
                    (parentOdsCode is null || x.ParentSublocationOdsCode == parentOdsCode))
                .Select(x => new ServiceRecipientQuantityDto(
                    x.ParentSublocationOdsCode,
                    x.RecipientOdsCode,
                    x.RecipientOdsOrganisation?.Name,
                    x.GetQuantityForItem(orderItem.Id)));
        }

        private async Task SetPracticeSizes(
            SelectServiceRecipientQuantityModel model,
            string parentOdsCode = null,
            OrderItem solution = null,
            ICollection<OrderSublocationRecipient> recipients = null)
        {
            var odsCodes = model.SubLocations.SelectMany(x => x.ServiceRecipients)
                .Where(x => x.Quantity == 0 && (parentOdsCode is null || x.ParentSublocationOdsCode == parentOdsCode))
                .Select(x => x.RecipientOdsCode)
                .ToArray();
            var practiceSizes =
                (await gpPracticeService.GetNumberOfPatients(odsCodes)).ToDictionary(
                    x => x.OdsCode,
                    x => x.NumberOfPatients);

            foreach ((SubLocationModel location, var index) in model.SubLocations.Select((x, i) => (x, i)))
            {
                foreach (var serviceRecipient in location.ServiceRecipients)
                {
                    if (serviceRecipient.Quantity > 0)
                    {
                        continue;
                    }

                    var existing = solution != null
                        ? recipients
                            ?.FirstOrDefault(x =>
                                x.RecipientOdsCode == serviceRecipient.RecipientOdsCode && x.ParentSublocationOdsCode
                                == serviceRecipient.ParentSublocationOdsCode)
                            ?.GetQuantityForItem(solution.Id)
                        : null;

                    if (existing.HasValue)
                    {
                        serviceRecipient.InputQuantity = $"{existing.Value}";
                    }
                    else if (practiceSizes.TryGetValue(serviceRecipient.RecipientOdsCode, out var quantity))
                    {
                        serviceRecipient.InputQuantity = $"{quantity}";
                    }
                }

                model.SubLocations[index].ServiceRecipients = model.SubLocations[index].ServiceRecipients
                    .OrderBy(x => x.Quantity == 0 ? 0 : 1)
                    .ThenBy(x => x.Name)
                    .ToArray();
            }
        }
    }
}
