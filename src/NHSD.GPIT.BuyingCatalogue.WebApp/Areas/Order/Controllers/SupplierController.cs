using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/supplier")]
    public sealed class SupplierController : Controller
    {
        private readonly IOrderService orderService;
        private readonly ISupplierService supplierService;

        public SupplierController(
            IOrderService orderService,
            ISupplierService supplierService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        [HttpGet]
        public async Task<IActionResult> Supplier(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order.Supplier is null)
            {
                return RedirectToAction(
                    nameof(SupplierSearch),
                    typeof(SupplierController).ControllerName(),
                    new { odsCode, callOffId });
            }

            var supplier = await supplierService.GetSupplierFromBuyingCatalogue(order.Supplier.Id);

            return View(new SupplierModel(odsCode, order, supplier.SupplierContacts));
        }

        [HttpPost]
        public async Task<IActionResult> Supplier(string odsCode, CallOffId callOffId, SupplierModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await supplierService.AddOrUpdateOrderSupplierContact(callOffId, model.PrimaryContact);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SupplierSearch(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order.Supplier is not null)
            {
                return RedirectToAction(
                    nameof(Supplier),
                    typeof(SupplierController).ControllerName(),
                    new { odsCode, callOffId });
            }

            return View(new SupplierSearchModel(odsCode, order));
        }

        [HttpPost("search")]
        public IActionResult SupplierSearch(string odsCode, CallOffId callOffId, SupplierSearchModel model)
        {
            return RedirectToAction(
                nameof(SupplierSearchSelect),
                typeof(SupplierController).ControllerName(),
                new { odsCode, callOffId, search = model.SearchString });
        }

        [HttpGet("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string odsCode, CallOffId callOffId, [FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return View("NoSupplierFound", new NoSupplierFoundModel(odsCode, callOffId));

            var suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);

            if (suppliers.Count == 0)
                return View("NoSupplierFound", new NoSupplierFoundModel(odsCode, callOffId));

            return View(new SupplierSearchSelectModel(odsCode, callOffId, suppliers));
        }

        [HttpPost("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string odsCode, CallOffId callOffId, SupplierSearchSelectModel model, [FromQuery] string search)
        {
            if (!ModelState.IsValid)
            {
                model.Suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);
                return View(model);
            }

            await supplierService.AddOrderSupplier(callOffId, model.SelectedSupplierId);

            return RedirectToAction(
                nameof(Supplier),
                typeof(SupplierController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
