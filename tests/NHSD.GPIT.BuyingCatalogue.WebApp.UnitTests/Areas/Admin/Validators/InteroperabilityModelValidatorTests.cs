using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class InteroperabilityModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoLink_DoesNotValidate(
            [Frozen] IUrlValidator urlValidator,
            InteroperabilityModelValidator validator)
        {
            var model = new InteroperabilityModel();

            var result = validator.TestValidate(model);

            urlValidator.DidNotReceive().IsValidUrl(Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            InteroperabilityModel model,
            [Frozen] IUrlValidator urlValidator,
            InteroperabilityModelValidator validator)
        {
            urlValidator.IsValidUrl(model.Link).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a prefix to the URL, either http or https");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_InvalidLink_SetsModelError(
            [Frozen] IUrlValidator urlValidator,
            InteroperabilityModelValidator validator)
        {
            var model = new InteroperabilityModel { Link = "http://wiothaoih" };

            urlValidator.IsValidUrl(model.Link).Returns(false);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a valid URL");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidLink_NoModelError(
            Uri uri,
            [Frozen] IUrlValidator urlValidator,
            InteroperabilityModelValidator validator)
        {
            var model = new InteroperabilityModel { Link = uri.ToString() };
            urlValidator.IsValidUrl(model.Link).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
