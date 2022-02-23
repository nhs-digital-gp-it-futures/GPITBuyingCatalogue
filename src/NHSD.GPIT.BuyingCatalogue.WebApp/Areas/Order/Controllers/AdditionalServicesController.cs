using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/additional-services")]
    public sealed class AdditionalServicesController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly ISolutionsService solutionsService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderSessionService orderSessionService;

        public AdditionalServicesController(
            IOrderService orderService,
            IAdditionalServicesService additionalServicesService,
            ISolutionsService solutionsService,
            IOrderItemService orderItemService,
            IOrderSessionService orderSessionService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, internalOrgId, CatalogueItemType.AdditionalService);

            var model = new AdditionalServiceModel(internalOrgId, order, orderItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpGet("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            // Get Catalogue Solutions related to the order
            var orderItems = await orderItemService.GetOrderItems(callOffId, internalOrgId, CatalogueItemType.Solution);
            var solutionIds = orderItems.Select(i => i.CatalogueItemId).ToList();

            var state = orderSessionService.InitialiseStateForCreate(order, CatalogueItemType.AdditionalService, solutionIds, null);

            // Get Additional Services that are related to any Catalogue Solution in the order
            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionIds(solutionIds);

            return !additionalServices.Any()
                ? View("NoAdditionalServicesFound", new NoAdditionalServicesFoundModel()
                {
                    BackLink = Url.Action(
                        nameof(OrderController.Order),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId }),
                })
                : View(new SelectAdditionalServiceModel(internalOrgId, callOffId, additionalServices, state.CatalogueItemId)
                {
                    BackLink = Url.Action(nameof(Index), new { internalOrgId, callOffId }),
                });
        }

        [HttpPost("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string internalOrgId, CallOffId callOffId, SelectAdditionalServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await additionalServicesService.GetAdditionalServicesBySolutionIds(state.SolutionIds);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, internalOrgId, model.SelectedAdditionalServiceId.GetValueOrDefault());

            if (existingOrder is not null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { internalOrgId, callOffId, existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedAdditionalServiceId;
            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());
            state.CatalogueItemName = solution.Name;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            if (!prices.Any())
                throw new InvalidOperationException($"Additional Service {state.CatalogueItemId.GetValueOrDefault()} does not have any Flat prices associated");

            if (prices.Count == 1)
            {
                state = orderSessionService.SetPrice(callOffId, prices.Single());

                state.SkipPriceSelection = true;
                orderSessionService.SetOrderStateToSession(state);

                return RedirectToAction(
                    nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients),
                    typeof(AdditionalServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(SelectAdditionalServicePrice),
                typeof(AdditionalServicesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            var model = new SelectAdditionalServicePriceModel(internalOrgId, callOffId, state.CatalogueItemName, prices)
            {
                BackLink = Url.Action(nameof(SelectAdditionalService), new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string internalOrgId, CallOffId callOffId, SelectAdditionalServicePriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(p => p.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(callOffId, cataloguePrice);

            return RedirectToAction(
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients),
                typeof(AdditionalServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatDeclarativeQuantityModel(callOffId, state.CatalogueItemName, state.Quantity)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                    typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string internalOrgId, CallOffId callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = model.QuantityAsInt;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { internalOrgId, callOffId, state.CatalogueItemId });
        }

        [HttpGet("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatOnDemandQuantityModel(internalOrgId, callOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                    typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string internalOrgId, CallOffId callOffId, SelectFlatOnDemandQuantityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = model.QuantityAsInt;
            state.EstimationPeriod = model.EstimationPeriod.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { internalOrgId, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditAdditionalService(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(internalOrgId, callOffId, catalogueItemId);

            var model = new EditAdditionalServiceModel(internalOrgId, state)
            {
                BackLink = GetEditAdditionalServiceBackLink(state, internalOrgId),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditAdditionalService(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, EditAdditionalServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);
            if (!ModelState.IsValid)
            {
                model.UpdateModel(state);
                return View(model);
            }

            state.AgreedPrice = model.OrderItem.AgreedPrice;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            await orderItemService.Create(callOffId, internalOrgId, state);

            orderSessionService.ClearSession(callOffId);

            return RedirectToAction(
                nameof(Index),
                typeof(AdditionalServicesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private string GetEditAdditionalServiceBackLink(CreateOrderItemModel state, string internalOrgId)
        {
            if (!state.IsNewSolution)
            {
                return Url.Action(nameof(Index), new { internalOrgId, callOffId = state.CallOffId });
            }

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.Declarative)
                return Url.Action(nameof(SelectFlatDeclarativeQuantity), new { internalOrgId, callOffId = state.CallOffId });
            else if (state.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
                return Url.Action(nameof(SelectFlatOnDemandQuantity), new { internalOrgId, callOffId = state.CallOffId });

            return Url.Action(
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                new { internalOrgId, callOffId = state.CallOffId });
        }
    }
}
