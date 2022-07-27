using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DataProcessing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/data-processing-plan")]
    public class DataProcessingPlanController : Controller
    {
        private readonly IDataProcessingPlanService dataProcessingPlanService;
        private readonly IContractsService contractsService;

        public DataProcessingPlanController(
            IDataProcessingPlanService dataProcessingPlanService,
            IContractsService contractsService)
        {
            this.dataProcessingPlanService = dataProcessingPlanService
                ?? throw new ArgumentNullException(nameof(dataProcessingPlanService));
            this.contractsService = contractsService
                ?? throw new ArgumentNullException(nameof(contractsService));
        }

        [HttpGet("default")]
        public async Task<IActionResult> Index(
            string internalOrgId,
            CallOffId callOffId)
        {
            var contract = await contractsService.GetContract(callOffId.Id);

            var model = new DataProcessingPlanModel(contract.DataProcessingPlan)
            {
                CallOffId = callOffId,
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("default")]
        public async Task<IActionResult> Index(
            string internalOrgId,
            CallOffId callOffId,
            DataProcessingPlanModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var defaultPlan = await dataProcessingPlanService.GetDefaultDataProcessingPlan();
            if (model.UseDefaultDataProcessing!.Value)
            {
                await contractsService.SetDataProcessingPlanId(callOffId.Id, defaultPlan.Id);

                return RedirectToAction(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var contract = await contractsService.GetContract(callOffId.Id);
            if (contract.DataProcessingPlanId == null
                || contract.DataProcessingPlanId == defaultPlan?.Id)
            {
                var dataProcessingPlan = await dataProcessingPlanService.CreateDataProcessingPlan();

                await contractsService.SetDataProcessingPlanId(callOffId.Id, dataProcessingPlan.Id);
            }

            return RedirectToAction(
                nameof(BespokeDataProcessingPlan),
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
