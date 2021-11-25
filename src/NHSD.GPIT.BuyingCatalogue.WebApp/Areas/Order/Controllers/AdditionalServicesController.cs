using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    [Route("order/organisation/{odsCode}/order/{callOffId}/additional-services")]
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

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.AdditionalService);

            var model = new AdditionalServiceModel(odsCode, order, orderItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpGet("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            // Get Catalogue Solutions related to the order
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);
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
                        new { odsCode, callOffId }),
                })
                : View(new SelectAdditionalServiceModel(odsCode, callOffId, additionalServices, state.CatalogueItemId)
                {
                    BackLink = Url.Action(nameof(Index), new { odsCode, callOffId }),
                });
        }

        [HttpPost("select/additional-service")]
        public async Task<IActionResult> SelectAdditionalService(string odsCode, CallOffId callOffId, SelectAdditionalServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await additionalServicesService.GetAdditionalServicesBySolutionIds(state.SolutionIds);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedAdditionalServiceId.GetValueOrDefault());

            if (existingOrder is not null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditAdditionalService),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { odsCode, callOffId, existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedAdditionalServiceId;
            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());
            state.CatalogueItemName = solution.Name;

            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

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
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(SelectAdditionalServicePrice),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            var model = new SelectAdditionalServicePriceModel(odsCode, callOffId, state.CatalogueItemName, prices)
            {
                BackLink = Url.Action(nameof(SelectAdditionalService), new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/additional-service/price")]
        public async Task<IActionResult> SelectAdditionalServicePrice(string odsCode, CallOffId callOffId, SelectAdditionalServicePriceModel model)
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
                new { odsCode, callOffId });
        }

        [HttpGet("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatDeclarativeQuantityModel(callOffId, state.CatalogueItemName, state.Quantity)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                    typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/additional-service/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError("Quantity", error);

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = quantity;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatOnDemandQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                    typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/additional-service/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId, SelectFlatOnDemandQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError(nameof(model.Quantity), error);

            if (model.EstimationPeriod is null)
                ModelState.AddModelError(nameof(model.EstimationPeriod), "Time Unit is Required");

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = quantity;
            state.EstimationPeriod = model.EstimationPeriod.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditAdditionalService),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, catalogueItemId);

            var model = new EditAdditionalServiceModel(odsCode, state)
            {
                BackLink = GetEditAdditionalServiceBackLink(state, odsCode),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditAdditionalService(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, EditAdditionalServiceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            for (int i = 0; i < model.OrderItem.ServiceRecipients.Count; i++)
            {
                var error = model.OrderItem.ServiceRecipients[i].ValidateDeliveryDate(state.CommencementDate);

                if (error is not null)
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Day", error);

                var quantityModelStateKey = $"OrderItem.ServiceRecipients[{i}].Quantity";

                if (model.OrderItem.ServiceRecipients[i].Quantity is null
                    && !(ModelState[quantityModelStateKey].ValidationState == ModelValidationState.Invalid))
                    ModelState.AddModelError(quantityModelStateKey, "Enter a quantity");

                if (model.OrderItem.ServiceRecipients[i].Quantity <= 0
                    && !(ModelState[quantityModelStateKey].ValidationState == ModelValidationState.Invalid))
                    ModelState.AddModelError(quantityModelStateKey, "Quantity must be greater than 0");
            }

            var priceModelStateKey = "OrderItem.AgreedPrice";

            if (model.OrderItem.AgreedPrice > state.CataloguePrice.Price
                && !(ModelState[priceModelStateKey].ValidationState == ModelValidationState.Invalid))
                ModelState.AddModelError(priceModelStateKey, "Price cannot be greater than list price");

            if (model.OrderItem.AgreedPrice is null
                && !(ModelState[priceModelStateKey].ValidationState == ModelValidationState.Invalid))
                ModelState.AddModelError(priceModelStateKey, "Enter an agreed price");

            if (model.OrderItem.AgreedPrice < 0
                && !(ModelState[priceModelStateKey].ValidationState == ModelValidationState.Invalid))
                ModelState.AddModelError(priceModelStateKey, "Price cannot be negative");

            if (!ModelState.IsValid)
            {
                model.UpdateModel(state);
                return View(model);
            }

            state.AgreedPrice = model.OrderItem.AgreedPrice;
            state.ServiceRecipients = model.OrderItem.ServiceRecipients;

            await orderItemService.Create(callOffId, state);

            orderSessionService.ClearSession(callOffId);

            return RedirectToAction(
                nameof(Index),
                typeof(AdditionalServicesController).ControllerName(),
                new { odsCode, callOffId });
        }

        private string GetEditAdditionalServiceBackLink(CreateOrderItemModel state, string odsCode)
        {
            if (!state.IsNewSolution)
            {
                return Url.Action(nameof(Index), new { odsCode, callOffId = state.CallOffId });
            }

            if (state.CataloguePrice.ProvisioningType == ProvisioningType.Declarative)
                return Url.Action(nameof(SelectFlatDeclarativeQuantity), new { odsCode, callOffId = state.CallOffId });
            else if (state.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand)
                return Url.Action(nameof(SelectFlatOnDemandQuantity), new { odsCode, callOffId = state.CallOffId });

            return Url.Action(
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                typeof(AdditionalServiceRecipientsDateController).ControllerName(),
                new { odsCode, callOffId = state.CallOffId });
        }
    }
}
