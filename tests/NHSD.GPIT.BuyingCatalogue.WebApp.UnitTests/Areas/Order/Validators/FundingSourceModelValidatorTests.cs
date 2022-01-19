using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class FundingSourceModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_FundingSourceOnlyGmsNullOrEmpty_SetsModelError(
            string fundingSourceOnlyGms,
            FundingSourceModel model,
            FundingSourceModelValidator validator)
        {
            model.FundingSourceOnlyGms = fundingSourceOnlyGms;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FundingSourceOnlyGms)
                .WithErrorMessage("Select yes if you're paying for this order in full using your GP IT Futures centrally held funding allocation");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            FundingSourceModel model,
            FundingSourceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
