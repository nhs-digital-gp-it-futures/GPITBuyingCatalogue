using System;
using System.Threading.Tasks;
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static async Task Validate_HostingModelNullOrEmpty_SetsModelError(
            string hostingModel,
            HybridModel model,
            HybridModelValidator validator)
        {
            model.HostingModel = hostingModel;

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.HostingModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_NoLink_DoesNotValidate(
            [Frozen] Mock<IUrlValidator> urlValidator,
            HybridModelValidator validator)
        {
            var model = new HybridModel();

            var result = await validator.TestValidateAsync(model);

            urlValidator.Verify(uv => uv.IsValidUrl(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_MissingProtocol_SetsModelError(
            HybridModel model,
            [Frozen] Mock<IUrlValidator> urlValidator,
            HybridModelValidator validator)
        {
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(false);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a prefix to the URL, either http or https");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_InvalidLink_SetsModelError(
            [Frozen] Mock<IUrlValidator> urlValidator,
            HybridModelValidator validator)
        {
            var model = new HybridModel { Link = "http://wiothaoih" };

            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(false);

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.Link)
                .WithErrorMessage("Enter a valid URL");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_ValidLink_NoModelError(
            Uri uri,
            [Frozen] Mock<IUrlValidator> urlValidator,
            HybridModelValidator validator)
        {
            var model = new HybridModel { Link = uri.ToString() };
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(true);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
