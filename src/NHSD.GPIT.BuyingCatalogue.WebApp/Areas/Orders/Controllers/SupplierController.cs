using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/supplier")]
    public sealed class SupplierController : Controller
    {
        private readonly IOrderService orderService;
        private readonly ISupplierContactSessionService sessionService;
        private readonly ISupplierService supplierService;

        public SupplierController(
            IOrderService orderService,
            ISupplierContactSessionService sessionService,
            ISupplierService supplierService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        [HttpGet]
        public async Task<IActionResult> Supplier(string internalOrgId, CallOffId callOffId, int? selected = null)
        {
            var order = (await orderService.GetOrderWithSupplier(callOffId, internalOrgId)).Order;

            if (order.Supplier is null)
            {
                return RedirectToAction(
                    nameof(SelectSupplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var supplier = await supplierService.GetSupplierFromBuyingCatalogue(order.Supplier.Id);
            var supplierContact = supplier.SupplierContacts.FirstOrDefault(x => AreEqual(x, order.SupplierContact));
            var temporaryContact = sessionService.GetSupplierContact(callOffId, supplier.Id);

            if (temporaryContact == null)
            {
                if (order.SupplierContact != null
                    && supplierContact == null)
                {
                    temporaryContact = new SupplierContact
                    {
                        Id = SupplierContact.TemporaryContactId,
                        SupplierId = supplier.Id,
                        FirstName = order.SupplierContact.FirstName,
                        LastName = order.SupplierContact.LastName,
                        Department = order.SupplierContact.Department,
                        PhoneNumber = order.SupplierContact.Phone,
                        Email = order.SupplierContact.Email,
                    };
                }

                // we need something written to the session so the e2e tests don't break
                // hence possibly writing a null temporary contact here
                sessionService.SetSupplierContact(callOffId, supplier.Id, temporaryContact);
            }

            if (selected == null
                && order.SupplierContact != null)
            {
                selected = supplierContact?.Id ?? SupplierContact.TemporaryContactId;
            }

            var model = new SupplierModel(internalOrgId, callOffId, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                Contacts = GetSupplierContacts(callOffId, supplier),
                SelectedContactId = selected,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Supplier(string internalOrgId, CallOffId callOffId, SupplierModel model)
        {
            var supplier = await supplierService.GetSupplierFromBuyingCatalogue(model.SupplierId);
            var contacts = GetSupplierContacts(callOffId, supplier);

            if (!ModelState.IsValid)
            {
                model.Contacts = contacts;
                return View(model);
            }

            var contact = contacts.FirstOrDefault(x => x.Id == model.SelectedContactId);

            await supplierService.AddOrUpdateOrderSupplierContact(callOffId, internalOrgId, contact);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectSupplier(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderWithSupplier(callOffId, internalOrgId)).Order;

            if (order.Supplier is not null)
            {
                return RedirectToAction(
                    nameof(Supplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var suppliers = await supplierService.GetAllSuppliersByOrderType(order.OrderType);

            if (!order.OrderType.UsesSupplierSearch)
            {
                if (suppliers.Count == 0)
                {
                    return RedirectToAction(
                        nameof(NoAvailableSuppliers),
                        typeof(SupplierController).ControllerName(),
                        new { internalOrgId, callOffId });
                }
                else if (suppliers.Count == 1)
                {
                    return RedirectToAction(
                        nameof(ConfirmSupplier),
                        typeof(SupplierController).ControllerName(),
                        new { internalOrgId, callOffId, supplierId = suppliers.First().Id, onlyOption = true });
                }
            }

            var model = new SelectSupplierModel
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CallOffId = callOffId,
                InternalOrgId = internalOrgId,
                OrderType = order.OrderType,
                Suppliers = suppliers
                    .Select(x => new SelectOption<string>(x.Name, $"{x.Id}"))
                    .ToList(),
            };

            return View(model);
        }

        [HttpPost("select")]
        public IActionResult SelectSupplier(string internalOrgId, CallOffId callOffId, SelectSupplierModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var supplierId = int.Parse(model.SelectedSupplierId);

            return RedirectToAction(
                nameof(ConfirmSupplier),
                typeof(SupplierController).ControllerName(),
                new { internalOrgId, callOffId, supplierId });
        }

        [HttpGet("no-available-suppliers")]
        public IActionResult NoAvailableSuppliers(string internalOrgId, CallOffId callOffId)
        {
            return View(new NoAvailableSuppliersModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(
                        nameof(OrderController.Order),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId }),
            });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmSupplier(string internalOrgId, CallOffId callOffId, int supplierId, bool onlyOption = false)
        {
            var order = (await orderService.GetOrderWithSupplier(callOffId, internalOrgId)).Order;

            if (order?.Supplier is not null)
            {
                return RedirectToAction(
                    nameof(Supplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var supplier = await supplierService.GetSupplierFromBuyingCatalogue(supplierId);

            if (onlyOption == false && supplier == null)
            {
                return RedirectToAction(
                    nameof(SelectSupplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return View(new ConfirmSupplierModel(internalOrgId, callOffId, supplier, onlyOption)
            {
                BackLink = onlyOption
                    ? Url.Action(
                        nameof(OrderController.Order),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId })
                    : Url.Action(
                        nameof(SelectSupplier),
                        new { internalOrgId, callOffId }),
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

        [HttpGet("new-contact")]
        public async Task<IActionResult> NewContact(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderWithSupplier(callOffId, internalOrgId)).Order;

            if (order?.SupplierId is null)
            {
                return RedirectToAction(
                    nameof(SelectSupplier),
                    typeof(SupplierController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var newContact = sessionService.GetSupplierContact(callOffId, order.SupplierId.Value);

            return View(new NewContactModel(callOffId, order.SupplierId.Value, order.Supplier.Name)
            {
                Title = newContact == null
                    ? "Add a contact"
                    : $"{newContact.FirstName} {newContact.LastName} details",
                BackLink = Url.Action(nameof(Supplier), new { internalOrgId, callOffId }),
                FirstName = newContact?.FirstName,
                LastName = newContact?.LastName,
                Department = newContact?.Department,
                PhoneNumber = newContact?.PhoneNumber,
                Email = newContact?.Email,
            });
        }

        [HttpPost("new-contact")]
        public IActionResult NewContact(string internalOrgId, CallOffId callOffId, NewContactModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            sessionService.SetSupplierContact(callOffId, model.SupplierId, new SupplierContact
            {
                Id = SupplierContact.TemporaryContactId,
                SupplierId = model.SupplierId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Department = model.Department,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
            });

            return RedirectToAction(
                nameof(Supplier),
                typeof(SupplierController).ControllerName(),
                new { internalOrgId, callOffId, selected = SupplierContact.TemporaryContactId });
        }

        private static bool AreEqual(SupplierContact a, Contact b)
        {
            if (a == null
                || b == null)
            {
                return false;
            }

            return $"{a.FirstName}{a.LastName}{a.Department}{a.Email}" == $"{b.FirstName}{b.LastName}{b.Department}{b.Email}";
        }

        private List<SupplierContact> GetSupplierContacts(CallOffId callOffId, Supplier supplier)
        {
            var temporaryContact = sessionService.GetSupplierContact(callOffId, supplier.Id);

            var contacts = temporaryContact == null
                ? supplier.SupplierContacts
                : supplier.SupplierContacts.Union(new[] { temporaryContact });

            return contacts
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
        }
    }
}
