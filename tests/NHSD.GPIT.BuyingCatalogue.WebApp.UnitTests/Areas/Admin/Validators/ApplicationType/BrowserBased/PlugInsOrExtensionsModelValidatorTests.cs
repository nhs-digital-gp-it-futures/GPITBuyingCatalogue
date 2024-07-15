using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.BrowserBased
{
    public static class PlugInsOrExtensionsModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static void Validate_PluginsRequiredNullOrEmpty_SetsModelError(
            string pluginsRequired,
            PlugInsOrExtensionsModel model,
            PlugInsOrExtensionsModelValidator validator)
        {
            model.PlugInsRequired = pluginsRequired;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PlugInsRequired)
                .WithErrorMessage("Select yes if any plug-ins or extensions are required");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            PlugInsOrExtensionsModel model,
            PlugInsOrExtensionsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
