using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Solutions
{
    public static class SelectFlatDeclarativeQuantityModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NullQuantity_ThrowsValidationError(
            SelectFlatDeclarativeQuantityModel model,
            SelectFlatDeclarativeQuantityModelValidator validator)
        {
            model.Quantity = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Enter a quantity");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AlphanumericalQuantity_ThrowsValidationError(
            SelectFlatDeclarativeQuantityModel model,
            SelectFlatDeclarativeQuantityModelValidator validator)
        {
            model.Quantity = "abc";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Quantity must be a number");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_QuantityLessThan1_ThrowsValidationError(
            SelectFlatDeclarativeQuantityModel model,
            SelectFlatDeclarativeQuantityModelValidator validator)
        {
            model.Quantity = "0";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Quantity must be greater than zero");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidQuantity_NoValidationErrors(
            SelectFlatDeclarativeQuantityModel model,
            SelectFlatDeclarativeQuantityModelValidator validator)
        {
            model.Quantity = "2";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
