using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Prices
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
    }
}
