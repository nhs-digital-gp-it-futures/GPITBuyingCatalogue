using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/supplier")]
    public class SupplierController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public SupplierController(ILogWrapper<OrderController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Supplier(string odsCode, string callOffId)
        {
            // TODO - display view if supplier already defined otherwise rediect to select
            // return View(new SupplierModel());
            return Redirect($"/order/organisation/03F/order/C01005-01/supplier/search");
        }

        [HttpGet("search")]
        public IActionResult SupplierSearch(string odsCode, string callOffId)
        {
            return View(new SupplierSearchModel());
        }

        [HttpPost("search")]
        public IActionResult SupplierSearch(string odsCode, string callOffId, SupplierSearchModel model)
        {
            // TODO - Display NoSupplierFound if no results
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/supplier/search/select");
        }

        [HttpGet("search/select")]
        public IActionResult SupplierSearchSelect(string odsCode, string callOffId)
        {
            return View(new SupplierSearchSelectModel());
        }

        [HttpPost("search/select")]
        public IActionResult SupplierSearchSelect(string odsCode, string callOffId, SupplierSearchSelectModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/supplier");
        }
    }
}
