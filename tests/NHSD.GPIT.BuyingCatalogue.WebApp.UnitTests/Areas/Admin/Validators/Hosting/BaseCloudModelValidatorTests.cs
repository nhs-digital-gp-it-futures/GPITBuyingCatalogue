using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Hosting;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Hosting
{
    public static class BaseCloudModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SummaryNullOrEmpty_SetsModelError(
            string summary,
            BaseCloudModel model,
            BaseCloudModelValidator validator)
        {
            model.Summary = summary;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Summary)
                .WithErrorMessage("Enter a summary");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            string summary,
            [Frozen] IUrlValidator urlValidator,
            BaseCloudModel model,
            BaseCloudModelValidator validator)
        {
            model.Summary = summary;

            urlValidator.IsValidUrl(Arg.Any<string>()).Returns(true);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
