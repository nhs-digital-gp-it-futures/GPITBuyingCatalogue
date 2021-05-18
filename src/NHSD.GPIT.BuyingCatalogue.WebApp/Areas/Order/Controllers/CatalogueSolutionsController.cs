using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public CatalogueSolutionsController(ILogWrapper<OrderController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(string odsCode, string callOffId)
        {
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id)
        {
            return View(new EditSolutionModel());
        }

        [HttpPost("{id}")]
        public IActionResult EditSolution(string odsCode, string callOffId, string id, EditSolutionModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions");
        }

        [HttpGet("select/solution")]
        public IActionResult SelectSolution(string odsCode, string callOffId)
        {
            return View(new SelectSolutionModel());
        }

        [HttpPost("select/solution")]
        public IActionResult SelectSolution(string odsCode, string callOffId, SelectSolutionModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions/select/solution/price/recipients");
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
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions/select/solution/price/recipients/date");
        }

        [HttpGet("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId)
        {
            return View(new SelectSolutionServiceRecipientsDateModel());
        }

        [HttpPost("select/solution/price/recipients/date")]
        public IActionResult SelectSolutionServiceRecipientsDate(string odsCode, string callOffId, SelectSolutionServiceRecipientsDateModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions/select/solution/price/flat/declarative");
        }

        [HttpGet("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId)
        {
            return View(new SelectFlatDeclarativeQuantityModel());
        }

        [HttpPost("select/solution/price/flat/declarative")]
        public IActionResult SelectFlatDeclarativeQuantity(string odsCode, string callOffId, SelectFlatDeclarativeQuantityModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions/neworderitem");
        }

        [HttpGet("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId)
        {
            return View(new NewOrderItemModel());
        }

        [HttpPost("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId, NewOrderItemModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions");
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}")]
        public IActionResult DeleteSolution(string odsCode, string callOffId, string id, string solutionName)
        {
            return View(new DeleteSolutionModel());
        }

        [HttpPost("delete/{id}/confirmation/{solutionName}")]
        public IActionResult DeleteSolution(string odsCode, string callOffId, string id, string solutionName, DeleteSolutionModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/catalogue-solutions/delete/10001-01/confirmation/AnywereConsult/continue");
        }

        [HttpGet("delete/{id}/confirmation/{solutionName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string solutionName)
        {
            return View();
        }
    }
}
