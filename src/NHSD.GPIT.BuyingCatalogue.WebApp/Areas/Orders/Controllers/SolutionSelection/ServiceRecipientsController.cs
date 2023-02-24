using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/item/{catalogueItemId}/service-recipients")]
    public class ServiceRecipientsController : Controller
    {
        public const char Separator = ',';

        private readonly IOdsService odsService;
        private readonly IOrderItemRecipientService orderItemRecipientService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;
        private readonly IOrganisationsService organisationsService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderItemService orderItemService,
            IOrderItemRecipientService orderItemRecipientService,
            IOrderService orderService,
            IRoutingService routingService,
            IOrganisationsService organisationsService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderItemRecipientService = orderItemRecipientService ?? throw new ArgumentNullException(nameof(orderItemRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
            this.organisationsService =
                organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectionMode? selectionMode = null,
            RoutingSource? source = null,
            string recipientIds = null,
            string importedRecipients = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);
            var previousItem = wrapper.Previous?.OrderItem(catalogueItemId);
            var previousRecipients = previousItem?.OrderItemRecipients?.Select(x => x.OdsCode) ?? Enumerable.Empty<string>();
            var serviceRecipients = await GetServiceRecipients(internalOrgId);
            var splitImportedRecipients = importedRecipients?.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            var route = routingService.GetRoute(
                RoutingPoint.SelectServiceRecipientsBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var model = new SelectRecipientsModel(organisation, orderItem, previousItem, serviceRecipients, selectionMode, splitImportedRecipients)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Source = source,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                HasMissingImportedRecipients = splitImportedRecipients?.Any(previousRecipients.Contains) ?? false,
            };

            if (recipientIds is null
                && splitImportedRecipients is null)
            {
                var baseOrderItem = model.AssociatedServicesOnly
                    ? order.GetAssociatedServices().FirstOrDefault()
                    : wrapper.RolledUp.GetSolution();

                model.PreSelectRecipients(baseOrderItem);
            }

            if (recipientIds != null)
            {
                model.SelectRecipientIds(recipientIds);
            }

            return View("SelectRecipients", model);
        }

        [HttpPost("add")]
        public IActionResult AddServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var recipientIds = string.Join(
                Separator,
                model.GetServiceRecipients().Where(x => x.Selected).Select(x => x.OdsCode));

            return RedirectToAction(
                nameof(ConfirmChanges),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, recipientIds, journey = JourneyType.Add, model.Source });
        }

        [HttpGet("edit")]
        public async Task<IActionResult> EditServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectionMode? selectionMode = null,
            RoutingSource? source = null,
            string recipientIds = null,
            string importedRecipients = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);
            var previousItem = wrapper.Previous?.OrderItem(catalogueItemId);
            var previousRecipients = previousItem?.OrderItemRecipients?.Select(x => x.OdsCode) ?? Enumerable.Empty<string>();
            var serviceRecipients = await GetServiceRecipients(internalOrgId);
            var splitImportedRecipients = importedRecipients?.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            var route = routingService.GetRoute(
                RoutingPoint.SelectServiceRecipientsBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var model = new SelectRecipientsModel(organisation, orderItem, previousItem, serviceRecipients, selectionMode, splitImportedRecipients)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Source = source,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                HasMissingImportedRecipients = splitImportedRecipients?.Any(previousRecipients.Contains) ?? false,
                IsAdding = false,
            };

            if (orderItem == null
                && selectionMode == null
                && recipientIds == null
                && importedRecipients == null
                && !order.AssociatedServicesOnly)
            {
                model.PreSelectSolutionServiceRecipients(wrapper.RolledUp, catalogueItemId);
            }

            if (recipientIds != null)
            {
                model.SelectRecipientIds(recipientIds);
            }

            return View("SelectRecipients", model);
        }

        [HttpPost("edit")]
        public IActionResult EditServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var odsCodes = model.GetServiceRecipients()?
                .Where(x => x.Selected)
                .Select(x => x.OdsCode)
                .ToList() ?? Enumerable.Empty<string>().ToList();

            if (!odsCodes.Any())
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var recipientIds = string.Join(Separator, odsCodes);

            return RedirectToAction(
                nameof(ConfirmChanges),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId, recipientIds, journey = JourneyType.Edit, model.Source });
        }

        [HttpGet("confirm-changes")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string recipientIds,
            JourneyType journey,
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.RolledUp;
            var catalogueItem = order.OrderItem(catalogueItemId).CatalogueItem;
            var serviceRecipients = await GetServiceRecipients(internalOrgId);
            var selectedRecipientIds = recipientIds?.Split(Separator, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();

            var selected = serviceRecipients
                .Where(x => selectedRecipientIds.Contains(x.OdsCode))
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OdsCode, Location = x.Location })
                .ToList();

            var previouslySelectedIds = wrapper.Previous?.OrderItem(catalogueItemId)
                ?.OrderItemRecipients.Select(x => x.OdsCode)
                .ToList() ?? Enumerable.Empty<string>();

            var previouslySelected = serviceRecipients
                .Where(x => previouslySelectedIds.Contains(x.OdsCode))
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OdsCode, Location = x.Location })
                .ToList();

            var route = routingService.GetRoute(
                RoutingPoint.ConfirmServiceRecipientsBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId)
                {
                    RecipientIds = recipientIds,
                    Journey = journey,
                    Source = source,
                });

            var model = new ConfirmChangesModel(order.OrderingParty)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                Caption = catalogueItem.Name,
                Advice = callOffId.IsAmendment
                    ? string.Format(ConfirmChangesModel.AdditionalAdviceText, catalogueItem.CatalogueItemType.Name())
                    : string.Format(ConfirmChangesModel.AdviceText, catalogueItem.CatalogueItemType.Name()),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Journey = journey,
                Source = source,
                Selected = selected,
                PreviouslySelected = previouslySelected,
            };

            return View(model);
        }

        [HttpPost("confirm-changes")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            ConfirmChangesModel model)
        {
            var wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);
            var order = wrapper.Order;

            await orderItemService.CopyOrderItems(internalOrgId, callOffId, new[] { catalogueItemId });

            await orderItemRecipientService.UpdateOrderItemRecipients(
                order.Id,
                catalogueItemId,
                model.Selected.Select(x => x.Dto).ToList());

            var route = routingService.GetRoute(
                RoutingPoint.ConfirmServiceRecipients,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId)
                {
                    FromPreviousRevision = callOffId.IsAmendment && wrapper.Previous?.OrderItem(catalogueItemId) != null,
                    Source = model.Source,
                });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        private async Task<List<ServiceRecipientModel>> GetServiceRecipients(string internalOrgId)
        {
            var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

            return recipients
                .OrderBy(x => x.Name)
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                    Location = x.Location,
                })
                .ToList();
        }
    }
}
