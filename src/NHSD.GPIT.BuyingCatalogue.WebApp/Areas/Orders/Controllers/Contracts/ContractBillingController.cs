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
        private readonly IAssociatedServicesBillingService associatedServicesBillingService;
        private readonly IImplementationPlanService implementationPlanService;
        private readonly IOrderService orderService;

        public ContractBillingController(
            IContractsService contractsService,
            IAssociatedServicesBillingService associatedServicesBillingService,
            IImplementationPlanService implementationPlanService,
            IOrderService orderService)
        {
            this.contractsService = contractsService ?? throw new ArgumentNullException(nameof(contractsService));
            this.associatedServicesBillingService = associatedServicesBillingService ?? throw new ArgumentNullException(nameof(associatedServicesBillingService));
            this.implementationPlanService = implementationPlanService ?? throw new ArgumentNullException(nameof(implementationPlanService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contract = await contractsService.GetContract(order.Id);
            //TODO Remove
            contract.ContractBilling = new ContractBilling()
            {
                ContractBillingItems = new List<ContractBillingItem>()
                {
                    new ContractBillingItem()
                    {
                        Milestone =
                            new ImplementationPlanMilestone()
                            {
                                Title = "Test", PaymentTrigger = "Trigger",
                            },
                        Quantity = 10,
                        OrderItem = new OrderItem() { CatalogueItem = new CatalogueItem() { Id = new CatalogueItemId(4, "CO"), Name = "Test Associated Service", },},
                    },
                },
            };

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
            //TODO await contractsService.AddContract(order.Id);

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

            //TODO await contractBillingService.AddBespokeMilestone(order.Id, contract.Id, model.Name, model.PaymentTrigger);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }

        [HttpGet("edit-milestone")]
        public async Task<IActionResult> EditMilestone(string internalOrgId, CallOffId callOffId, int milestoneId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var associatedServices = order.GetAssociatedServices();

            var contractBillingItem = new ContractBillingItem() { Milestone = new ImplementationPlanMilestone() { Title = "Test", PaymentTrigger = "Trigger", }, Quantity = 10, OrderItem = new OrderItem() { CatalogueItem = new CatalogueItem() { Id = new CatalogueItemId(10000, "S-036"), Name = "Test Associated Service", }, } }; 
            //TODO await contractBillingService.GetContractBillingItem(order.Id, milestoneId);

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

            // TODO await contractBillingService.EditMilestone(order.Id, model.MilestoneId, model.Name, model.PaymentTrigger);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }

        [HttpGet("delete-milestone")]
        public async Task<IActionResult> DeleteContractBillingItem(string internalOrgId, CallOffId callOffId, int milestoneId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var contractBillingItem = new ContractBillingItem() { Milestone = new ImplementationPlanMilestone() { Title = "Test", PaymentTrigger = "Trigger", }, Quantity = 10, CatalogueItemId = new CatalogueItemId(4, "CO") };
            //TODO await contractBillingService.GetContractBillingItem(order.Id, milestoneId);

            var model = new DeleteContractBillingItemModel(callOffId, internalOrgId, contractBillingItem)
            {
                BackLink = Url.Action(
                    nameof(EditMilestone),
                    typeof(ContractBillingController).ControllerName(),
                    new { internalOrgId, callOffId, milestoneId }),
            };
            return View(model);
        }

        [HttpPost("delete-milestone")]
        public async Task<IActionResult> DeleteMilestone(string internalOrgId, CallOffId callOffId, DeleteMilestoneModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            await implementationPlanService.DeleteMilestone(order.Id, model.MilestoneId);

            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });
        }
    }
}
