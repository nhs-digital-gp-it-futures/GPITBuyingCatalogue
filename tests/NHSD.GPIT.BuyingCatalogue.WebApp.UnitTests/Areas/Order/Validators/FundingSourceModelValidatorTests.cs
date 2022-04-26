using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSource;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class FundingSourceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_FundingSourceOnlyGmsNullOrEmpty_SetsModelError(
            FundingSourceModel model,
            FundingSourceModelValidator validator)
        {
            model.SelectedFundingSource = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedFundingSource)
                .WithErrorMessage("Select a funding source");
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
