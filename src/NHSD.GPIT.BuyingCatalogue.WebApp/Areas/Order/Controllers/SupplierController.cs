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
            {
                return RedirectToAction(
                    actionName: nameof(SupplierSearch),
                    controllerName: "Supplier",
                    routeValues: new { odsCode, callOffId });
            }

            return View(new SupplierModel(odsCode, order));
        }

        [HttpPost]
        public async Task<IActionResult> Supplier(string odsCode, string callOffId, SupplierModel model)
        {
            if (model is null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await supplierService.AddOrUpdateOrderSupplierContact(callOffId, model.PrimaryContact);

            return RedirectToAction();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SupplierSearch(string odsCode, string callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order.Supplier is not null)
            {
                return RedirectToAction(
                    actionName: nameof(Supplier),
                    controllerName: "Supplier",
                    routeValues: new { odsCode, callOffId });
            }

            return View(new SupplierSearchModel(odsCode, order));
        }

        [HttpPost("search")]
        public IActionResult SupplierSearch(string odsCode, string callOffId, SupplierSearchModel model)
        {
            return RedirectToAction(
                actionName: nameof(SupplierSearchSelect),
                controllerName: "Supplier",
                routeValues: new { odsCode, callOffId, search = model.SearchString });
        }

        [HttpGet("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string odsCode, string callOffId, [FromQuery]string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return View("NoSupplierFound");

            var suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);

            if (suppliers.Count == 0)
                return View("NoSupplierFound");

            return View(new SupplierSearchSelectModel(odsCode, callOffId, suppliers));
        }

        [HttpPost("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string odsCode, string callOffId, SupplierSearchSelectModel model)
        {
            if (model is null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await supplierService.AddOrderSupplier(callOffId, model.SelectedSupplierId);

            return RedirectToAction(
                actionName: nameof(Supplier),
                controllerName: "Supplier",
                routeValues: new { odsCode, callOffId });
        }
    }
}
