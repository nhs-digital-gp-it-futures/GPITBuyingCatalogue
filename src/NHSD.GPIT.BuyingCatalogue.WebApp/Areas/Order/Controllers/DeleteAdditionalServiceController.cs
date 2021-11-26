using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAdditionalService;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/additional-services/delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
    public sealed class DeleteAdditionalServiceController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderItemService orderItemService;

        public DeleteAdditionalServiceController(
            IOrderService orderService,
            IOrderItemService orderItemService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, string catalogueItemName)
        {
            var order = await orderService.GetOrder(callOffId);

            var model = new DeleteAdditionalServiceModel(odsCode, callOffId, catalogueItemId, catalogueItemName, order.Description)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServicesController.EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId, catalogueItemId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, string catalogueItemName, DeleteAdditionalServiceModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, catalogueItemId);

            return RedirectToAction(
                nameof(AdditionalServicesController.Index),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
