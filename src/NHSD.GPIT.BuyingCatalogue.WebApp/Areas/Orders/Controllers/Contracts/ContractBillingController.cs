using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services-billing")]
    public class ContractBillingController : Controller
    {
        private readonly IContractsService contractsService;
        private readonly IContractBillingService contractBillingService;
        private readonly IOrderService orderService;

        public ContractBillingController(
            IContractsService contractsService,
            IContractBillingService contractBillingService,
            IOrderService orderService)
        {
            this.contractsService = contractsService ?? throw new ArgumentNullException(nameof(contractsService));
            this.contractBillingService = contractBillingService ?? throw new ArgumentNullException(nameof(contractBillingService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contract = await contractsService.GetContractWithContractBilling(order.Id);
            var model = new ContractBillingModel(contract?.ContractBilling)
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

        [HttpPost]
        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId, ContractBillingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = (await orderService.GetOrderThin(model.CallOffId, model.InternalOrgId)).Order;
            var contract = await contractsService.GetContract(order.Id);
            await contractBillingService.AddContractBilling(order.Id, contract.Id);

            return RedirectToAction(nameof(Order), typeof(OrderController).ControllerName(), new { internalOrgId, callOffId });
        }

        [HttpGet("add-milestone")]
        public async Task<IActionResult> AddMilestone(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var associatedServices = order.GetAssociatedServices();

            var model = new ContractBillingItemModel(callOffId, internalOrgId, associatedServices)
            {
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(ContractBillingController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View("ContractBillingItem", model);
        }

        [HttpPost("add-milestone")]
        public async Task<IActionResult> AddMilestone(string internalOrgId, CallOffId callOffId, ContractBillingItemModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (!ModelState.IsValid)
            {
                model.AssociatedServices = order.GetAssociatedServices();
                return View("ContractBillingItem", model);
            }

            var contract = await contractsService.GetContract(order.Id);
            await contractBillingService.AddBespokeContractBillingItem(order.Id, contract.Id, model.SelectedOrderItemId, model.Name, model.PaymentTrigger, model.Quantity);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }

        [HttpGet("edit-milestone")]
        public async Task<IActionResult> EditMilestone(string internalOrgId, CallOffId callOffId, int itemId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var associatedServices = order.GetAssociatedServices();

            var contractBillingItem = await contractBillingService.GetContractBillingItem(order.Id, itemId);

            var model = new ContractBillingItemModel(contractBillingItem, callOffId, internalOrgId, associatedServices)
            {
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(ContractBillingController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };
            return View("ContractBillingItem", model);
        }

        [HttpPost("edit-milestone")]
        public async Task<IActionResult> EditMilestone(string internalOrgId, CallOffId callOffId, ContractBillingItemModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (!ModelState.IsValid)
            {
                model.AssociatedServices = order.GetAssociatedServices();
                return View("ContractBillingItem", model);
            }

            await contractBillingService.EditContractBillingItem(order.Id, model.ItemId, model.SelectedOrderItemId, model.Name, model.PaymentTrigger, model.Quantity);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }

        [HttpGet("delete-milestone")]
        public async Task<IActionResult> DeleteContractBillingItem(string internalOrgId, CallOffId callOffId, int itemId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contractBillingItem = await contractBillingService.GetContractBillingItem(order.Id, itemId);

            var model = new DeleteContractBillingItemModel(callOffId, internalOrgId, contractBillingItem)
            {
                BackLink = Url.Action(
                    nameof(EditMilestone),
                    typeof(ContractBillingController).ControllerName(),
                    new { internalOrgId, callOffId, itemId }),
            };
            return View(model);
        }

        [HttpPost("delete-milestone")]
        public async Task<IActionResult> DeleteMilestone(string internalOrgId, CallOffId callOffId, DeleteContractBillingItemModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            await contractBillingService.DeleteContractBillingItem(order.Id, model.ItemId);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }
    }
}
