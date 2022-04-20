using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/service-recipients")]
    public class ServiceRecipientsController : Controller
    {
        private readonly IOdsService odsService;
        private readonly IOrderItemRecipientService orderItemRecipientService;
        private readonly IOrderService orderService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderItemRecipientService orderItemRecipientService,
            IOrderService orderService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemRecipientService = orderItemRecipientService ?? throw new ArgumentNullException(nameof(orderItemRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("solution")]
        public async Task<IActionResult> SolutionRecipients(string internalOrgId, CallOffId callOffId, SelectionMode? selectionMode)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solution = order.GetSolution();
            var serviceRecipients = await GetServiceRecipients(internalOrgId);

            var model = new SelectRecipientsModel(serviceRecipients, selectionMode)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                ItemName = solution.CatalogueItem.Name,
                ItemType = CatalogueItemType.Solution,
            };

            return View("SelectRecipients", model);
        }

        [HttpPost("solution")]
        public async Task<IActionResult> SolutionRecipients(string internalOrgId, CallOffId callOffId, SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solution = order.GetSolution();

            await AddServiceRecipients(order.Id, solution.CatalogueItem.Id, model);

            // TODO: Replace
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("additional-service/{catalogueItemId}")]
        public async Task<IActionResult> AdditionalServiceRecipients(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectionMode? selectionMode)
        {
            var order = await orderService.GetOrderSummary(callOffId, internalOrgId);
            var solution = order.GetSolution();
            var service = order.GetAdditionalService(catalogueItemId);
            var serviceRecipients = await GetServiceRecipients(internalOrgId);

            var model = new SelectRecipientsModel(serviceRecipients, selectionMode)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = service.CatalogueItem.Id,
                ItemName = service.CatalogueItem.Name,
                ItemType = CatalogueItemType.AdditionalService,
            };

            model.PreSelectRecipients(solution);

            return View("SelectRecipients", model);
        }

        [HttpPost("additional-service/{catalogueItemId}")]
        public async Task<IActionResult> AdditionalServiceRecipients(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            await AddServiceRecipients(order.Id, catalogueItemId, model);

            // TODO: Replace
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("associated-service/{catalogueItemId}")]
        public async Task<IActionResult> AssociatedServiceRecipients(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectionMode? selectionMode)
        {
            var order = await orderService.GetOrderSummary(callOffId, internalOrgId);
            var solution = order.GetSolution();
            var service = order.GetAssociatedService(catalogueItemId);
            var serviceRecipients = await GetServiceRecipients(internalOrgId);

            var model = new SelectRecipientsModel(serviceRecipients, selectionMode)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = service.CatalogueItem.Id,
                ItemName = service.CatalogueItem.Name,
                ItemType = CatalogueItemType.AssociatedService,
            };

            model.PreSelectRecipients(solution);

            return View("SelectRecipients", model);
        }

        [HttpPost("associated-service/{catalogueItemId}")]
        public async Task<IActionResult> AssociatedServiceRecipients(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            await AddServiceRecipients(order.Id, catalogueItemId, model);

            // TODO: Replace
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
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
