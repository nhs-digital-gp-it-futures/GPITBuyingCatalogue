using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Pricing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Pricing
{
    public static class SelectPriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            SelectPriceModel model,
            SelectPriceModelValidator validator)
        {
            model.SelectedPriceId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(nameof(model.SelectedPriceId))
                .WithErrorMessage(SelectPriceModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AllCorrect_NoErrorThrown(
            SelectPriceModel model,
            SelectPriceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
