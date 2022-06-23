using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("ordersummary/{internalOrgId}/{callOffId}")]
    [RestrictToLocalhostActionFilter]
    public sealed class OrderSummaryController : Controller
    {
        private readonly IOrderService orderService;

        public OrderSummaryController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderForSummary(callOffId, internalOrgId);

            return View(new OrderSummaryModel(order));
        }
    }
}
