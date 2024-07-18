using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.FundingSources
{
    public static class FundingSourceModelValidatorTests
    {
        [Theory]
        [MockAutoData]
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
