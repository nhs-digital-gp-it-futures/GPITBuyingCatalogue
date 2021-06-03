using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models;
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

        public CatalogueSolutionsController(
            ILogWrapper<OrderController> logger,
            IOrderService orderService,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        public async Task<IActionResult> Index(string odsCode, string callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new CatalogueSolutionsModel(odsCode, order));
        }

        [HttpGet("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id)
        {
            return View(new EditSolutionModel());
        }

        [HttpPost("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id, EditSolutionModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions");
        }

        [HttpGet("select/solution")]
        public async Task<IActionResult> SelectSolution(string odsCode, string callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var solutions = await solutionsService.GetSupplierSolutions(order.SupplierId);

            // TODO - handle no solutions returned

            return View(new SelectSolutionModel(odsCode, callOffId, solutions));
        }

        [HttpPost("select/solution")]
        public IActionResult SelectSolution(string odsCode, string callOffId, SelectSolutionModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients");
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

        [HttpGet("select/solution/price/recipients")]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, string callOffId)
        {
            return View(new SelectSolutionServiceRecipientsModel());
        }

        [HttpPost("select/solution/price/recipients")]
        public IActionResult SelectSolutionServiceRecipients(string odsCode, string callOffId, SelectSolutionServiceRecipientsModel model)
        {
            // TODO - if we came from the EditSolution page then we should go back to it rather than the date page
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients/date");
        }

        [HttpGet("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId)
        {
            return View(new SelectSolutionServiceRecipientsDateModel());
        }

        [HttpPost("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/declarative");
        }

        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId)
        {
            return View(new SelectFlatDeclarativeQuantityModel());
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/neworderitem");
        }

        [HttpGet("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId)
        {
            return View(new NewOrderItemModel());
        }

        [HttpPost("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId, NewOrderItemModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions");
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}")]
        public IActionResult DeleteSolution(string odsCode, string callOffId, string id, string solutionName)
        {
            return View(new DeleteSolutionModel());
        }

        [HttpPost("delete/{id}/confirmation/{solutionName}")]
        public IActionResult DeleteSolution(string odsCode, string callOffId, string id, string solutionName, DeleteSolutionModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/delete/{id}/confirmation/{solutionName}/continue");
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string solutionName)
        {
            return View(new DeleteContinueModel());
        }
    }
}
