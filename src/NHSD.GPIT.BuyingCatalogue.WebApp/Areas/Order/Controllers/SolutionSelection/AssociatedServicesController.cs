using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services")]
    public class AssociatedServicesController : Controller
    {
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;

        public AssociatedServicesController(
            IAssociatedServicesService associatedServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService)
        {
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("add")]
        public IActionResult AddAssociatedServices(string internalOrgId, CallOffId callOffId)
        {
            return View(new AddAssociatedServicesModel
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            });
        }

        [HttpPost("add")]
        public IActionResult AddAssociatedServices(string internalOrgId, CallOffId callOffId, AddAssociatedServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var addAssociatedServices = model.AdditionalServicesRequired.EqualsIgnoreCase(YesNoRadioButtonTagHelper.Yes);

            if (addAssociatedServices)
            {
                return RedirectToAction(
                    nameof(SelectAssociatedServices),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(ReviewSolutionsController.ReviewSolutions),
                typeof(ReviewSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectAssociatedServices(string internalOrgId, CallOffId callOffId)
        {
            return View(await GetModel(internalOrgId, callOffId));
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectAssociatedServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await GetModel(internalOrgId, callOffId));
            }

            var serviceIds = model.Services?
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList();

            if (serviceIds?.Any() ?? false)
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, serviceIds);

                var catalogueItemId = serviceIds.First();

                return RedirectToAction(
                    nameof(ServiceRecipientsController.AddServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId });
            }

            return RedirectToAction(
                nameof(ReviewSolutionsController.ReviewSolutions),
                typeof(ReviewSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private async Task<SelectServicesModel> GetModel(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSupplier(order.SupplierId);

            return new SelectServicesModel(order, associatedServices, CatalogueItemType.AssociatedService)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
            };
        }
    }
}
