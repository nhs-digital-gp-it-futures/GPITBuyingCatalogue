using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditContactModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_PersonalDetailsNullOrEmpty_SetsModelError(
            string value,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.FirstName = value;
            existingContact.LastName = value;
            existingContact.Department = value;

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(ContactModelValidator.PersonalDetailsMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.FirstName = firstName;

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(ContactModelValidator.FirstNameMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.LastName = lastName;

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage(ContactModelValidator.LastNameMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_ContactDetailsNullOrEmpty_SetsModelError(
            string value,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.Email = value;
            existingContact.PhoneNumber = value;

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber)
                .WithErrorMessage(ContactModelValidator.ContactDetailsMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailFormatInvalid_SetsModelError(
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.Email = "abc";

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(ContactModelValidator.EmailAddressFormatErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddDuplicateContact_SetsModelError(
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            suppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var model = new EditContactModel(supplier)
            {
                FirstName = existingContact.FirstName,
                LastName = existingContact.LastName,
                Email = existingContact.Email,
                PhoneNumber = existingContact.PhoneNumber,
                Department = existingContact.Department,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("edit-contact")
                .WithErrorMessage(EditContactModelValidator.DuplicateContactErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddContact_SetsModelError(
            SupplierContact supplierContact,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            supplierContact.Email = "a@a.com";
            supplier.SupplierContacts = new List<SupplierContact>();

            suppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var model = new EditContactModel(supplier)
            {
                FirstName = supplierContact.FirstName,
                LastName = supplierContact.LastName,
                Email = supplierContact.Email,
                PhoneNumber = supplierContact.PhoneNumber,
                Department = supplierContact.Department,
            };

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditDuplicateContact_SetsModelError(
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>())
            {
                ContactId = existingContact.Id + 1,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("edit-contact")
                .WithErrorMessage(EditContactModelValidator.DuplicateContactErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditContact_NoModelError(
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.Email = "a@a.com";

            suppliersService.GetSupplier(existingContact.SupplierId).Returns(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
