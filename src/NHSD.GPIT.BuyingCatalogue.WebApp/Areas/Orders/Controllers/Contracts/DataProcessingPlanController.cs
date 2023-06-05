using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DataProcessing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/data-processing-plan")]
    public class DataProcessingPlanController : Controller
    {
        private readonly IContractsService contractsService;
        private readonly IOrderService orderService;

        public DataProcessingPlanController(IContractsService contractsService, IOrderService orderService)
        {
            this.contractsService = contractsService ?? throw new ArgumentNullException(nameof(contractsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("default")]
        public async Task<IActionResult> Index(
            string internalOrgId,
            CallOffId callOffId)
        {
            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);
            var contract = await contractsService.GetContract(orderId);

            var model = new BespokeDataProcessingModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            await contractsService.UseDefaultDataProcessing(orderId, true);

            /*return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });*/
            return View(model);
        }

        [HttpPost("default")]
        public async Task<IActionResult> Index(
            string internalOrgId,
            CallOffId callOffId,
            BespokeDataProcessingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);

            await contractsService.UseDefaultDataProcessing(orderId, true);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("bespoke")]
        public IActionResult BespokeDataProcessingPlan(
            string internalOrgId,
            CallOffId callOffId)
        {
            var model = new BespokeDataProcessingModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, callOffId }),
            };

            return View(model);
        }
    }
}
