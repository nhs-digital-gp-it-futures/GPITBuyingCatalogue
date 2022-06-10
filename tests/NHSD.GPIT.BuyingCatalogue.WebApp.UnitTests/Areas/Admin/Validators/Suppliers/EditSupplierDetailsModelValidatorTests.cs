using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class EditSupplierDetailsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoWebsite_DoesNotValidate(
            [Frozen] Mock<IUrlValidator> urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel();

            var result = validator.TestValidate(model);

            urlValidator.Verify(uv => uv.IsValidUrl(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            EditSupplierDetailsModel model,
            [Frozen] Mock<IUrlValidator> urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            urlValidator.Setup(uv => uv.IsValidUrl(model.SupplierWebsite))
                .Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierWebsite)
                .WithErrorMessage("Enter a prefix to the URL, either http or https");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InvalidWebsite_SetsModelError(
            [Frozen] Mock<IUrlValidator> urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel { SupplierWebsite = "http://wiothaoih" };

            urlValidator.Setup(uv => uv.IsValidUrl(model.SupplierWebsite))
                .Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierWebsite)
                .WithErrorMessage("Enter a valid URL");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidWebsite_NoModelError(
            Uri uri,
            [Frozen] Mock<IUrlValidator> urlValidator,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel { SupplierWebsite = uri.ToString() };
            urlValidator.Setup(uv => uv.IsValidUrl(model.SupplierWebsite))
                .Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierWebsite);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddSupplierName_SetsModelError(
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierName = "Supplier Name",
            };

            suppliersService.Setup(s => s.GetSupplierByName(model.SupplierName))
                .ReturnsAsync((Supplier)default);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierName);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditDuplicateSupplierName_SetsModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id + 1,
                SupplierName = supplier.Name,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.Setup(s => s.GetSupplierByName(model.SupplierName))
                .ReturnsAsync(supplier);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierName)
                .WithErrorMessage("Supplier name already exists. Enter a different name");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditSupplierName_NoModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.Name,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.Setup(s => s.GetSupplierByName(model.SupplierName))
                .ReturnsAsync(supplier);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierName);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AddSupplierLegalName_SetsModelError(
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierLegalName = "Supplier Legal Name",
            };

            suppliersService.Setup(s => s.GetSupplierByLegalName(model.SupplierLegalName))
                .ReturnsAsync((Supplier)default);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierLegalName);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditDuplicateSupplierLegalName_SetsModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id + 1,
                SupplierLegalName = supplier.LegalName,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.Setup(s => s.GetSupplierByLegalName(model.SupplierLegalName))
                .ReturnsAsync(supplier);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierLegalName)
                .WithErrorMessage("Supplier legal name already exists. Enter a different name");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EditSupplierLegalName_SetsModelError(
            Uri uri,
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierDetailsModelValidator validator)
        {
            var model = new EditSupplierDetailsModel
            {
                SupplierId = supplier.Id,
                SupplierLegalName = supplier.LegalName,
                SupplierWebsite = uri.ToString(),
            };

            suppliersService.Setup(s => s.GetSupplierByLegalName(model.SupplierLegalName))
                .ReturnsAsync(supplier);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierLegalName);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
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
