using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAssociatedService;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/associated-services/delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
    public sealed class DeleteAssociatedServiceController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderItemService orderItemService;

        public DeleteAssociatedServiceController(
            IOrderService orderService,
            IOrderItemService orderItemService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssociatedService(
            string odsCode,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string catalogueItemName)
        {
            var order = await orderService.GetOrder(callOffId);

            var model = new DeleteAssociatedServiceModel(odsCode, callOffId, catalogueItemId, catalogueItemName, order.Description)
            {
                BackLink = Url.Action(
                    nameof(AssociatedServicesController.EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { odsCode, callOffId, catalogueItemId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAssociatedService(
            string odsCode,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string catalogueItemName,
            DeleteAssociatedServiceModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, catalogueItemId);

            return RedirectToAction(
                nameof(AssociatedServicesController.Index),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
