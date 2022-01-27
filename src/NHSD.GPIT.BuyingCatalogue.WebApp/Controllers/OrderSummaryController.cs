using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("ordersummary/{odsCode}/{callOffId}")]
    [RestrictToLocalhostActionFilter]
    public sealed class OrderSummaryController : Controller
    {
        private readonly IOrderService orderService;

        public OrderSummaryController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderForSummary(callOffId, odsCode);

            var model = new OrderSummaryModel(odsCode, order)
            {
                Title = order.OrderStatus switch
                {
                    OrderStatus.Complete => $"Order confirmed for {callOffId}",
                    _ => order.CanComplete()
                        ? $"Review order summary for {callOffId}"
                        : $"Order summary for {callOffId}",
                },

                AdviceText = order.OrderStatus switch
                {
                    OrderStatus.Complete => "This order has been confirmed and can no longer be changed.",
                    _ => order.CanComplete()
                        ? "Review your order summary and confirm the content is correct. Once confirmed, you'll be unable to make changes."
                        : "This is what's been added to your order so far. You must complete all mandatory steps before you can confirm your order.",
                },
            };

            return View(model);
        }
    }
}
