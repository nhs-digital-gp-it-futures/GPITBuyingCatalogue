using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/supplier")]
    public class SupplierController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;
        private readonly IOrderService orderService;
        private readonly ISupplierService supplierService;

        public SupplierController(
            ILogWrapper<OrderController> logger,
            IOrderService orderService,
            ISupplierService supplierService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        [HttpGet]
        public async Task<IActionResult> Supplier(string odsCode, string callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order.Supplier is null)
                return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/supplier/search");

            return View(new SupplierModel(odsCode, order));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SupplierSearch(string odsCode, string callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order.Supplier is not null)
                return Redirect($"/order/organisations/{odsCode}/order/{callOffId}/supplier");

            return View(new SupplierSearchModel(odsCode, order));
        }

        [HttpPost("search")]
        public async Task<IActionResult> SupplierSearch(string odsCode, string callOffId, SupplierSearchModel model)
        {
            var suppliers = await supplierService.GetList(model.SearchString, null, null);

            if (suppliers.Count == 0)
                return View("NoSupplierFound");

            return View(new SupplierSearchSelectModel());

            // TODO - Display NoSupplierFound if no results
            // return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/supplier/search/select");
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
