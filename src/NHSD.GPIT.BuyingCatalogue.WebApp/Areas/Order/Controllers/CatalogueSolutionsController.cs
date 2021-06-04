using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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

        public CatalogueSolutionsController(
            ILogWrapper<OrderController> logger,
            IOrderService orderService,
            ISolutionsService solutionsService,
            ISessionService sessionService,
            IOdsService odsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
        }

        public async Task<IActionResult> Index(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await orderService.GetOrder(callOffId);

            var state = new CatalogueSolutionStateModel { CommencementDate = order.CommencementDate, SupplierId = order.SupplierId };

            SetStateModel(state);

            return View(new CatalogueSolutionsModel(odsCode, order));
        }

        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var state = GetStateModel();

            var solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);

            return View(new SelectSolutionModel(odsCode, callOffId, solutions, state.SelectedSolutionId));
        }

        [HttpPost("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId, SelectSolutionModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            var state = GetStateModel();

            if (!ModelState.IsValid)
            {
                model.Solutions = await solutionsService.GetSupplierSolutions(state.SupplierId);
                return View(model);
            }

            state.SelectedSolutionId = model.SelectedSolutionId;

            state.SolutionName = (await solutionsService.GetSolution(state.SelectedSolutionId)).Name;

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

            var state = GetStateModel();

            if (state.ServiceRecipients == null)
            {
                var recpients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
                state.ServiceRecipients = recpients.Select(x => new ServiceRecipientsModel(x)).ToList();
                SetStateModel(state);
            }

            return View(new SelectSolutionServiceRecipientsModel(odsCode, callOffId, state.SolutionName, state.ServiceRecipients, selectionMode));
        }

        [HttpPost("select/solution/price/recipients")]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, string callOffId, SelectSolutionServiceRecipientsModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

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

            var state = GetStateModel();

            return View(new SelectSolutionServiceRecipientsDateModel(odsCode, callOffId, state.SolutionName, state.CommencementDate, state.PlannedDeliveryDate));
        }

        [HttpPost("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            var state = GetStateModel();

            (var date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            state.PlannedDeliveryDate = date;

            state.ServiceRecipients.All(c =>
            {
                c.PlannedDeliveryDate = date;
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

            // TODO - this is not always asked. Does it for Anywhere Consult but not EMIS Web GP
            var state = GetStateModel();

            return View(new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, state.SolutionName, state.Quantity));
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

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

            var state = GetStateModel();

            return View(new NewOrderItemModel(odsCode, callOffId, state.SolutionName));
        }

        [HttpPost("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId, NewOrderItemModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            model.ValidateNotNull(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                actionName: nameof(Index),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id)
        {
            return View(new EditSolutionModel());
        }

        [HttpPost("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id, EditSolutionModel model)
        {
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
        public IActionResult DeleteSolution(string odsCode, string callOffId, string id, string solutionName)
        {
            return View(new DeleteSolutionModel());
        }

        [HttpPost("delete/{id}/confirmation/{solutionName}")]
        public IActionResult DeleteSolution(string odsCode, string callOffId, string id, string solutionName, DeleteSolutionModel model)
        {
            return RedirectToAction(
                actionName: nameof(DeleteContinue),
                controllerName: typeof(CatalogueSolutionsController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = callOffId });
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string solutionName)
        {
            return View(new DeleteContinueModel());
        }

        private CatalogueSolutionStateModel GetStateModel()
        {
            var state = sessionService.GetObject<CatalogueSolutionStateModel>("CatalogueItemState");

            return state ?? new CatalogueSolutionStateModel();
        }

        private void SetStateModel(CatalogueSolutionStateModel state)
        {
            sessionService.SetObject("CatalogueItemState", state);
        }
    }
}
