using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/associated-services")]
    public sealed class AssociatedServicesController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly ISessionService sessionService;
        private readonly ISolutionsService solutionsService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderSessionService orderSessionService;
        private readonly IOrganisationsService organisationService;

        public AssociatedServicesController(
            IOrderService orderService,
            IAssociatedServicesService associatedServicesService,
            ISessionService sessionService,
            ISolutionsService solutionsService,
            IOrderItemService orderItemService,
            IOrderSessionService orderSessionService,
            IOrganisationsService organisationService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            sessionService.ClearSession();

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.AssociatedService);

            return View(new AssociatedServiceModel(odsCode, order, orderItems));
        }

        [HttpGet("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var organisation = await organisationService.GetOrganisationByOdsCode(odsCode);

            var state = new CreateOrderItemModel
            {
                IsNewOrder = true,
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = CatalogueItemType.AssociatedService,
                ServiceRecipients = new List<OrderItemRecipientModel> { new OrderItemRecipientModel { OdsCode = odsCode, Name = organisation.Name } },
            };

            orderSessionService.SetOrderStateToSession(state);

            var associatedServices = await associatedServicesService.GetAssociatedServicesForSupplier(order.SupplierId);

            if (!associatedServices.Any())
                return View("NoAssociatedServicesFound", new NoAssociatedServicesFoundModel(odsCode, callOffId));

            return View(new SelectAssociatedServiceModel(odsCode, callOffId, associatedServices, state.CatalogueItemId));
        }

        [HttpPost("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string odsCode, CallOffId callOffId, SelectAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            if (!ModelState.IsValid)
            {
                model.Solutions = await associatedServicesService.GetAssociatedServicesForSupplier(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId.Value);

            if (existingOrder != null)
            {
                return RedirectToAction(
                    actionName: nameof(EditAssociatedService),
                    controllerName: typeof(AssociatedServicesController).ControllerName(),
                    routeValues: new { odsCode, callOffId, id = existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolution(state.CatalogueItemId.Value);
            state.CatalogueItemName = solution.Name;
            state.CatalogueSolutionId = model.SelectedSolutionId;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(x => x.CataloguePriceType == CataloguePriceType.Flat).ToList();

            if (prices.Count == 1)
            {
                orderSessionService.SetPrice(prices.Single());

                return RedirectToAction(
                    actionName: nameof(EditAssociatedService),
                    controllerName: typeof(AssociatedServicesController).ControllerName(),
                    routeValues: new { odsCode, callOffId, id = state.CatalogueItemId });
            }

            return RedirectToAction(
                actionName: nameof(SelectAssociatedServicePrice),
                controllerName: typeof(AssociatedServicesController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }

        [HttpGet("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId.Value);

            var prices = solution.CataloguePrices.Where(x => x.CataloguePriceType == CataloguePriceType.Flat).ToList();

            return View(new SelectAssociatedServicePriceModel(odsCode, callOffId, state.CatalogueItemName, prices));
        }

        [HttpPost("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string odsCode, CallOffId callOffId, SelectAssociatedServicePriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId.Value);

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(x => x.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(x => x.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(cataloguePrice);

            return RedirectToAction(
                actionName: nameof(EditAssociatedService),
                controllerName: typeof(AssociatedServicesController).ControllerName(),
                routeValues: new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> EditAssociatedService(string odsCode, CallOffId callOffId, CatalogueItemId id)
        {
            var isNewSolution = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, id);

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new EditAssociatedServiceModel(odsCode, callOffId, id, state, isNewSolution));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> EditAssociatedService(string odsCode, CallOffId callOffId, string id, EditAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            if (!model.OrderItem.ServiceRecipients[0].Quantity.HasValue
                || model.OrderItem.ServiceRecipients[0].Quantity.Value == 0)
                ModelState.AddModelError($"OrderItem.ServiceRecipients[0].Quantity", "Quantity is Required");

            var solutionListPrices = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.Value);

            var solutionPrice = solutionListPrices.CataloguePrices.Where(cp =>
                cp.ProvisioningType == state.ProvisioningType
                && cp.CataloguePriceType == state.Type
                && (cp.TimeUnit is null || cp.TimeUnit == state.TimeUnit)).FirstOrDefault();

            if (solutionPrice is not null)
            {
                if (model.OrderItem.Price is null)
                    ModelState.AddModelError("OrderItem.Price", "Price is Required");

                if (model.OrderItem.Price > solutionPrice.Price)
                    ModelState.AddModelError("OrderItem.Price", "Price cannot be greater than list price");
            }

            if (state.ProvisioningType == ProvisioningType.OnDemand)
                state.TimeUnit = model.TimeUnit;

            if (!ModelState.IsValid)
            {
                model.OrderItem.ProvisioningType = state.ProvisioningType;
                model.OrderItem.ItemUnit = state.ItemUnit;
                model.OrderItem.TimeUnit = state.TimeUnit;
                return View(model);
            }

            state.Price = model.OrderItem.Price;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            // TODO - handle errors
            var result = await orderItemService.Create(callOffId, state);

            sessionService.ClearSession();

            return RedirectToAction(
                actionName: nameof(Index),
                controllerName: typeof(AssociatedServicesController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }

        [HttpGet("delete/{id}/confirmation/{serviceName}")]
        public async Task<IActionResult> DeleteAssociatedService(string odsCode, CallOffId callOffId, string id, string serviceName)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteAssociatedServiceModel(odsCode, callOffId, id, serviceName, order.Description));
        }

        [HttpPost("delete/{id}/confirmation/{serviceName}")]
        public async Task<IActionResult> DeleteAssociatedService(string odsCode, CallOffId callOffId, CatalogueItemId id, string serviceName, DeleteAssociatedServiceModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, id);

            return RedirectToAction(
                actionName: nameof(Index),
                controllerName: typeof(AssociatedServicesController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }
    }
}
