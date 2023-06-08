using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
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

        [HttpGet]
        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contract = await contractsService.GetContract(order.Id);
            var catalogueItem = await solutionsService.GetSolutionThin(order.GetSolutionId().GetValueOrDefault());
            var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();

            var model = new ImplementationPlanModel(defaultPlan, contract?.ImplementationPlan, catalogueItem?.Solution)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
            };

            return View(model);
        }

        [HttpGet("add-milestone")]
        public IActionResult AddMilestone(string internalOrgId, CallOffId callOffId)
        {
            var model = new MilestoneModel()
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
            };

            return View("Milestone", model);
        }

        [HttpPost("add-milestone")]
        public async Task<IActionResult> AddMilestone(string internalOrgId, CallOffId callOffId, MilestoneModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Milestone", model);
            }

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contract = await contractsService.GetContract(order.Id);

            await implementationPlanService.AddImplementationPlan(contract.Id, model.Name, model.PaymentTrigger);

            return View("Milestone", model);
        }

        [HttpGet("edit-milestone")]
        public IActionResult EditMilestone(string internalOrgId, CallOffId callOffId, int milestoneId)
        {
            return View("Milestone", new MilestoneModel());
        }
    }
}
