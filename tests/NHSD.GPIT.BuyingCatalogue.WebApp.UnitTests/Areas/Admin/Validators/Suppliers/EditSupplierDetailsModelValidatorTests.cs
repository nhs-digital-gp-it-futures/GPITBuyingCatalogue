using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditSupplierDetailsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoWebsite_DoesNotValidate(
            [Frozen] IUrlValidator urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel();

            var result = validator.TestValidate(model);

            urlValidator.DidNotReceive().IsValidUrl(Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            EditSupplierDetailsModel model,
            [Frozen] IUrlValidator urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            urlValidator.IsValidUrl(model.SupplierWebsite).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierWebsite)
                .WithErrorMessage("Enter a prefix to the URL, either http or https");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_InvalidWebsite_SetsModelError(
            [Frozen] IUrlValidator urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel { SupplierWebsite = "http://wiothaoih" };

            urlValidator.IsValidUrl(model.SupplierWebsite).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierWebsite)
                .WithErrorMessage("Enter a valid URL");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidWebsite_NoModelError(
            Uri uri,
            [Frozen] IUrlValidator urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel { SupplierWebsite = uri.ToString() };
            urlValidator.IsValidUrl(model.SupplierWebsite).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierWebsite);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddSupplierName_SetsModelError(
            [Frozen] ISuppliersService suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierName = "Supplier Name",
            };

            suppliersService.GetSupplierByName(model.SupplierName).Returns((Supplier)default);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierName);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditDuplicateSupplierName_SetsModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id + 1,
                SupplierName = supplier.Name,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.GetSupplierByName(model.SupplierName).Returns(supplier);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierName)
                .WithErrorMessage("Supplier name already exists. Enter a different name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditSupplierName_NoModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.GetSupplierByName(model.SupplierName).Returns(supplier);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierName);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_AddSupplierLegalName_SetsModelError(
            [Frozen] ISuppliersService suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierLegalName = "Supplier Legal Name",
            };

            suppliersService.GetSupplierByLegalName(model.SupplierLegalName).Returns((Supplier)default);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierLegalName);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditDuplicateSupplierLegalName_SetsModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id + 1,
                SupplierLegalName = supplier.LegalName,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.GetSupplierByLegalName(model.SupplierLegalName).Returns(supplier);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierLegalName)
                .WithErrorMessage("Supplier legal name already exists. Enter a different name");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EditSupplierLegalName_SetsModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] ISuppliersService suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id,
                SupplierLegalName = supplier.LegalName,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.GetSupplierByLegalName(model.SupplierLegalName).Returns(supplier);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierLegalName);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SupplierNameNullOrEmpty_SetsModelError(
            string supplierName,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierName = supplierName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierName)
                .WithErrorMessage("Enter a supplier name");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SupplierLegalNameNullOrEmpty_SetsModelError(
            string supplierLegalName,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierLegalName = supplierLegalName,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierLegalName)
                .WithErrorMessage("Enter a supplier legal name");
        }
    }
}
