using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("order-summary/{internalOrgId}/{callOffId}")]
    [RestrictToLocalhostActionFilter]
    public sealed class OrderSummaryController : Controller
    {
        private readonly IImplementationPlanService implementationPlanService;
        private readonly IOrderService orderService;

        public OrderSummaryController(IImplementationPlanService implementationPlanService, IOrderService orderService)
        {
            this.implementationPlanService = implementationPlanService ?? throw new ArgumentNullException(nameof(implementationPlanService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();
            var orderWrapper = await orderService.GetOrderForSummary(callOffId, internalOrgId);

            return View(new OrderSummaryModel(orderWrapper, defaultPlan));
        }
    }
}
