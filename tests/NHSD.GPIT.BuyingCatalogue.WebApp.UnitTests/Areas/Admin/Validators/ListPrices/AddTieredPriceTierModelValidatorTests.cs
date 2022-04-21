using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class AddTieredPriceTierModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_MissingPrice_SetsModelError(
            AddEditTieredPriceTierModel model,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.Price = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceEmptyError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_NegativePrice_SetsModelError(
            AddEditTieredPriceTierModel model,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.Price = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceNegativeError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceGreaterThan4DecimalPlaces_SetsModelError(
                AddEditTieredPriceTierModel model,
                AddEditTieredPriceTierModelValidator validator)
        {
            model.Price = 1.23456M;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Price)
                .WithErrorMessage(FluentValidationExtensions.PriceGreaterThanDecimalPlacesError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MissingLowerRange_SetsModelError(
            AddEditTieredPriceTierModel model,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.LowerRange = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.LowerRange)
                .WithErrorMessage(AddEditTieredPriceTierModelValidator.LowerRangeMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UpperRangeMissing_SetsModelError(
            AddEditTieredPriceTierModel model,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = false;
            model.UpperRange = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.UpperRange)
                .WithErrorMessage(AddEditTieredPriceTierModelValidator.UpperRangeMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_UpperRangeMissing_InfiniteRange_NoModelError(
            AddEditTieredPriceTierModel model,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = true;
            model.UpperRange = null;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.UpperRange);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_RangeTypeMissing_SetsModelError(
            AddEditTieredPriceTierModel model,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.IsInfiniteRange)
                .WithErrorMessage(AddEditTieredPriceTierModelValidator.RangeTypeMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_InfiniteRange_SetsModelError(
            AddEditTieredPriceTierModel model,
            [Frozen] Mock<IListPriceService> service,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = true;
            model.Price = 1.23M;
            model.LowerRange = 1;
            model.UpperRange = null;

            service.Setup(s => s.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.TierId,
                model.LowerRange!.Value,
                model.UpperRange)).ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Price|LowerRange|IsInfiniteRange")
                .WithErrorMessage(AddEditTieredPriceTierModelValidator.DuplicateListPriceTierError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Duplicate_UpperRange_SetsModelError(
            AddEditTieredPriceTierModel model,
            [Frozen] Mock<IListPriceService> service,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = false;
            model.Price = 1.23M;
            model.LowerRange = 1;
            model.UpperRange = 9;

            service.Setup(s => s.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.TierId,
                model.LowerRange!.Value,
                model.UpperRange)).ReturnsAsync(true);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Price|LowerRange|UpperRange|IsInfiniteRange")
                .WithErrorMessage(AddEditTieredPriceTierModelValidator.DuplicateListPriceTierError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            AddEditTieredPriceTierModel model,
            [Frozen] Mock<IListPriceService> service,
            AddEditTieredPriceTierModelValidator validator)
        {
            model.IsInfiniteRange = false;
            model.Price = 1.23M;
            model.LowerRange = 1;
            model.UpperRange = 9;

            service.Setup(s => s.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.TierId,
                model.LowerRange!.Value,
                model.UpperRange)).ReturnsAsync(false);

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
