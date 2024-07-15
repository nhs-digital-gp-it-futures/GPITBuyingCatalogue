using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.DesktopBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.DesktopBased
{
    public static class ConnectivityModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_SelectedConnectionSpeedNullOrEmpty_SetsModelError(
            string selectedConnectionSpeed,
            ConnectivityModel model,
            ConnectivityModelValidator validator)
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
            ConnectivityModel model,
            ConnectivityModelValidator validator)
        {
            model.SelectedConnectionSpeed = selectedConnectionSpeed;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
