using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class DetermineAssociatedServiceTypeModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoOrderType_SetsModelError(
            DetermineAssociatedServiceTypeModel model,
            DetermineAssociatedServiceTypeModelValidator validator)
        {
            model.OrderType = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderType)
                .WithErrorMessage(DetermineAssociatedServiceTypeModelValidator.OrderTypeRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_OrderType_NoModelError(
            DetermineAssociatedServiceTypeModel model,
            DetermineAssociatedServiceTypeModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
