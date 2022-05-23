using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/task-list")]
    public class TaskListController : Controller
    {
        private readonly IOrderService orderService;

        public TaskListController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> TaskList(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);

            return View(new TaskListModel(internalOrgId, callOffId, order)
            {
                BackLink = Url.Action(
                    nameof(ReviewSolutionsController.ReviewSolutions),
                    typeof(ReviewSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId }),
            });
        }
    }
}
