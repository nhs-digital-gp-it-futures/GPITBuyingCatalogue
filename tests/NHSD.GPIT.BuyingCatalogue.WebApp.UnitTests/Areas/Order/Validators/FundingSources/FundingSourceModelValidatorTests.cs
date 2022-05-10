using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.FundingSources
{
    public static class FundingSourceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedFundingTypeNotSelected_SetsModelError(
            FundingSource model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingType = EntityFramework.Ordering.Models.OrderItemFundingType.None;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedFundingType)
                .WithErrorMessage(FundingSourceModelValidator.FundingSourceMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MixedFunding_CentralAllocationEmpty_SetsModelError(
            FundingSource model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingType = EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding;
            model.AmountOfCentralFunding = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AmountOfCentralFunding)
                .WithErrorMessage(FundingSourceModelValidator.FundingSourceCentrallyAllocatedAmountMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MixedFunding_CentralAllocationLessThanZero_SetsModelError(
            FundingSource model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingType = EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding;
            model.AmountOfCentralFunding = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AmountOfCentralFunding)
                .WithErrorMessage(FundingSourceModelValidator.FundingSourceAmountMustBeGreaterThanZeroErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MixedFunding_CentralAllocationMoreThan4Places_SetsModelError(
            FundingSource model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingType = EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding;
            model.AmountOfCentralFunding = 1.0000000000M;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AmountOfCentralFunding)
                .WithErrorMessage(FundingSourceModelValidator.FundingSourceAmountTo4DeciamlPlacesErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MixedFunding_CentralAllocationGreaterThanTotalCost_SetsModelError(
            FundingSource model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingType = EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding;
            model.AmountOfCentralFunding = model.TotalCost + 1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AmountOfCentralFunding)
                .WithErrorMessage(FundingSourceModelValidator.FundingSourceAmountCannotExceedeTotalCost);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MixedFunding_CentralAllocationIgnoredIfNotMixed_SetsModelError(
            FundingSource model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingType = EntityFramework.Ordering.Models.OrderItemFundingType.CentralFunding;
            model.AmountOfCentralFunding = null;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.AmountOfCentralFunding);
        }
    }
}
