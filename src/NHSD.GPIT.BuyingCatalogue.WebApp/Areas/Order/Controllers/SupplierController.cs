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
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/supplier")]
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
        public async Task<IActionResult> Supplier(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithSupplier(callOffId, internalOrgId);

            if (order.Supplier is null)
            {
                return RedirectToAction(
                    nameof(SupplierSearch),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var supplier = await supplierService.GetSupplierFromBuyingCatalogue(order.Supplier.Id);

            var model = new SupplierModel(internalOrgId, order, supplier.SupplierContacts)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Supplier(string internalOrgId, CallOffId callOffId, SupplierModel model)
        {
            if (!ModelState.IsValid)
            {
                var supplier = await supplierService.GetSupplierFromBuyingCatalogue(model.Id);
                model.Address = supplier.Address;
                return View(model);
            }

            await supplierService.AddOrUpdateOrderSupplierContact(callOffId, internalOrgId, model.PrimaryContact);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SupplierSearch(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderWithSupplier(callOffId, internalOrgId);

            if (order.Supplier is not null)
            {
                return RedirectToAction(
                    nameof(SupplierController.Supplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = new SupplierSearchModel(internalOrgId, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("search")]
        public IActionResult SupplierSearch(string internalOrgId, CallOffId callOffId, SupplierSearchModel model)
        {
            return RedirectToAction(
                nameof(SupplierSearchSelect),
                typeof(SupplierController).ControllerName(),
                new { internalOrgId, callOffId, search = model.SearchString });
        }

        [HttpGet("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string internalOrgId, CallOffId callOffId, [FromQuery] string search, int? supplierId = null)
        {
            var order = await orderService.GetOrderWithSupplier(callOffId, internalOrgId);

            if (order?.Supplier is not null)
            {
                return RedirectToAction(
                    nameof(SupplierController.Supplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var backlink = Url.Action(nameof(SupplierSearch), new { internalOrgId, callOffId });

            if (string.IsNullOrWhiteSpace(search))
                return View("NoSupplierFound", new NoSupplierFoundModel() { BackLink = backlink });

            var suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);

            if (suppliers.Count == 0)
                return View("NoSupplierFound", new NoSupplierFoundModel() { BackLink = backlink });

            return View(new SupplierSearchSelectModel(internalOrgId, callOffId, suppliers, supplierId)
            {
                BackLink = backlink,
                SelectedSupplierId = supplierId,
            });
        }

        [HttpPost("search/select")]
        public async Task<IActionResult> SupplierSearchSelect(string internalOrgId, CallOffId callOffId, SupplierSearchSelectModel model, [FromQuery] string search)
        {
            if (!ModelState.IsValid)
            {
                model.Suppliers = await supplierService.GetListFromBuyingCatalogue(search, null, null);
                return View(model);
            }

            return RedirectToAction(
                nameof(ConfirmSupplier),
                typeof(SupplierController).ControllerName(),
                new
                {
                    internalOrgId,
                    callOffId,
                    search,
                    supplierId = model.SelectedSupplierId!.Value,
                });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmSupplier(string internalOrgId, CallOffId callOffId, string search, int supplierId)
        {
            var order = await orderService.GetOrderWithSupplier(callOffId, internalOrgId);

            if (order?.Supplier is not null)
            {
                return RedirectToAction(
                    nameof(Supplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var supplier = await supplierService.GetSupplierFromBuyingCatalogue(supplierId);

            if (supplier == null)
            {
                return RedirectToAction(
                    nameof(SupplierSearch),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return View(new ConfirmSupplierModel(internalOrgId, callOffId, supplier, search)
            {
                BackLink = Url.Action(nameof(SupplierSearchSelect), new { internalOrgId, callOffId, search, supplierId }),
            });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmSupplier(string internalOrgId, CallOffId callOffId, ConfirmSupplierModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await supplierService.AddOrderSupplier(callOffId, internalOrgId, model.SupplierId);

            return RedirectToAction(
                nameof(Supplier),
                typeof(SupplierController).ControllerName(),
                new { internalOrgId, callOffId });
        }
    }
}
