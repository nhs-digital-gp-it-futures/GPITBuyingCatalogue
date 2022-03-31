using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/additional-services")]
    public class AdditionalServicesController : Controller
    {
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;

        public AdditionalServicesController(
            IAdditionalServicesService additionalServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService)
        {
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectAdditionalServices(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var solution = order.GetSolution();

            if (solution == null)
            {
                return RedirectToAction(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(solution.CatalogueItemId);

            return View(new SelectServicesModel(order, additionalServices, CatalogueItemType.AdditionalService)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            });
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectAdditionalServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            var serviceIds = model.Services?
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList();

            if (serviceIds?.Any() ?? false)
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, serviceIds);

                var catalogueItemId = serviceIds.First();

                return RedirectToAction(
                    nameof(ServiceRecipientsController.AdditionalServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId });
            }

            // TODO: Replace
            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
