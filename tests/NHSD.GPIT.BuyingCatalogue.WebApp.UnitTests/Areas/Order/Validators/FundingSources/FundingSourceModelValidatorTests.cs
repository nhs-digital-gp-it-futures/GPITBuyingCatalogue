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
    }
}
