using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class EditTierPriceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_MissingPrice_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.InputPrice = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceEmptyError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NegativePrice_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.InputPrice = "-1";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceNegativeError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PriceGreaterThan4DecimalPlaces_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.InputPrice = "1.23456";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceGreaterThanDecimalPlacesError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PriceNotNumeric_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.InputPrice = "abc";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InputPrice)
                .WithErrorMessage(FluentValidationExtensions.PriceNotANumberError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidPrice_NoModelErrors(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.InputPrice = "3.14";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
