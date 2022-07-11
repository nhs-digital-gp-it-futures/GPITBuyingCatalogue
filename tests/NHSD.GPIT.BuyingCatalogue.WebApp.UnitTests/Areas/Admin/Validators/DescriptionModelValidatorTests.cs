using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DescriptionModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static async Task Validate_SummaryNullOrEmpty_SetsModelError(
            string summary,
            DescriptionModel model,
            DescriptionModelValidator validator)
        {
            model.Summary = summary;

            var result = await validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(m => m.Summary)
                .WithErrorMessage("Enter a summary");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_NoLink_DoesNotValidate(
            [Frozen] Mock<IUrlValidator> urlValidator,
            DescriptionModelValidator validator)
        {
            var model = new DescriptionModel();

            var result = await validator.TestValidateAsync(model);

            urlValidator.Verify(uv => uv.IsValidUrl(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Validate_MissingProtocol_SetsModelError(
            DescriptionModel model,
            [Frozen] Mock<IUrlValidator> urlValidator,
            DescriptionModelValidator validator)
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
            DescriptionModelValidator validator)
        {
            var model = new DescriptionModel { Link = "http://wiothaoih" };

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
            DescriptionModelValidator validator)
        {
            var model = new DescriptionModel { Link = uri.ToString() };
            urlValidator.Setup(uv => uv.IsValidUrl(model.Link))
                .Returns(true);

            var result = await validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
