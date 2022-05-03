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
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/funding-source")]
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
        public async Task<IActionResult> FundingSource(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var model = new ConfirmFundingSourceModel(callOffId, order.FundingSourceOnlyGms)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FundingSource(string internalOrgId, CallOffId callOffId, ConfirmFundingSourceModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var onlyGms = model.SelectedFundingSource!.Value.IsCentralFunding();

            await fundingSourceService.SetFundingSource(callOffId, internalOrgId, onlyGms, true);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
