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

            var model = new SupplierModel(odsCode, order, supplier.SupplierContacts)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Supplier(string odsCode, CallOffId callOffId, SupplierModel model)
        {
            if (!ModelState.IsValid)
            {
                var supplier = await supplierService.GetSupplierFromBuyingCatalogue(model.Id);
                model.Address = supplier.Address;
                return View(model);
            }

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

            var model = new SupplierSearchModel(odsCode, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
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
            var backlink = Url.Action(nameof(SupplierSearch), new { odsCode, callOffId });

            if (string.IsNullOrWhiteSpace(search))
                return View("NoSupplierFound", new NoSupplierFoundModel() { BackLink = backlink });

            var suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);

            if (suppliers.Count == 0)
                return View("NoSupplierFound", new NoSupplierFoundModel() { BackLink = backlink });

            return View(new SupplierSearchSelectModel(odsCode, callOffId, suppliers) { BackLink = backlink });
        }

        [HttpPost("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string odsCode, CallOffId callOffId, SupplierSearchSelectModel model, [FromQuery] string search)
        {
            if (!ModelState.IsValid)
            {
                model.Suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);
                return View(model);
            }

            // Model validation ensures that a supplier is selected
            await supplierService.AddOrderSupplier(callOffId, model.SelectedSupplierId!.Value);

            return RedirectToAction(
                nameof(Supplier),
                typeof(SupplierController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
