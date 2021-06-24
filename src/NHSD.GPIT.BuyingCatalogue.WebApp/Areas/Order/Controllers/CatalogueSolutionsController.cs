using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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
    public class CatalogueSolutionsController : Controller
    {
        private readonly ILogWrapper<CatalogueSolutionsController> logger;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;
        private readonly ISessionService sessionService;
        private readonly IOdsService odsService;
        private readonly IOrderItemService orderItemService;
        private readonly IDefaultDeliveryDateService defaultDeliveryDateService;
        private readonly IOrderSessionService orderSessionService;

        public CatalogueSolutionsController(
            ILogWrapper<CatalogueSolutionsController> logger,
            IOrderService orderService,
            ISolutionsService solutionsService,
            ISessionService sessionService,
            IOdsService odsService,
            IOrderItemService orderItemService,
            IDefaultDeliveryDateService defaultDeliveryDateService,
            IOrderSessionService orderSessionService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.defaultDeliveryDateService = defaultDeliveryDateService ?? throw new ArgumentNullException(nameof(defaultDeliveryDateService));
            this.orderSessionService = orderSessionService ?? throw new ArgumentNullException(nameof(orderSessionService));
        }

        // TODO: callOffId should be of type CallOffId
        public async Task<IActionResult> Index(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(Index)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            sessionService.ClearSession();

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);

            return View(new CatalogueSolutionsModel(odsCode, order, orderItems));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

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

            return View(new SelectSolutionModel(odsCode, callOffId, solutions, state.CatalogueItemId));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId, SelectSolutionModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            if (!ModelState.IsValid)
            {
                model.Solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);
                return View(model);
            }

            var existingOrder = await orderItemService.GetOrderItem(callOffId, model.SelectedSolutionId);

            if (existingOrder != null)
            {
                return RedirectToAction(
                    nameof(EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { odsCode, callOffId, id = existingOrder.CatalogueItemId });
            }

            state.CatalogueItemId = model.SelectedSolutionId;
            var solution = await solutionsService.GetSolution(state.CatalogueItemId);
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

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string odsCode, string callOffId)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId);

            var prices = solution.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            return View(new SelectSolutionPriceModel(odsCode, callOffId, state.CatalogueItemName, prices));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/solution/price")]
        public async Task<IActionResult> SelectSolutionPrice(string odsCode, string callOffId, SelectSolutionPriceModel model)
        {
            var state = orderSessionService.GetOrderStateFromSession();

            var solution = await solutionsService.GetSolution(state.CatalogueItemId);

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

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/solution/price/recipients")]
        public async Task<IActionResult> SelectSolutionServiceRecipients(string odsCode, string callOffId, string selectionMode)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipients)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            if (state.ServiceRecipients is null)
            {
                var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recipients.Select(x => new OrderItemRecipientModel(x)).ToList();
                orderSessionService.SetOrderStateToSession(state);
            }

            return View(new SelectSolutionServiceRecipientsModel(odsCode, callOffId, state.CatalogueItemName, state.ServiceRecipients, selectionMode, state.IsNewOrder, state.CatalogueItemId));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/solution/price/recipients")]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, string callOffId, SelectSolutionServiceRecipientsModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipients)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

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
                    new { odsCode, callOffId, id = state.CatalogueItemId });
            }

            return RedirectToAction(
                nameof(SelectSolutionServiceRecipientsDate),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/solution/price/recipients/date")]
        public async Task<IActionResult> SelectSolutionServiceRecipientsDate(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipientsDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            var defaultDeliveryDate = await defaultDeliveryDateService.GetDefaultDeliveryDate(callOffId, state.CatalogueItemId);

            return View(new SelectSolutionServiceRecipientsDateModel(odsCode, callOffId, state.CatalogueItemName, state.CommencementDate, state.PlannedDeliveryDate, defaultDeliveryDate));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/solution/price/recipients/date")]
        public async Task<IActionResult> SelectSolutionServiceRecipientsDate(string odsCode, string callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipientsDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            (DateTime? date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            state.PlannedDeliveryDate = date;

            await defaultDeliveryDateService.SetDefaultDeliveryDate(callOffId, state.CatalogueItemId, date.Value);

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
                new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

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
                new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        // TODO: callOffId should be of type CallOffId
        [HttpGet("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, string callOffId)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectFlatOnDemandQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new SelectFlatOnDemandQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity, state.TimeUnit));
        }

        // TODO: callOffId should be of type CallOffId
        [HttpPost("select/solution/price/flat/ondemand")]
        public IActionResult SelectFlatOnDemandQuantity(string odsCode, string callOffId, SelectFlatOnDemandQuantityModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            (int? quantity, var error) = model.GetQuantity();

            if (error != null)
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
                new { odsCode, callOffId, id = state.CatalogueItemId });
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: rename id to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpGet("{id}")]
        public async Task<IActionResult> EditSolution(string odsCode, string callOffId, string id)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(EditSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var isNewSolution = await orderSessionService.InitialiseStateForEdit(odsCode, callOffId, id);

            var state = orderSessionService.GetOrderStateFromSession();

            return View(new EditSolutionModel(odsCode, callOffId, id, state, isNewSolution));
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: rename id to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpPost("{id}")]
        public async Task<IActionResult> EditSolution(string odsCode, string callOffId, string id, EditSolutionModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(EditSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = orderSessionService.GetOrderStateFromSession();

            for (int i = 0; i < model.OrderItem.ServiceRecipients.Count; i++)
            {
                (DateTime? date, var error) = model.OrderItem.ServiceRecipients[i].ToDateTime(state.CommencementDate);

                if (error != null)
                {
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Day", error);
                }

                if (model.OrderItem.ServiceRecipients[i].Quantity is null or 0)
                    ModelState.AddModelError($"OrderItem.ServiceRecipients[{i}].Quantity", "Quantity is Required");
            }

            var solutionListPrices = await solutionsService.GetSolutionListPrices(state.CatalogueItemId);

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

        // TODO: callOffId should be of type CallOffId
        // TODO: rename id to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpGet("delete/{id}/confirmation/{solutionName}")]
        public async Task<IActionResult> DeleteSolution(string odsCode, string callOffId, string id, string solutionName)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(DeleteSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(solutionName)} {solutionName}");

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteSolutionModel(odsCode, callOffId, id, solutionName, order.Description));
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: rename id to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpPost("delete/{id}/confirmation/{solutionName}")]
        public async Task<IActionResult> DeleteSolution(string odsCode, string callOffId, string id, string solutionName, DeleteSolutionModel model)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(DeleteSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(solutionName)} {solutionName}");

            await orderItemService.DeleteOrderItem(callOffId, id);

            return RedirectToAction(
                nameof(DeleteContinue),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { odsCode, callOffId, id, solutionName });
        }

        // TODO: callOffId should be of type CallOffId
        // TODO: rename id to solutionId or catalogueItemId
        // TODO: solutionId/catalogueItemId should be of type CatalogueItemId
        [HttpGet("delete/{id}/confirmation/{solutionName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string solutionName)
        {
            // TODO: logger invocations should pass values as args
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(DeleteContinue)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(solutionName)} {solutionName}");

            return View(new DeleteContinueModel(odsCode, callOffId, solutionName));
        }
    }
}
