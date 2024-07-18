using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class OrderItemTypeModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectedOrderItemType_SetsModelError(
            OrderItemTypeModel model,
            OrderItemTypeModelValidator validator)
        {
            model.SelectedOrderItemType = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrderItemType)
                .WithErrorMessage(OrderItemTypeModelValidator.SelectedOrderItemTypeError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedOrderItemType_NoModelError(
            OrderItemTypeModel model,
            OrderItemTypeModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
