using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services-requirement")]
    public class RequirementController : Controller
    {
        private readonly IContractsService contractsService;
        private readonly IRequirementsService requirementsService;
        private readonly IOrderService orderService;

        public RequirementController(
            IContractsService contractsService,
            IRequirementsService requirementsService,
            IOrderService orderService)
        {
            this.contractsService = contractsService ?? throw new ArgumentNullException(nameof(contractsService));
            this.requirementsService = requirementsService ?? throw new ArgumentNullException(nameof(requirementsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contract = await contractsService.GetContractWithContractBillingRequirements(order.Id);
            var model = new RequirementModel(contract?.ContractBilling)
            {
                BackLink = Url.Action(
                    nameof(ContractBillingController.Index),
                    typeof(ContractBillingController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId, RequirementModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = (await orderService.GetOrderThin(model.CallOffId, model.InternalOrgId)).Order;
            var contract = await contractsService.GetContract(order.Id);
            await requirementsService.SetRequirementComplete(order.Id, contract.Id);

            return RedirectToAction(nameof(Order), typeof(OrderController).ControllerName(), new { internalOrgId, callOffId });
        }

        [HttpGet("add-requirement")]
        public async Task<IActionResult> AddRequirement(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var associatedServices = order.GetAssociatedServices();

            var model = new RequirementDetailsModel(callOffId, internalOrgId, associatedServices)
            {
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(RequirementController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View("RequirementDetails", model);
        }

        [HttpPost("add-requirement")]
        public async Task<IActionResult> AddRequirement(string internalOrgId, CallOffId callOffId, RequirementDetailsModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (!ModelState.IsValid)
            {
                model.AssociatedServices = order.GetAssociatedServices();
                return View("RequirementDetails", model);
            }

            var contract = await contractsService.GetContract(order.Id);
            await requirementsService.AddRequirement(order.Id, contract.Id, model.SelectedOrderItemId, model.Details);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }

        [HttpGet("edit-requirement")]
        public async Task<IActionResult> EditRequirement(string internalOrgId, CallOffId callOffId, int itemId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var associatedServices = order.GetAssociatedServices();

            var requirement = await requirementsService.GetRequirement(order.Id, itemId);

            var model = new RequirementDetailsModel(requirement, callOffId, internalOrgId, associatedServices)
            {
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(RequirementController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };
            return View("RequirementDetails", model);
        }

        [HttpPost("edit-requirement")]
        public async Task<IActionResult> EditRequirement(string internalOrgId, CallOffId callOffId, RequirementDetailsModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (!ModelState.IsValid)
            {
                model.AssociatedServices = order.GetAssociatedServices();
                return View("RequirementDetails", model);
            }

            await requirementsService.EditRequirement(order.Id, model.ItemId, model.SelectedOrderItemId, model.Details);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }

        [HttpGet("delete-requirement")]
        public async Task<IActionResult> DeleteRequirement(string internalOrgId, CallOffId callOffId, int itemId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var requirement = await requirementsService.GetRequirement(order.Id, itemId);

            var model = new DeleteRequirementModel(callOffId, internalOrgId, requirement.Id)
            {
                BackLink = Url.Action(
                    nameof(EditRequirement),
                    typeof(RequirementController).ControllerName(),
                    new { internalOrgId, callOffId, itemId }),
            };
            return View(model);
        }

        [HttpPost("delete-requirement")]
        public async Task<IActionResult> DeleteRequirement(string internalOrgId, CallOffId callOffId, DeleteRequirementModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            await requirementsService.DeleteRequirement(order.Id, model.ItemId);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }
    }
}
