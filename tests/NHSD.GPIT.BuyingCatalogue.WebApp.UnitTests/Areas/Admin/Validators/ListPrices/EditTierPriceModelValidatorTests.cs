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
        [CommonAutoData]
        public static void Validate_MissingPrice_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.Price = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceEmptyError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NegativePrice_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.Price = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceNegativeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceGreaterThan4DecimalPlaces_SetsModelError(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.Price = 1.23456M;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceGreaterThanDecimalPlacesError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidPrice_NoModelErrors(
            EditTierPriceModel model,
            EditTierPriceModelValidator validator)
        {
            model.Price = 3.14M;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
