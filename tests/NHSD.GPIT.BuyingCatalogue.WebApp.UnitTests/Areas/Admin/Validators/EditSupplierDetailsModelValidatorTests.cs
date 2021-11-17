using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
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
                .ReturnsAsync(false);

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
                .ReturnsAsync(false);

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
                .ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.SupplierWebsite);
        }
    }
}
