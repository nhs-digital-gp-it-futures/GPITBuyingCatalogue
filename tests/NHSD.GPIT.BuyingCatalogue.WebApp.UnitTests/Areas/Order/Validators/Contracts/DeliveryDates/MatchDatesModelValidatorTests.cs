using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates
{
    public static class MatchDatesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_MatchDatesIsNull_ThrowsValidationError(
            MatchDatesModel model,
            MatchDatesModelValidator validator)
        {
            model.MatchDates = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.MatchDates)
                .WithErrorMessage(MatchDatesModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Validate_MatchDatesIsNotNull_NoErrors(
            bool matchDates,
            MatchDatesModel model,
            MatchDatesModelValidator validator)
        {
            model.MatchDates = matchDates;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
