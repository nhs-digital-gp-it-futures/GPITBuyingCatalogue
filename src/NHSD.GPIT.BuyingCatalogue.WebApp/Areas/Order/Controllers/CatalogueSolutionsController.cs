using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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
        private readonly ISessionService sessionService;
        private readonly IOdsService odsService;
        private readonly IOrderItemService orderItemService;
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IOrderSessionService orderSessionService;

        // TODO: too many dependencies, i.e. too many responsibilities
        public CatalogueSolutionsController(
            IOrderService orderService,
            ISolutionsService solutionsService,
            ISessionService sessionService,
            IOdsService odsService,
            IOrderItemService orderItemService,
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IOrderSessionService orderSessionService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        public async Task<IActionResult> Index(string odsCode, CallOffId callOffId)
        {
            sessionService.ClearSession();

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);

            return View(new CatalogueSolutionsModel(odsCode, order, orderItems));
        }

        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var state = new CreateOrderItemModel
            {
                IsNewOrder = true,
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = CatalogueItemType.Solution,
            };

            orderSessionService.SetOrderStateToSession(state);

            var solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);

            return View(new SelectSolutionModel(odsCode, callOffId, solutions, state.CatalogueItemId.GetValueOrDefault()));
        }

        [HttpPost("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, CallOffId callOffId, SelectSolutionModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            if (!ModelState.IsValid)
            {
                model.Solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId);

            if (existingOrder is not null)
            {
                return RedirectToAction(
                    nameof(EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId, solutionId = existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolution(state.CatalogueItemId.GetValueOrDefault());
            state.CatalogueItemName = solution.Name;
            orderSessionService.SetOrderStateToSession(state);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            if (prices.Count == 1)
            {
                orderSessionService.SetPrice(prices.Single());

                return RedirectToAction(
                    nameof(SelectSolutionServiceRecipients),
                    typeof(CatalogueSolutionsController).ControllerName(),
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
            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId.GetValueOrDefault());

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            return View(new SelectSolutionPriceModel(odsCode, callOffId, state.CatalogueItemName, prices));
        }

        [HttpPost("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string odsCode, CallOffId callOffId, SelectSolutionPriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId.GetValueOrDefault());

            if (!ModelState.IsValid)
            {
                model.SetPrices(solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList());
                return View(model);
            }

            var cataloguePrice = solution.CataloguePrices.Single(x => x.CataloguePriceId == model.SelectedPrice);

            orderSessionService.SetPrice(cataloguePrice);

            return RedirectToAction(
                nameof(SelectSolutionServiceRecipients),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/solution/price/recipients")]
        public async Task<IActionResult> SelectSolutionServiceRecipients(string odsCode, CallOffId callOffId, string selectionMode)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recipients.Select(x => new OrderItemRecipientModel(x)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectSolutionServiceRecipientsModel(
                odsCode,
                callOffId,
                state.CatalogueItemName,
                state.ServiceRecipients,
                selectionMode,
                state.IsNewOrder,
                state.CatalogueItemId.GetValueOrDefault()));
        }

        [HttpPost("select/solution/price/recipients")]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, CallOffId callOffId, SelectSolutionServiceRecipientsModel model)
        {
            if (!model.ServiceRecipients.Any(sr => sr.Selected))
                ModelState.AddModelError("ServiceRecipients[0].Selected", "Select a Service Recipient");

            var state = orderSessionService.GetOrderStateFromSession();

            if (!ModelState.IsValid)
                return View(model);

            state.ServiceRecipients = model.ServiceRecipients;

            orderSessionService.SetOrderStateToSession(state);

            if (!state.IsNewOrder)
            {
                return RedirectToAction(
                    nameof(EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId, solutionId = state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectSolutionServiceRecipientsDate),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("select/solution/price/recipients/date")]
        public async Task<IActionResult> SelectSolutionServiceRecipientsDate(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            var defaultDeliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, state.CatalogueItemId.GetValueOrDefault());

            return View(new SelectSolutionServiceRecipientsDateModel(odsCode, callOffId, state.CatalogueItemName, state.CommencementDate, state.PlannedDeliveryDate, defaultDeliveryDate));
        }

        [HttpPost("select/solution/price/recipients/date")]
        public async Task<IActionResult> SelectSolutionServiceRecipientsDate(string odsCode, CallOffId callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            (DateTime? date, var error) = model.ToDateTime();

            if (error is not null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            state.PlannedDeliveryDate = date;

            await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, state.CatalogueItemId.GetValueOrDefault(), date.Value);

            orderSessionService.SetOrderStateToSession(state);

            if (state.ProvisioningType == ProvisioningType.Declarative)
            {
                return RedirectToAction(
                    nameof(SelectFlatDeclarativeQuantity),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId });
            }

            if (state.ProvisioningType == ProvisioningType.OnDemand)
            {
                return RedirectToAction(
                    nameof(SelectFlatOnDemandQuantity),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, solutionId = state.CatalogueItemId });
        }

        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            return View(new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity));
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, CallOffId callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error is not null)
                ModelState.AddModelError("Quantity", error);

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession();

            state.Quantity = quantity;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, solutionId = state.CatalogueItemId });
        }

        [HttpGet("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            return View(new SelectFlatOnDemandQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity, state.TimeUnit));
        }

        [HttpPost("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, CallOffId callOffId, SelectFlatOnDemandQuantityModel model)
        {
            (int? quantity, var error) = model.GetQuantity();

            if (error is not null)
                ModelState.AddModelError(nameof(model.Quantity), error);

            if (model.TimeUnit is null)
                ModelState.AddModelError(nameof(model.TimeUnit), "Time Unit is Required");

            if (!ModelState.IsValid)
                return View(model);

            var state = orderSessionService.GetOrderStateFromSession();

            state.Quantity = quantity;
            state.TimeUnit = model.TimeUnit.Value;

            orderSessionService.SetOrderStateToSession(state);

            return RedirectToAction(
                nameof(EditSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, solutionId = state.CatalogueItemId });
        }

        [HttpGet("{solutionId}")]
        public async Task<IActionResult> EditSolution(string odsCode, CallOffId callOffId, CatalogueItemId solutionId)
        {
            var isNewSolution = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, solutionId);

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new EditSolutionModel(odsCode, callOffId, state, isNewSolution));
        }

        [HttpPost("{solutionId}")]
        public async Task<IActionResult> EditSolution(string odsCode, CallOffId callOffId, CatalogueItemId solutionId, EditSolutionModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            for (int i = 0; i < model.OrderItem.ServiceRecipients.Count; i++)
            {
                (DateTime? date, var error) = model.OrderItem.ServiceRecipients[i].ToDateTime(state.CommencementDate);

                if (error is not null)
                {
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Day", error);
                }

                if (model.OrderItem.ServiceRecipients[i].Quantity is null or 0)
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Quantity", "Quantity is Required");
            }

            var solutionListPrices = await solutionsService.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault());

            var solutionPrice = solutionListPrices.CataloguePrices.FirstOrDefault(
                cp => cp.ProvisioningType == state.ProvisioningType
                    && cp.CataloguePriceType == state.Type
                    && (cp.TimeUnit is null || cp.TimeUnit == state.TimeUnit));

            if (solutionPrice is not null)
            {
                if (model.OrderItem.Price > solutionPrice.Price)
                    ModelState.AddModelError("OrderItem.Price", "Price cannot be greater than list price");
            }

            if (!ModelState.IsValid)
            {
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
                nameof(Index),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("delete/{solutionId}/confirmation/{solutionName}")]
        public async Task<IActionResult> DeleteSolution(string odsCode, CallOffId callOffId, CatalogueItemId solutionId, string solutionName)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteSolutionModel(odsCode, callOffId, solutionId, solutionName, order.Description));
        }

        [HttpPost("delete/{solutionId}/confirmation/{solutionName}")]
        public async Task<IActionResult> DeleteSolution(string odsCode, CallOffId callOffId, CatalogueItemId solutionId, string solutionName, DeleteSolutionModel model)
        {
            await orderItemService.DeleteOrderItem(callOffId, solutionId);

            return RedirectToAction(
                nameof(Index),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
