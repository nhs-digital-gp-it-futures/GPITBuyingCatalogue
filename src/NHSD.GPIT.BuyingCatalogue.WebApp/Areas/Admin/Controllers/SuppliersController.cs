﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/manage-suppliers")]
    public sealed class SuppliersController : Controller
    {
        private readonly ISuppliersService suppliersService;

        public SuppliersController(ISuppliersService suppliersService)
        {
            this.suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await suppliersService.GetAllSuppliers();

            var model = new ManageSuppliersModel(suppliers);

            return View(model);
        }

        [HttpGet("{supplierId}")]
        public async Task<IActionResult> EditSupplier(int supplierId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var model = new EditSupplierModel(supplier)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(SuppliersController).ControllerName()),
            };

            return View(model);
        }

        [HttpPost("{supplierId}")]
        public async Task<IActionResult> EditSupplier(int supplierId, EditSupplierModel model)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            if (supplier.IsActive == model.SupplierStatus) // status hasn't changed
                RedirectToAction(nameof(Index), typeof(SuppliersController).ControllerName());

            var supplierStatus = new EditSupplierModel(supplier)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(SuppliersController).ControllerName()),
            };

            if (model.SupplierStatus && !(supplierStatus.AddressStatus && supplierStatus.DetailsStatus && supplierStatus.ContactsStatus))
                ModelState.AddModelError(nameof(model.SupplierStatus), "Mandatory section incomplete");

            if (!model.SupplierStatus)
            {
                var catalogueItems = await suppliersService.GetAllSolutionsForSupplier(supplierId);

                if (catalogueItems.Any(ci => ci.PublishedStatus == PublicationStatus.Published))
                {
                    ModelState.AddModelError(
                        nameof(model.SupplierStatus),
                        $"Cannot set to inactive while {catalogueItems.Count(ci => ci.PublishedStatus == PublicationStatus.Published)} solutions for this supplier are still published");
                }
            }

            if (!ModelState.IsValid)
                return View(supplierStatus);

            await suppliersService.UpdateSupplierActiveStatus(supplierId, model.SupplierStatus);

            return RedirectToAction(nameof(Index), typeof(SuppliersController).ControllerName());
        }

        [HttpGet("add-supplier/details")]
        public IActionResult AddSupplierDetails()
        {
            var model = new EditSupplierDetailsModel
            {
                BackLink = Url.Action(
                    nameof(Index),
                    typeof(SuppliersController).ControllerName()),
                BackLinkText = "Go back",
            };

            return View("EditSupplierDetails", model);
        }

        [HttpPost("add-supplier/details")]
        public async Task<IActionResult> AddSupplierDetails(EditSupplierDetailsModel model)
        {
            if ((await suppliersService.GetSupplierByName(model.SupplierName)) is not null)
                ModelState.AddModelError(nameof(model.SupplierName), "Supplier name already exists. Enter a different name");

            if ((await suppliersService.GetSupplierByLegalName(model.SupplierLegalName)) is not null)
                ModelState.AddModelError(nameof(model.SupplierLegalName), "Supplier legal name already exists. Enter a different name");

            if (!ModelState.IsValid)
                return View("EditSupplierDetails", model);

            var supplier = await suppliersService.AddSupplier(new ServiceContracts.Models.EditSupplierModel
            {
                SupplierName = model.SupplierName,
                SupplierLegalName = model.SupplierLegalName,
                AboutSupplier = model.AboutSupplier,
                SupplierWebsite = model.SupplierWebsite,
            });

            return RedirectToAction(
                nameof(EditSupplier),
                typeof(SuppliersController).ControllerName(),
                new { supplierId = supplier.Id });
        }

        [HttpGet("{supplierId}/details")]
        public async Task<IActionResult> EditSupplierDetails(int supplierId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var model = new EditSupplierDetailsModel(supplier)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(nameof(EditSupplier), typeof(SuppliersController).ControllerName(), new { supplierId }),
            };

            return View(model);
        }

        [HttpPost("{supplierId}/details")]
        public async Task<IActionResult> EditSupplierDetails(int supplierId, EditSupplierDetailsModel model)
        {
            var supplierByName = await suppliersService.GetSupplierByName(model.SupplierName);
            var supplierByLegalName = await suppliersService.GetSupplierByLegalName(model.SupplierLegalName);

            if (supplierByName is not null && supplierByName.Id != supplierId)
                ModelState.AddModelError(nameof(model.SupplierName), "Supplier name already exists. Enter a different name");

            if (supplierByLegalName is not null && supplierByLegalName.Id != supplierId)
                ModelState.AddModelError(nameof(model.SupplierLegalName), "Supplier legal name already exists. Enter a different name");

            if (!ModelState.IsValid)
                return View("EditSupplierDetails", model);

            var supplier = await suppliersService.EditSupplierDetails(supplierId, new ServiceContracts.Models.EditSupplierModel
            {
                SupplierName = model.SupplierName,
                SupplierLegalName = model.SupplierLegalName,
                AboutSupplier = model.AboutSupplier,
                SupplierWebsite = model.SupplierWebsite,
            });

            return RedirectToAction(
                nameof(EditSupplier),
                typeof(SuppliersController).ControllerName(),
                new { supplierId = supplier.Id });
        }

        [HttpGet("{supplierId}/address")]
        public async Task<IActionResult> EditSupplierAddress(int supplierId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var model = new EditSupplierAddressModel(supplier);

            return View(model);
        }

        [HttpPost("{supplierId}/address")]
        public async Task<IActionResult> EditSupplierAddress(int supplierId, EditSupplierAddressModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var address = new Address
            {
                Line1 = model.AddressLine1,
                Line2 = model.AddressLine2,
                Line3 = model.AddressLine3,
                Line4 = model.AddressLine4,
                Line5 = model.AddressLine5,
                Town = model.Town,
                County = model.County,
                Postcode = model.PostCode,
                Country = model.Country,
            };

            var supplier = await suppliersService.EditSupplierAddress(supplierId, address);

            return RedirectToAction(
                nameof(EditSupplier),
                typeof(SuppliersController).ControllerName(),
                new { supplierId = supplier.Id });
        }

        [HttpGet("{supplierId}/contacts")]
        public async Task<IActionResult> ManageSupplierContacts(int supplierId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var model = new ManageSupplierContactsModel(supplier)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(
                    nameof(EditSupplier),
                    typeof(SuppliersController).ControllerName(),
                    new { supplierId = supplier.Id }),
            };

            return View(model);
        }

        [HttpGet("{supplierId}/contacts/add-contact")]
        public async Task<IActionResult> AddSupplierContact(int supplierId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var model = new EditContactModel(supplier)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(
                    nameof(ManageSupplierContacts),
                    typeof(SuppliersController).ControllerName(),
                    new { supplierId = supplier.Id }),
            };

            return View("EditSupplierContact", model);
        }

        [HttpPost("{supplierId}/contacts/add-contact")]
        public async Task<IActionResult> AddSupplierContact(int supplierId, EditContactModel model)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            if (IsContactDuplicateOfExistingContact(supplier.SupplierContacts, model))
                ModelState.AddModelError("edit-contact", "A contact with these contact details already exists for this supplier");

            if (!ModelState.IsValid)
                return View("EditSupplierContact", model);

            var newContact = new SupplierContact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
            };

            supplier = await suppliersService.AddSupplierContact(supplierId, newContact);

            return RedirectToAction(
                nameof(ManageSupplierContacts),
                typeof(SuppliersController).ControllerName(),
                new { supplierId = supplier.Id });
        }

        [HttpGet("{supplierId}/contacts/{contactId}")]
        public async Task<IActionResult> EditSupplierContact(int supplierId, int contactId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var contact = supplier.SupplierContacts.Single(sc => sc.Id == contactId);

            var model = new EditContactModel(contact, supplier)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(
                    nameof(ManageSupplierContacts),
                    typeof(SuppliersController).ControllerName(),
                    new { supplierId = supplier.Id }),
            };

            return View(model);
        }

        [HttpPost("{supplierId}/contacts/{contactId}")]
        public async Task<IActionResult> EditSupplierContact(int supplierId, int contactId, EditContactModel model)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            if (IsContactDuplicateOfExistingContact(supplier.SupplierContacts, model))
                ModelState.AddModelError("edit-contact", "A contact with these contact details already exists for this supplier");

            if (!ModelState.IsValid)
                return View(model);

            var updatedContact = new SupplierContact
            {
                Id = model.ContactId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
            };

            supplier = await suppliersService.EditSupplierContact(supplierId, contactId, updatedContact);

            return RedirectToAction(
                nameof(ManageSupplierContacts),
                typeof(SuppliersController).ControllerName(),
                new { supplierId = supplier.Id });
        }

        [HttpGet("{supplierId}/contacts/{contactId}/delete")]
        public async Task<IActionResult> DeleteSupplierContact(int supplierId, int contactId)
        {
            var supplier = await suppliersService.GetSupplier(supplierId);

            var contact = supplier.SupplierContacts.Single(sc => sc.Id == contactId);

            var model = new DeleteContactModel(contact, supplier.Name)
            {
                BackLinkText = "Go back",
                BackLink = Url.Action(
                    nameof(EditSupplierContact),
                    typeof(SuppliersController).ControllerName(),
                    new { supplierId = supplier.Id, contactId = contact.Id }),
            };

            return View(model);
        }

        [HttpPost("{supplierId}/contacts/{contactId}/delete")]
        public async Task<IActionResult> DeleteSupplierContact(int supplierId, int contactId, DeleteContactModel model)
        {
            var supplier = await suppliersService.DeleteSupplierContact(supplierId, contactId);

            return RedirectToAction(
                nameof(ManageSupplierContacts),
                typeof(SuppliersController).ControllerName(),
                new { supplierId = supplier.Id });
        }

        private static bool IsContactDuplicateOfExistingContact(IEnumerable<SupplierContact> existingContacts, EditContactModel model)
        {
            return existingContacts.Any(sc =>
                string.Equals(sc.FirstName.Trim(), model.FirstName?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(sc.LastName.Trim(), model.LastName?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(sc.Email.Trim(), model.Email?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(sc.PhoneNumber.Trim(), model.PhoneNumber?.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(sc.Department.Trim(), model.Department?.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
