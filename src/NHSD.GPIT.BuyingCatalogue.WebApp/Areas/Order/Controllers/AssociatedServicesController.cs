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
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.AssociatedService);

            return View(new AssociatedServiceModel(odsCode, order, orderItems));
        }

        [HttpGet("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var organisation = await organisationService.GetOrganisationByOdsCode(odsCode);

            var state = orderSessionService.InitialiseStateForCreate(callOffId, order.CommencementDate, order.SupplierId, CatalogueItemType.AssociatedService, null, new OrderItemRecipientModel { OdsCode = odsCode, Name = organisation.Name });

            var associatedServices = await associatedServicesService.GetAssociatedServicesForSupplier(order.SupplierId);

            if (!associatedServices.Any())
                return View("NoAssociatedServicesFound", new NoAssociatedServicesFoundModel(odsCode, callOffId));

            return View(new SelectAssociatedServiceModel(odsCode, callOffId, associatedServices, state.CatalogueItemId));
        }

        [HttpPost("select/associated-service")]
        public async Task<IActionResult> SelectAssociatedService(string odsCode, CallOffId callOffId, SelectAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await associatedServicesService.GetAssociatedServicesForSupplier(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId.Value);

            if (existingOrder != null)
            {
                return RedirectToAction(
                    nameof(EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { odsCode, callOffId, id = existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolution(state.CatalogueItemId.Value);
            state.CatalogueItemName = solution.Name;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            if (prices.Count == 1)
            {
                orderSessionService.SetPrice(callOffId, prices.Single());

                state.SkipAssociatedServicePrices = true;
                orderSessionService.SetOrderStateToSession(state);

                return RedirectToAction(
                    nameof(EditAssociatedService),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { odsCode, callOffId, state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectAssociatedServicePrice),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolution(state.CatalogueItemId.Value);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            return View(new SelectAssociatedServicePriceModel(odsCode, callOffId, state.CatalogueItemName, prices));
        }

        [HttpPost("select/associated-service/price")]
        public async Task<IActionResult> SelectAssociatedServicePrice(string odsCode, CallOffId callOffId, SelectAssociatedServicePriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolution(state.CatalogueItemId.Value);

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(p => p.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(callOffId, cataloguePrice);

            return RedirectToAction(
                nameof(EditAssociatedService),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditAssociatedService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, catalogueItemId);

            return View(new EditAssociatedServiceModel(odsCode, callOffId, state));
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditAssociatedService(string odsCode, CallOffId callOffId, string catalogueItemId, EditAssociatedServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!model.OrderItem.ServiceRecipients[0].Quantity.HasValue
                || model.OrderItem.ServiceRecipients[0].Quantity.Value == 0)
                ModelState.AddModelError($"OrderItem.ServiceRecipients[0].Quantity", "Quantity is Required");

            if (model.OrderItem.AgreedPrice is null)
                ModelState.AddModelError("OrderItem.Price", "Price is Required");

            if (model.OrderItem.AgreedPrice > state.CataloguePrice)
                ModelState.AddModelError("OrderItem.Price", "Price cannot be greater than list price");

            if (state.ProvisioningType == ProvisioningType.OnDemand)
                state.TimeUnit = model.TimeUnit;

            if (!ModelState.IsValid)
            {
                model.OrderItem.CataloguePrice = state.CataloguePrice;
                model.OrderItem.TimeUnit = state.TimeUnit;
                return View(model);
            }

            state.AgreedPrice = model.OrderItem.AgreedPrice;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            // TODO - handle errors
            var result = await orderItemService.Create(callOffId, state);

            orderSessionService.ClearSession(callOffId);

            return RedirectToAction(
                nameof(Index),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
        public async Task<IActionResult> DeleteAssociatedService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, string catalogueItemName)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteAssociatedServiceModel(odsCode, callOffId, catalogueItemId, catalogueItemName, order.Description));
        }

        [HttpPost("delete/{catalogueItemId}/confirmation/{catalogueItemName}")]
        public async Task<IActionResult> DeleteAssociatedService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, string catalogueItemName, DeleteAssociatedServiceModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, catalogueItemId);

            return RedirectToAction(
                nameof(Index),
                typeof(AssociatedServicesController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
