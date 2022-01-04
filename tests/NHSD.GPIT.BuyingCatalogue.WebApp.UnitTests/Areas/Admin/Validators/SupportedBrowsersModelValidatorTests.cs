using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class SupportedBrowsersModelValidatorTests
    {
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
