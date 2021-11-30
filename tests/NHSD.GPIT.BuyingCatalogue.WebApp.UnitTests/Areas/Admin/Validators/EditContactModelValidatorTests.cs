using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditContactModelValidatorTests
    {
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

            var model = new EditContactModel(existingContact, supplier)
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
            supplier.SupplierContacts = supplierContacts;

            suppliersService.Setup(s => s.GetSupplier(existingContact.SupplierId))
                .ReturnsAsync(supplier);

            var model = new EditContactModel(existingContact, supplier);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
