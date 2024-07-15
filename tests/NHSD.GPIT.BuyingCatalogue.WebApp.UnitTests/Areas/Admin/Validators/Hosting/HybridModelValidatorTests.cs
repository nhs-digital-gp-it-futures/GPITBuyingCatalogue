using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Hosting
{
    public sealed class HybridModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_HostingModelNullOrEmpty_SetsModelError(
            string hostingModel,
            HybridModel model,
            HybridModelValidator validator)
        {
            model.HostingModel = hostingModel;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HostingModel);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoLink_DoesNotValidate(
            [Frozen] IUrlValidator urlValidator,
            HybridModelValidator validator)
        {
            var model = new HybridModel();

            var result = validator.TestValidate(model);

            urlValidator.DidNotReceive().IsValidUrl(Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            HybridModel model,
            [Frozen] IUrlValidator urlValidator,
            HybridModelValidator validator)
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
            HybridModelValidator validator)
        {
            var model = new HybridModel { Link = "http://wiothaoih" };

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
            HybridModelValidator validator)
        {
            var model = new HybridModel { Link = uri.ToString() };
            urlValidator.IsValidUrl(model.Link).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
