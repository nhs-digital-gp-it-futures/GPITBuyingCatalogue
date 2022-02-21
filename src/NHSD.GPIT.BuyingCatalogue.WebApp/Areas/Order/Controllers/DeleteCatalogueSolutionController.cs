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
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/catalogue-solutions/delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
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
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string catalogueItemName)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var model = new DeleteSolutionModel(internalOrgId, callOffId, catalogueItemId, catalogueItemName, order.Description)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSolution(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string catalogueItemName,
            DeleteSolutionModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, internalOrgId, catalogueItemId);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.Index),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
