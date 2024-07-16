using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.BrowserBased
{
    public static class ConnectivityAndResolutionModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SelectedConnectionSpeedNullOrEmpty_SetsModelError(
            string selectedConnectionSpeed,
            ConnectivityAndResolutionModel model,
            ConnectivityAndResolutionModelValidator validator)
        {
            model.SelectedConnectionSpeed = selectedConnectionSpeed;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedConnectionSpeed)
                .WithErrorMessage("Select a connection speed");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedConnectionSpeedValid_NoModelError(
            string selectedConnectionSpeed,
            ConnectivityAndResolutionModel model,
            ConnectivityAndResolutionModelValidator validator)
        {
            model.SelectedConnectionSpeed = selectedConnectionSpeed;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
