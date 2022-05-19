using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/item/{catalogueItemId}")]
    public class ServiceRecipientsController : Controller
    {
        private readonly IOdsService odsService;
        private readonly IOrderItemRecipientService orderItemRecipientService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderItemRecipientService orderItemRecipientService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemRecipientService = orderItemRecipientService ?? throw new ArgumentNullException(nameof(orderItemRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("service-recipients")]
        public async Task<IActionResult> ServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectionMode? selectionMode)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderItem = order.OrderItem(catalogueItemId);
            var serviceRecipients = await GetServiceRecipients(internalOrgId);

            var model = new SelectRecipientsModel(orderItem, serviceRecipients, selectionMode)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
            };

            model.PreSelectRecipients(order.GetSolution());

            return View("SelectRecipients", model);
        }

        [HttpPost("service-recipients")]
        public async Task<IActionResult> ServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var order = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);

            await AddServiceRecipients(order.Id, catalogueItemId, model);

            var route = routingService.GetRoute(
                RoutingSource.SelectServiceRecipients,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId));

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        private async Task AddServiceRecipients(int orderId, CatalogueItemId catalogueItemId, SelectRecipientsModel model)
        {
            var recipients = model.ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto);

            await orderItemRecipientService.AddOrderItemRecipients(orderId, catalogueItemId, recipients);
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
                })
                .ToList();
        }
    }
}
