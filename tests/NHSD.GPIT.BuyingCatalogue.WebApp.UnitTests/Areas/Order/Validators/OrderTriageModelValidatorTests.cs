using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class OrderTriageModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectedOption_SetsModelError(
            OrderTriageModel model,
            OrderTriageModelValidator validator)
        {
            model.SelectedTriageOption = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedTriageOption)
                .WithErrorMessage("Select the approximate value of you order, or ‘I’m not sure’ if you do not know");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedOption_NoModelError(
            OrderTriageModel model,
            OrderTriageModelValidator validator)
        {
            model.SelectedTriageOption = TriageOption.Under40k;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
