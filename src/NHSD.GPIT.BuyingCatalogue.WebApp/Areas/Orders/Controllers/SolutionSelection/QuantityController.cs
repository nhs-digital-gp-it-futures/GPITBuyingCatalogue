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
            CatalogueItemId catalogueItemId)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            List<ServiceRecipientQuantityDto> recipientDtos = GetRecipientDtos(orderRecipients, orderItem);

            var model = new SublocationQuantityHubModel(
                order.OrderingParty,
                orderItem.CatalogueItem,
                recipientDtos,
                RoutingDestination.Order,
                orderItem.OrderItemPrice)
            {
                BackLink = Url.Action(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId }),
                Caption = $"Order {callOffId}",
                RoutingFields = new RoutingFields
                {
                    CatalogueItem = catalogueItemId, InternalOrgId = internalOrgId, CallOffId = callOffId,
                },
            };

            return View(SublocationHubViewName, model);
        }

        [HttpPost]
        public async Task<IActionResult> SublocationHub(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SublocationQuantityHubModel model)
        {
            _ = model;

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            if (orderRecipients.All(x => x.GetQuantityForItem(catalogueItemId) is not null))
                return RedirectToAction(nameof(ConfirmQuantities), new { internalOrgId, callOffId, catalogueItemId });

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("{parentOdsCode}")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SelectServiceSublocationRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string parentOdsCode,
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            List<ServiceRecipientQuantityDto> recipientDtos = GetRecipientDtos(orderRecipients, orderItem, parentOdsCode);

            IEnumerable<ServiceRecipientQuantityDto> previousRecipients =
                GetPreviousRecipients(wrapper, orderItem, parentOdsCode);

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
                    new { internalOrgId, callOffId, catalogueItemId, source }),
                Source = source,
            };

            if (orderItem.OrderItemPrice.ProvisioningType != ProvisioningType.Patient)
            {
                return View(ServiceSublocationRecipientViewName, model);
            }

            var solution = order.GetSolutionOrderItem();
            if (solution?.OrderItemPrice?.ProvisioningType is ProvisioningType.Patient
                && solution.CatalogueItemId != catalogueItemId)
            {
                await SetPracticeSizes(model, parentOdsCode, solution, wrapper.DetermineOrderRecipients(solution.CatalogueItemId));
            }
            else
            {
                await SetPracticeSizes(model, parentOdsCode);
            }

            return View(ServiceSublocationRecipientViewName, model);
        }

        [HttpPost("{parentOdsCode}")]
        [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
        public async Task<IActionResult> SelectServiceSublocationRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string parentOdsCode,
            SelectServiceRecipientQuantityModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderWrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            List<OrderItemRecipientQuantityDto> quantities = model.SubLocations[0].ServiceRecipients
                .Select(x => new OrderItemRecipientQuantityDto
                {
                    ParentSublocationOdsCode = parentOdsCode,
                    RecipientOdsCode = x.RecipientOdsCode,
                    Quantity = string.IsNullOrWhiteSpace(x.InputQuantity)
                        ? null
                        : int.Parse(x.InputQuantity),
                })
                .ToList();

            await orderQuantityService.SetServiceRecipientQuantities(order.Id, catalogueItemId, quantities);

            await orderItemService.DetectChangesInFundingAndDelete(callOffId, internalOrgId, catalogueItemId);

            return RedirectToAction(
                nameof(SublocationHub),
                typeof(QuantityController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        [HttpGet("view")]
        public async Task<IActionResult> ViewServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Previous;
            IEnumerable<OrderSublocationRecipient> recipients = order.FlattenedRecipients;
            var orderItem = order.OrderItem(catalogueItemId);

            var model = new ViewServiceRecipientQuantityModel(orderItem, recipients)
            {
                BackLink = Url.Action(
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
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            List<ServiceRecipientQuantityDto> recipientDtos = GetRecipientDtos(orderRecipients, orderItem);

            var model = new ConfirmQuantitiesModel(
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipientDtos)
            {
                BackLink = Url.Action(
                    nameof(SublocationHub),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, source }),
                ContinueLink = Url.Action(
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
                        orderRecipient.GetQuantityForItem(orderItem.CatalogueItemId),
                        orderRecipient.ParentSublocation.SublocationOrganisation?.Name))
                .ToList();
        }

        private static IEnumerable<ServiceRecipientQuantityDto> GetPreviousRecipients(
            OrderWrapper wrapper,
            OrderItem orderItem,
            string parentOdsCode = null)
        {
            return wrapper.Previous?.FlattenedRecipients
                ?.Where(x =>
                    x.OrderItemSublocationRecipients.Any(y => y.CatalogueItemId == orderItem.CatalogueItemId) &&
                    (parentOdsCode is null || x.ParentSublocationOdsCode == parentOdsCode))
                .Select(x => new ServiceRecipientQuantityDto(
                    x.ParentSublocationOdsCode,
                    x.RecipientOdsCode,
                    x.RecipientOdsOrganisation?.Name,
                    x.GetQuantityForItem(orderItem.CatalogueItemId)));
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

                    var existing = recipients
                        ?.FirstOrDefault(x =>
                            x.RecipientOdsCode == serviceRecipient.RecipientOdsCode && x.ParentSublocationOdsCode
                            == serviceRecipient.ParentSublocationOdsCode)
                        ?.GetQuantityForItem(solution.CatalogueItemId);

                    if (existing.HasValue)
                    {
                        serviceRecipient.InputQuantity = $"{existing.Value}";
                    }
                    else
                    {
                        if (practiceSizes.TryGetValue(serviceRecipient.RecipientOdsCode, out var quantity))
                        {
                            serviceRecipient.InputQuantity = $"{quantity}";
                        }
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
