using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/funding-source")]
    public sealed class FundingSourceController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IFundingSourceService fundingSourceService;

        public FundingSourceController(
            IOrderService orderService,
            IFundingSourceService fundingSourceService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.fundingSourceService = fundingSourceService ?? throw new ArgumentNullException(nameof(fundingSourceService));
        }

        [HttpGet]
        public async Task<IActionResult> FundingSource(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, odsCode);

            var model = new FundingSourceModel(odsCode, callOffId, order.FundingSourceOnlyGms)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FundingSource(string odsCode, CallOffId callOffId, FundingSourceModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var onlyGms = model.FundingSourceOnlyGms.EqualsIgnoreCase("Yes");

            await fundingSourceService.SetFundingSource(callOffId, odsCode, onlyGms);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
