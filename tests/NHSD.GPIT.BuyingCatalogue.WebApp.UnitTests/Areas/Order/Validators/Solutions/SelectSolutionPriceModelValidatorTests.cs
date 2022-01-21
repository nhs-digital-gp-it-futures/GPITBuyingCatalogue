using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Solutions
{
    public static class SelectSolutionPriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedPriceNull_SetsModelError(
            SelectSolutionPriceModel model,
            SelectSolutionPriceModelValidator validator)
        {
            model.SelectedPrice = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPrice)
                .WithErrorMessage("Select a price");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            SelectSolutionPriceModel model,
            SelectSolutionPriceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
