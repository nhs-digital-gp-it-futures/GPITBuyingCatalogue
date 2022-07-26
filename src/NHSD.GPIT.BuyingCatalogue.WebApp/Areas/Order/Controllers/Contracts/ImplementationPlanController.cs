﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.ImplementationPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/implementation-plan")]
    public class ImplementationPlanController : Controller
    {
        private readonly IContractsService contractsService;
        private readonly IImplementationPlanService implementationPlanService;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;

        public ImplementationPlanController(
            IContractsService contractsService,
            IImplementationPlanService implementationPlanService,
            IOrderService orderService,
            ISolutionsService solutionsService)
        {
            this.contractsService = contractsService ?? throw new ArgumentNullException(nameof(contractsService));
            this.implementationPlanService = implementationPlanService ?? throw new ArgumentNullException(nameof(implementationPlanService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("default")]
        public async Task<IActionResult> DefaultImplementationPlan(string internalOrgId, CallOffId callOffId)
        {
            return View(
                "~/Areas/Order/Views/Contracts/DefaultImplementationPlan.cshtml",
                await GetDefaultViewModel(internalOrgId, callOffId));
        }

        [HttpPost("default")]
        public async Task<IActionResult> DefaultImplementationPlan(string internalOrgId, CallOffId callOffId, DefaultImplementationPlanModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Areas/Order/Views/Contracts/DefaultImplementationPlan.cshtml",
                    await GetDefaultViewModel(internalOrgId, callOffId));
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();

            if (model.UseDefaultMilestones!.Value)
            {
                await contractsService.SetImplementationPlanId(order.Id, defaultPlan?.Id);

                return new RedirectToActionResult(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var contract = await contractsService.GetContract(order.Id);

            if (contract.ImplementationPlanId == null
                || contract.ImplementationPlanId == defaultPlan?.Id)
            {
                var plan = await implementationPlanService.CreateImplementationPlan();

                await contractsService.SetImplementationPlanId(order.Id, plan.Id);
            }

            return new RedirectToActionResult(
                nameof(CustomImplementationPlan),
                typeof(ImplementationPlanController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("custom")]
        public IActionResult CustomImplementationPlan(string internalOrgId, CallOffId callOffId)
        {
            var model = new CustomImplementationPlanModel
            {
                BackLink = Url.Action(
                    nameof(DefaultImplementationPlan),
                    typeof(ImplementationPlanController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
            };

            return View("~/Areas/Order/Views/Contracts/CustomImplementationPlan.cshtml", model);
        }

        private async Task<DefaultImplementationPlanModel> GetDefaultViewModel(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var contract = await contractsService.GetContract(order.Id);
            var catalogueItem = await solutionsService.GetSolutionThin(order.GetSolutionId().GetValueOrDefault());
            var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();

            var selected = contract?.ImplementationPlan == null
                ? (bool?)null
                : contract.ImplementationPlan.Id == defaultPlan.Id;

            return new DefaultImplementationPlanModel
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CallOffId = callOffId,
                Solution = catalogueItem?.Solution,
                Plan = defaultPlan,
                UseDefaultMilestones = selected,
            };
        }
    }
}
