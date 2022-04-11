using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class AddTieredPriceTierModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_MissingPrice_SetsModelError(
            AddTieredPriceTierModel model,
            AddTieredPriceTierModelValidator validator)
        {
            model.Price = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(AddTieredPriceTierModelValidator.PriceEmptyError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NegativePrice_SetsModelError(
            AddTieredPriceTierModel model,
            AddTieredPriceTierModelValidator validator)
        {
            model.Price = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(AddTieredPriceTierModelValidator.PriceNegativeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceGreaterThan4DecimalPlaces_SetsModelError(
                AddTieredPriceTierModel model,
                AddTieredPriceTierModelValidator validator)
        {
            model.Price = 1.23456M;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(AddTieredPriceTierModelValidator.PriceGreaterThanDecimalPlacesError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MissingLowerRange_SetsModelError(
            AddTieredPriceTierModel model,
            AddTieredPriceTierModelValidator validator)
        {
            model.LowerRange = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LowerRange)
                .WithErrorMessage(AddTieredPriceTierModelValidator.LowerRangeMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UpperRangeMissing_SetsModelError(
            AddTieredPriceTierModel model,
            AddTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = false;
            model.UpperRange = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.UpperRange)
                .WithErrorMessage(AddTieredPriceTierModelValidator.UpperRangeMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UpperRangeMissing_InfiniteRange_NoModelError(
            AddTieredPriceTierModel model,
            AddTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = true;
            model.UpperRange = null;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.UpperRange);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_RangeTypeMissing_SetsModelError(
            AddTieredPriceTierModel model,
            AddTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsInfiniteRange)
                .WithErrorMessage(AddTieredPriceTierModelValidator.RangeTypeMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_InfiniteRange_SetsModelError(
            AddTieredPriceTierModel model,
            [Frozen] Mock<IDuplicateListPriceService> service,
            AddTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = true;
            model.Price = 1.23M;
            model.LowerRange = 1;
            model.UpperRange = null;

            service.Setup(s => s.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.Price!.Value,
                model.LowerRange!.Value,
                model.UpperRange)).ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Price|LowerRange|IsInfiniteRange")
                .WithErrorMessage(AddTieredPriceTierModelValidator.DuplicateListPriceTierError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_UpperRange_SetsModelError(
            AddTieredPriceTierModel model,
            [Frozen] Mock<IDuplicateListPriceService> service,
            AddTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = false;
            model.Price = 1.23M;
            model.LowerRange = 1;
            model.UpperRange = 9;

            service.Setup(s => s.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.Price!.Value,
                model.LowerRange!.Value,
                model.UpperRange)).ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Price|LowerRange|UpperRange|IsInfiniteRange")
                .WithErrorMessage(AddTieredPriceTierModelValidator.DuplicateListPriceTierError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            AddTieredPriceTierModel model,
            [Frozen] Mock<IDuplicateListPriceService> service,
            AddTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = false;
            model.Price = 1.23M;
            model.LowerRange = 1;
            model.UpperRange = 9;

            service.Setup(s => s.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.Price!.Value,
                model.LowerRange!.Value,
                model.UpperRange)).ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
