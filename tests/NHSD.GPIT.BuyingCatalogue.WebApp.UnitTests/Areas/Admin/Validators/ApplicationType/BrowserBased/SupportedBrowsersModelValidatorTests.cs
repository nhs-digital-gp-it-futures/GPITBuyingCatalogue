using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ApplicationType.BrowserBased
{
    public static class SupportedBrowsersModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_MobileResponsiveNullOrEmpty_SetsModelError(
            string pluginsRequired,
            SupportedBrowsersModel model,
            SupportedBrowsersModelValidator validator)
        {
            model.MobileResponsive = pluginsRequired;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MobileResponsive)
                .WithErrorMessage("Select yes if your Catalogue Solution is mobile responsive");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            SupportedBrowsersModel model,
            SupportedBrowsersModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Incomplete_HasError(
            SupportedBrowsersModel model,
            SupportedBrowsersModelValidator validator)
        {
            model.Browsers = new SupportedBrowserModel[0];

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Browsers[0].Checked")
                .WithErrorMessage(SupportedBrowsersModelValidator.MandatoryRequiredMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SupportedBrowsers_Complete(
            SupportedBrowsersModel model,
            SupportedBrowsersModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.Browsers);
        }
    }
}
