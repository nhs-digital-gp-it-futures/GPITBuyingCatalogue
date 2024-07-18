using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DescriptionModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SummaryNullOrEmpty_SetsModelError(
            string summary,
            DescriptionModel model,
            DescriptionModelValidator validator)
        {
            model.Summary = summary;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Summary)
                .WithErrorMessage("Enter a summary");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoLink_DoesNotValidate(
            [Frozen] IUrlValidator urlValidator,
            DescriptionModelValidator validator)
        {
            var model = new DescriptionModel();

            var result = validator.TestValidate(model);

            urlValidator.DidNotReceive().IsValidUrl(Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MissingProtocol_SetsModelError(
            DescriptionModel model,
            [Frozen] IUrlValidator urlValidator,
            DescriptionModelValidator validator)
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
            DescriptionModelValidator validator)
        {
            var model = new DescriptionModel { Link = "http://wiothaoih" };

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
            DescriptionModelValidator validator)
        {
            var model = new DescriptionModel { Link = uri.ToString() };
            urlValidator.IsValidUrl(model.Link).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Link);
        }
    }
}
