using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class BrowserBasedModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_Incomplete_HasError(
            BrowserBasedModel model,
            BrowserBasedModelValidator validator)
        {
            model.ApplicationTypeDetail.Plugins = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApplicationTypeProgress.SupportedBrowsersStatus() == TaskProgress.Completed && m.ApplicationTypeProgress.PluginsStatus() == TaskProgress.Completed)
                .WithErrorMessage(BrowserBasedModelValidator.MandatoryRequiredMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_BrowserBased_Complete(
            BrowserBasedModel model,
            BrowserBasedModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.ApplicationTypeProgress.SupportedBrowsersStatus() == TaskProgress.Completed && m.ApplicationTypeProgress.PluginsStatus() == TaskProgress.Completed);
        }
    }
}
