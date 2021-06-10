using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
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
        private readonly ILogWrapper<OrderController> logger;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;
        private readonly ISessionService sessionService;
        private readonly IOdsService odsService;
        private readonly IOrderItemService orderItemService;

        public CatalogueSolutionsController(
            ILogWrapper<OrderController> logger,
            IOrderService orderService,
            ISolutionsService solutionsService,
            ISessionService sessionService,
            IOdsService odsService,
            IOrderItemService orderItemService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        public async Task<IActionResult> Index(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(Index)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);
            var orderItems = await orderItemService.GetOrderItems(callOffId, CatalogueItemType.Solution);

            return View(new CatalogueSolutionsModel(odsCode, order, orderItems));
        }

        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            var state = new CreateOrderItemModel
            {
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = CatalogueItemType.Solution,
            };

            SetStateModel(state);

            var solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);

            return View(new SelectSolutionModel(odsCode, callOffId, solutions, state.CatalogueSolutionId));
        }

        [HttpPost("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId, SelectSolutionModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = GetStateModel();

            if (!ModelState.IsValid)
            {
                model.Solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);
                return View(model);
            }

            state.CatalogueSolutionId = model.SelectedSolutionId;

            var solution = await solutionsService.GetSolution(state.CatalogueSolutionId);
            state.CatalogueItemName = solution.Name;

            var cataloguePrice = solution.CataloguePrices.Single(x => x.CataloguePriceTypeId == CataloguePriceType.Flat.Id);
            state.ProvisioningType = ProvisioningType.Parse(cataloguePrice.ProvisioningType.Name);
            state.CurrencyCode = cataloguePrice.CurrencyCode;
            state.Type = CataloguePriceType.Parse(cataloguePrice.CataloguePriceType.Name);
            state.ItemUnit = new ItemUnitModel { Name = cataloguePrice.PricingUnit.Name, Description = cataloguePrice.PricingUnit.Description };
            state.TimeUnit = TimeUnit.Parse(cataloguePrice.TimeUnit.Name);

            // TODO - where does this come from?
            state.EstimationPeriod = TimeUnit.PerYear;

            SetStateModel(state);

            // TODO -- apears in old version that is this solution is already present, it jumps straight to the editor for that solution
            return RedirectToAction(
                actionName: nameof(SelectSolutionServiceRecipients),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("select/solution/price/recipients")]
        public async Task<IActionResult> SelectSolutionServiceRecipients(string odsCode, string callOffId, string selectionMode)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipients)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = GetStateModel();

            if (state.ServiceRecipients == null)
            {
                var recpients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recpients.Select(x => new OrderItemRecipientModel(x)).ToList();
                SetStateModel(state);
            }

            return View(new SelectSolutionServiceRecipientsModel(odsCode, callOffId, state.CatalogueItemName, state.ServiceRecipients, selectionMode));
        }

        [HttpPost("select/solution/price/recipients")]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, string callOffId, SelectSolutionServiceRecipientsModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipients)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = GetStateModel();

            if (!ModelState.IsValid)
                return View(model);

            state.ServiceRecipients = model.ServiceRecipients;

            SetStateModel(state);

            // TODO - if we came from the EditSolution page then we should go back to it rather than the date page
            return RedirectToAction(
                actionName: nameof(SelectSolutionServiceRecipientsDate),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipientsDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = GetStateModel();

            return View(new SelectSolutionServiceRecipientsDateModel(odsCode, callOffId, state.CatalogueItemName, state.CommencementDate, state.PlannedDeliveryDate));
        }

        [HttpPost("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectSolutionServiceRecipientsDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = GetStateModel();

            (var date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            state.PlannedDeliveryDate = date;

            state.ServiceRecipients.All(c =>
            {
                c.DeliveryDate = date;
                return true;
            });

            SetStateModel(state);

            return RedirectToAction(
                actionName: nameof(SelectFlatDeclarativeQuantity),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            // TODO - this is not always asked. Does it for Anywhere Consult but not EMIS Web GP
            var state = GetStateModel();

            return View(new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, state.CatalogueItemName, state.Quantity));
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(SelectFlatDeclarativeQuantity)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            (var quantity, var error) = model.GetQuantity();

            if (error != null)
                ModelState.AddModelError("Quantity", error);

            if (!ModelState.IsValid)
                return View(model);

            var state = GetStateModel();

            state.Quantity = quantity;

            state.ServiceRecipients.All(c =>
            {
                c.Quantity = quantity;
                return true;
            });

            SetStateModel(state);

            return RedirectToAction(
                actionName: nameof(NewOrderItem),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(NewOrderItem)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var state = GetStateModel();

            return View(new NewOrderItemModel(odsCode, callOffId, state.CatalogueItemName));
        }

        [HttpPost("neworderitem")]
        public async Task<IActionResult> NewOrderItem(string odsCode, string callOffId, NewOrderItemModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(NewOrderItem)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            if (!ModelState.IsValid)
                return View(model);

            var state = GetStateModel();

            // TODO - handle errors
            var result = await orderItemService.Create(callOffId, state);

            return RedirectToAction(
                actionName: nameof(Index),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> EditSolution(string odsCode, string callOffId, string id)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(EditSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var orderItem = await orderItemService.GetOrderItem(callOffId, id);

            var solution = await solutionsService.GetSolution(orderItem.CatalogueItemId.ToString());

            return View(new EditSolutionModel(odsCode, callOffId, solution.Name));
        }

        [HttpPost("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id, EditSolutionModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(EditSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            return RedirectToAction(
                actionName: nameof(Index),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("select/solution/price")]
        public IActionResult SelectSolutionPrice(string odsCode, string callOffId)
        {
            // TODO - this is not currently accessed by this skeleton framework
            return View(new SelectSolutionPriceModel());
        }

        [HttpPost("select/solution/price")]
        public IActionResult SelectSolutionPrice(string odsCode, string callOffId, SelectSolutionPriceModel model)
        {
            // TODO - this is not currently accessed by this skeleton framework
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients");
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}")]
        public async Task<IActionResult> DeleteSolution(string odsCode, string callOffId, string id, string solutionName)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            id.ValidateNotNullOrWhiteSpace(nameof(id));
            id.ValidateNotNullOrWhiteSpace(nameof(id));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(DeleteSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(solutionName)} {solutionName}");

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteSolutionModel(odsCode, callOffId, id, solutionName, order.Description));
        }

        [HttpPost("delete/{id}/confirmation/{solutionName}")]
        public async Task<IActionResult> DeleteSolution(string odsCode, string callOffId, string id, string solutionName, DeleteSolutionModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            id.ValidateNotNullOrWhiteSpace(nameof(id));
            id.ValidateNotNullOrWhiteSpace(nameof(id));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(CatalogueSolutionsController)}.{nameof(DeleteSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(solutionName)} {solutionName}");

            await orderItemService.DeleteOrderItem(callOffId, id);

            return RedirectToAction(
                actionName: nameof(DeleteContinue),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId, id = id, solutionName = solutionName });
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string solutionName)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            id.ValidateNotNullOrWhiteSpace(nameof(id));
            id.ValidateNotNullOrWhiteSpace(nameof(id));

            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(DeleteSolution)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(id)} {id}, {nameof(solutionName)} {solutionName}");

            return View(new DeleteContinueModel(odsCode, callOffId, solutionName));
        }

        private CreateOrderItemModel GetStateModel()
        {
            var state = sessionService.GetObject<CreateOrderItemModel>("CatalogueItemState");
            return state ?? new CreateOrderItemModel();
        }

        private void SetStateModel(CreateOrderItemModel state)
        {
            sessionService.SetObject("CatalogueItemState", state);
        }
    }
}
