using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.AssociatedServicesBilling;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services-billing")]
    public class AssociatedServicesBillingController : Controller
    {
        private readonly IContractsService contractsService;
        private readonly IAssociatedServicesBillingService associatedServicesBillingService;
        private readonly IImplementationPlanService implementationPlanService;
        private readonly IOrderService orderService;

        public AssociatedServicesBillingController(
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
        public async Task<IActionResult> ReviewBilling(string internalOrgId, CallOffId callOffId)
        {
            var associatedServiceItems = await associatedServicesBillingService.GetAssociatedServiceOrderItems(internalOrgId, callOffId);
            var implementationPlan = await implementationPlanService.GetDefaultImplementationPlan();
            var targetMilestone = implementationPlan.Milestones.OrderBy(ms => ms.Order).LastOrDefault();
            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);
            var contractFlags = await contractsService.GetContractFlags(orderId);

            var model = new ReviewBillingModel(callOffId, targetMilestone, contractFlags, associatedServiceItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewBilling(string internalOrgId, CallOffId callOffId, ReviewBillingModel model)
        {
            if (!ModelState.IsValid)
            {
                var associatedServiceItems = await associatedServicesBillingService.GetAssociatedServiceOrderItems(internalOrgId, callOffId);
                model.AssociatedServiceOrderItems = associatedServiceItems;
                return View(model);
            }

            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);

            await contractsService.UseDefaultBilling(orderId, model.UseDefaultBilling!.Value);

            if (model.UseDefaultBilling is false)
            {
                return RedirectToAction(
                    nameof(BespokeBilling),
                    typeof(AssociatedServicesBillingController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(SpecificRequirements),
                typeof(AssociatedServicesBillingController).ControllerName(),
                new { internalOrgId, callOffId, fromBespoke = false });
        }

        [HttpGet("bespoke-billing")]
        public IActionResult BespokeBilling(string internalOrgId, CallOffId callOffId)
        {
            var model = new BasicBillingModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(
                    nameof(ReviewBilling),
                    typeof(AssociatedServicesBillingController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpGet("requirements")]
        public async Task<IActionResult> SpecificRequirements(string internalOrgId, CallOffId callOffId, [FromQuery] bool? fromBespoke = false)
        {
            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);
            var contractFlags = await contractsService.GetContractFlags(orderId);

            var goBackLink = fromBespoke!.Value
                ? Url.Action(
                    nameof(BespokeBilling),
                    typeof(AssociatedServicesBillingController).ControllerName(),
                    new { internalOrgId, callOffId })
               : Url.Action(
                   nameof(ReviewBilling),
                   typeof(AssociatedServicesBillingController).ControllerName(),
                   new { internalOrgId, callOffId });

            var model = new SpecificRequirementsModel(callOffId, contractFlags)
            {
                BackLink = goBackLink,
            };

            return View(model);
        }

        [HttpPost("requirements")]
        public async Task<IActionResult> SpecificRequirements(string internalOrgId, CallOffId callOffId, SpecificRequirementsModel model, [FromQuery] bool? fromBespoke = false)
        {
            if (!ModelState.IsValid)
                return View(model);

            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);
            await contractsService.HasSpecificRequirements(orderId, !model.ProceedWithoutSpecificRequirements!.Value);

            if (model.ProceedWithoutSpecificRequirements is false)
            {
                return RedirectToAction(
                    nameof(BespokeRequirements),
                    typeof(AssociatedServicesBillingController).ControllerName(),
                    new { internalOrgId, callOffId, fromBespoke });
            }

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("bespoke-requirements")]
        public IActionResult BespokeRequirements(string internalOrgId, CallOffId callOffId, [FromQuery] bool? fromBespoke = false)
        {
            var model = new BasicBillingModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(
                    nameof(SpecificRequirements),
                    typeof(AssociatedServicesBillingController).ControllerName(),
                    new { internalOrgId, callOffId, fromBespoke }),
            };

            return View(model);
        }
    }
}
