using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteCatalogueSolution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
    public sealed class DeleteCatalogueSolutionController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderItemService orderItemService;

        public DeleteCatalogueSolutionController(
            IOrderService orderService,
            IOrderItemService orderItemService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSolution(
            string odsCode,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string catalogueItemName)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteSolutionModel(odsCode, callOffId, catalogueItemId, catalogueItemName, order.Description));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSolution(
            string odsCode,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string catalogueItemName,
            DeleteSolutionModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, catalogueItemId);

            return RedirectToAction(
                nameof(Index),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
