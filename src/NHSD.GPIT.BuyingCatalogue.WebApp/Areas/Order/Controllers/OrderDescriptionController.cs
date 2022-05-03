using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/description")]
    public sealed class OrderDescriptionController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IFundingSourceService fundingSourceService;
        private readonly IOrderDescriptionService orderDescriptionService;
        private readonly IOrganisationsService organisationsService;
        private readonly IUsersService usersService;

        public OrderDescriptionController(
            IOrderService orderService,
            IOrderDescriptionService orderDescriptionService,
            IOrganisationsService organisationsService,
            IUsersService usersService,
            IFundingSourceService fundingSourceService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderDescriptionService = orderDescriptionService ?? throw new ArgumentNullException(nameof(orderDescriptionService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this.fundingSourceService = fundingSourceService ?? throw new ArgumentNullException(nameof(fundingSourceService));
        }

        [HttpGet]
        public async Task<IActionResult> OrderDescription(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var descriptionModel = new OrderDescriptionModel(internalOrgId, order)
            {
                BackLink = Url.Action(
                            nameof(OrderController.Order),
                            typeof(OrderController).ControllerName(),
                            new { internalOrgId, callOffId }),
            };

            return View(descriptionModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrderDescription(string internalOrgId, CallOffId callOffId, OrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await orderDescriptionService.SetOrderDescription(callOffId, internalOrgId, model.Description);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("~/organisation/{internalOrgId}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string internalOrgId, TriageOption? option = null, FundingSource? fundingSource = null)
        {
            var user = await usersService.GetUser(User.UserId());
            var organisation = await organisationsService.GetOrganisation(user?.PrimaryOrganisationId ?? 0);

            var descriptionModel = new OrderDescriptionModel(internalOrgId, organisation?.Name)
            {
                BackLink = Url.Action(
                    nameof(OrderController.NewOrder),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, option, fundingSource }),
            };

            return View("OrderDescription", descriptionModel);
        }

        [HttpPost("~/organisation/{internalOrgId}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string internalOrgId, OrderDescriptionModel model, FundingSource? fundingSource = null)
        {
            if (!ModelState.IsValid)
                return View("OrderDescription", model);

            var order = await orderService.CreateOrder(model.Description, model.InternalOrgId);
            if (fundingSource != null)
                await fundingSourceService.SetFundingSource(order.CallOffId, internalOrgId, fundingSource!.Value.IsCentralFunding(), false);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, order.CallOffId });
        }
    }
}
