using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            model.SelectedOrderTriageValue = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrderTriageValue)
                .WithErrorMessage("Select the approximate value of your order, or ‘I’m not sure’ if you do not know");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedOption_NoModelError(
            OrderTriageModel model,
            OrderTriageModelValidator validator)
        {
            model.SelectedOrderTriageValue = OrderTriageValue.Under40K;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
