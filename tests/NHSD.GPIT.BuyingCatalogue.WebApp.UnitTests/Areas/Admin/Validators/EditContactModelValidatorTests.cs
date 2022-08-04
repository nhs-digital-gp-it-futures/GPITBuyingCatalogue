using System.Collections.Generic;
using System.Linq;
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
        public static void Validate_PersonalDetailsNullOrEmpty_SetsModelError(
            string value,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.FirstName = value;
            existingContact.LastName = value;
            existingContact.Department = value;

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(EditContactModelValidator.PersonalDetailsMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_FirstNameNullOrEmpty_SetsModelError(
            string firstName,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.FirstName = firstName;

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FirstName)
                .WithErrorMessage(EditContactModelValidator.FirstNameMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_LastNameNullOrEmpty_SetsModelError(
            string lastName,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.LastName = lastName;

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LastName)
                .WithErrorMessage(EditContactModelValidator.LastNameMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_ContactDetailsNullOrEmpty_SetsModelError(
            string value,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.Email = value;
            existingContact.PhoneNumber = value;

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber)
                .WithErrorMessage(EditContactModelValidator.ContactDetailsMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailFormatInvalid_SetsModelError(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.Email = "abc";

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Email)
                .WithErrorMessage(EditContactModelValidator.EmailAddressFormatErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddDuplicateContact_SetsModelError(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            suppliersService
                .Setup(s => s.GetSupplier(supplier.Id))
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
                .WithErrorMessage(EditContactModelValidator.DuplicateContactErrorMessage);
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

            suppliersService
                .Setup(s => s.GetSupplier(supplier.Id))
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
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>())
            {
                ContactId = existingContact.Id + 1,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("edit-contact")
                .WithErrorMessage(EditContactModelValidator.DuplicateContactErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditContact_NoModelError(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditContactModelValidator validator)
        {
            var existingContact = supplier.SupplierContacts.First();

            existingContact.Email = "a@a.com";

            suppliersService
                .Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier, new List<CatalogueItem>());

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
