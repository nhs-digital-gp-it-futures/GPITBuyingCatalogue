using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Review;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public class ReviewContractController : Controller
    {
        public const string ErrorKey = "Order";
        public const string ErrorMessage = "Your order is incomplete. Please go back to the order and check again";

        private readonly IImplementationPlanService implementationPlanService;
        private readonly IOrderService orderService;
        private readonly PdfSettings pdfSettings;

        public ReviewContractController(
            IImplementationPlanService implementationPlanService,
            IOrderService orderService,
            PdfSettings pdfSettings)
        {
            this.implementationPlanService = implementationPlanService ?? throw new ArgumentNullException(nameof(implementationPlanService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.pdfSettings = pdfSettings ?? throw new ArgumentNullException(nameof(pdfSettings));
        }

        [HttpGet("summary/contract")]

        public async Task<IActionResult> ContractSummary(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderForSummary(callOffId, internalOrgId)).Order;
            var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();
            var model = new ReviewContractModel(internalOrgId, order, defaultPlan)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Summary),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("summary/contract")]
        public async Task<IActionResult> ContractSummary(string internalOrgId, CallOffId callOffId, ReviewContractModel model)
        {
            var order = (await orderService.GetOrderForSummary(callOffId, internalOrgId)).Order;

            if (!order.CanComplete())
            {
                ModelState.AddModelError(ErrorKey, ErrorMessage);

                var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();

                return View(new ReviewContractModel(internalOrgId, order, defaultPlan)
                {
                    BackLink = Url.Action(
                        nameof(OrderController.Summary),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId }),
                });
            }

            await orderService.CompleteOrder(
                callOffId,
                internalOrgId,
                User.UserId());

            return RedirectToAction(
                nameof(OrderController.Completed),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
