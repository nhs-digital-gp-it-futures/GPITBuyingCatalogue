using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Review;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/review-solutions-and-services")]
    public class ReviewSolutionsController : Controller
    {
        private readonly IOrderService orderService;

        public ReviewSolutionsController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> ReviewSolutions(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new ReviewSolutionsModel(order, internalOrgId)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }
    }
}
