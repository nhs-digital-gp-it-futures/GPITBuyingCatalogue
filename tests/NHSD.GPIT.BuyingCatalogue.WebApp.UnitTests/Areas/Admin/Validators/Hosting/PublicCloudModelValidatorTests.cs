using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Hosting
{
    public sealed class PublicCloudModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoLink_DoesNotValidate(
            [Frozen] IUrlValidator urlValidator,
            PublicCloudModelValidator validator)
        {
            var model = new PublicCloudModel();

            var result = validator.TestValidate(model);

            urlValidator.DidNotReceive().IsValidUrl(Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            PublicCloudModel model,
            [Frozen] IUrlValidator urlValidator,
            PublicCloudModelValidator validator)
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
            PublicCloudModelValidator validator)
        {
            var model = new PublicCloudModel { Link = "http://wiothaoih" };

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
            PublicCloudModelValidator validator)
        {
            var model = new PublicCloudModel { Link = uri.ToString() };
            urlValidator.IsValidUrl(model.Link).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
