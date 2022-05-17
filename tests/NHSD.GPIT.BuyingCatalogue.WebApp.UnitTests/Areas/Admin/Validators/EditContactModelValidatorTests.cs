using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditContactModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.FirstName = firstName;

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage("Enter a first name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.LastName = lastName;

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage("Enter a last name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_PhoneNumberNullOrEmpty_SetsModelError(
            string phoneNumber,
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.PhoneNumber = phoneNumber;

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber)
                .WithErrorMessage("Enter a phone number");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DepartmentNullOrEmpty_SetsModelError(
            string department,
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.Department = department;

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Department)
                .WithErrorMessage("Enter a department name");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_EmailNullOrEmpty_SetsModelError(
            string email,
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.Email = email;

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage("Enter an email address");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailFormatInvalid_SetsModelError(
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.Email = "abc";

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage("Enter an email address in the correct format, like name@example.com");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddDuplicateContact_SetsModelError(
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

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
                .WithErrorMessage("A contact with these contact details already exists for this supplier");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddContact_SetsModelError(
            SupplierContact supplierContact,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            supplierContact.Email = "a@a.com";
            supplier.SupplierContacts = new List<SupplierContact>();

            suppliersService.Setup(s => s.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

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
        [CommonAutoData]
        public static void Validate_EditDuplicateContact_SetsModelError(
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>())
            {
                ContactId = existingContact.Id + 1,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("edit-contact")
                .WithErrorMessage("A contact with these contact details already exists for this supplier");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditContact_NoModelError(
            List<SupplierContact> supplierContacts,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplierContacts[0];
            existingContact.Email = "a@a.com";

            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
