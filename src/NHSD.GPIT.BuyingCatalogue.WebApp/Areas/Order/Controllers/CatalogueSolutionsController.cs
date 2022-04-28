using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/catalogue-solutions")]
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

        public async Task<IActionResult> Index(string internalOrgId, CallOffId callOffId)
        {
            orderSessionService.ClearSession(callOffId);

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, internalOrgId, CatalogueItemType.Solution);

            var model = new CatalogueSolutionsModel(internalOrgId, order, orderItems)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            var state = orderSessionService.InitialiseStateForCreate(order, CatalogueItemType.Solution, null, null);

            var solutions = await solutionsService.GetSupplierSolutions(order.SupplierId);

            var model = new SelectSolutionModel(internalOrgId, callOffId, solutions, state.CatalogueItemId)
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution")]
        public async Task<IActionResult> SelectSolution(string internalOrgId, CallOffId callOffId, SelectSolutionModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            if (!ModelState.IsValid)
            {
                model.Solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, internalOrgId, model.SelectedSolutionId.GetValueOrDefault());

            if (existingOrder is not null)
            {
                orderSessionService.ClearSession(callOffId);

                return RedirectToAction(
                    nameof(EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId, existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());
            state.CatalogueItemName = solution.Name;
            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

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
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(SelectSolutionPrice),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var solution = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            var prices = solution.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            var model = new SelectSolutionPriceModel(internalOrgId, state.CatalogueItemName, prices)
            {
                BackLink = Url.Action(nameof(SelectSolution), new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string internalOrgId, CallOffId callOffId, SelectSolutionPriceModel model)
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
                new { internalOrgId, callOffId });
        }

        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatDeclarativeQuantityModel(callOffId, state.CatalogueItemName, state.Quantity)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                    typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string internalOrgId, CallOffId callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = model.QuantityAsInt;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId, state.CatalogueItemId });
        }

        [HttpGet("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string internalOrgId, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            var model = new SelectFlatOnDemandQuantityModel(internalOrgId, callOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                    typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string internalOrgId, CallOffId callOffId, SelectFlatOnDemandQuantityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession(callOffId);

            state.Quantity = model.QuantityAsInt;
            state.EstimationPeriod = model.EstimationPeriod.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId, state.CatalogueItemId });
        }

        [HttpGet("{catalogueItemId}")]
        public async Task<IActionResult> EditSolution(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = await orderSessionService.InitialiseStateForEdit(internalOrgId, callOffId, catalogueItemId);

            var model = new EditSolutionModel(internalOrgId, state)
            {
                BackLink = GetEditSolutionBackLink(state, internalOrgId),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}")]
        public async Task<IActionResult> EditSolution(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, EditSolutionModel model)
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
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private string GetEditSolutionBackLink(CreateOrderItemModel state, string internalOrgId)
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
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                typeof(CatalogueSolutionRecipientsDateController).ControllerName(),
                new { internalOrgId, callOffId = state.CallOffId });
        }
    }
}
