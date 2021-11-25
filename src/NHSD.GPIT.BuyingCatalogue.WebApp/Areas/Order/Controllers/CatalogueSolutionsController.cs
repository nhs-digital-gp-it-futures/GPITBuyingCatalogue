using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions")]
    public sealed class CatalogueSolutionsController : Controller
    {
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderSessionService orderSessionService;

        public CatalogueSolutionsController(
            IOrderService orderService,
            ISolutionsService solutionsService,
            IOrderItemService orderItemService,
            IOrderSessionService orderSessionService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);

            var model = new CatalogueSolutionsModel(odsCode, order, orderItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var state = orderSessionService.InitialiseStateForCreate(order, CatalogueItemType.Solution, null, null);

            var solutions = await solutionsService.GetSupplierSolutions(order.SupplierId);

            var model = new SelectSolutionModel(odsCode, callOffId, solutions, state.CatalogueItemId)
            {
                BackLink = Url.Action(nameof(Index), new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, CallOffId callOffId, SelectSolutionModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId.GetValueOrDefault());

            if (existingOrder is not null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId, existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());
            state.CatalogueItemName = solution.Name;
            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            if (!prices.Any())
                throw new InvalidOperationException($"Catalogue Solution {state.CatalogueItemId.GetValueOrDefault()} does not have any Flat prices associated");

            if (prices.Count == 1)
            {
                state = orderSessionService.SetPrice(callOffId, prices.Single());

                state.SkipPriceSelection = true;
                orderSessionService.SetOrderStateToSession(state);

                return RedirectToAction(
                    nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients),
                    typeof(CatalogueSolutionRecipientsController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(SelectSolutionPrice),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            var model = new SelectSolutionPriceModel(odsCode, state.CatalogueItemName, prices)
            {
                BackLink = Url.Action(nameof(SelectSolution), new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string odsCode, CallOffId callOffId, SelectSolutionPriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(cp => cp.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(callOffId, cataloguePrice);

            return RedirectToAction(
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients),
                typeof(CatalogueSolutionRecipientsController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatDeclarativeQuantityModel(callOffId, state.CatalogueItemName, state.Quantity)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                    typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error is not null)
                ModelState.AddModelError("Quantity", error);

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = quantity;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatOnDemandQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                    typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId, SelectFlatOnDemandQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error is not null)
                ModelState.AddModelError(nameof(model.Quantity), error);

            if (model.EstimationPeriod is null)
                ModelState.AddModelError(nameof(model.EstimationPeriod), "Time Unit is required");

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = quantity;
            state.EstimationPeriod = model.EstimationPeriod.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditSolution(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, catalogueItemId);

            var model = new EditSolutionModel(odsCode, state)
            {
                BackLink = GetEditSolutionBackLink(state, odsCode),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditSolution(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId, EditSolutionModel model)
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
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }

        private string GetEditSolutionBackLink(CreateOrderItemModel state, string odsCode)
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
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                new { odsCode, callOffId = state.CallOffId });
        }
    }
}
