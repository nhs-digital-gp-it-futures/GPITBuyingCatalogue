using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class SelectFrameworkModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectedFramework_SetsModelError(
            SelectFrameworkModel model,
            SelectFrameworkModelValidator validator)
        {
            model.SelectedFrameworkId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedFrameworkId)
                .WithErrorMessage(SelectFrameworkModelValidator.FrameworkRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedFramework_NoModelError(
            SelectFrameworkModel model,
            SelectFrameworkModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
