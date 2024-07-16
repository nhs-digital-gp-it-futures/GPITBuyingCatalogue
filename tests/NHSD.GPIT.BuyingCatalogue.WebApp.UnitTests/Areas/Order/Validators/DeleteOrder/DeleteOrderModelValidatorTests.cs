using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.DeleteOrder
{
    public static class DeleteOrderModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_Amendment_SelectedOptionsNull_SetsModelError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.IsAmendment = true;
            model.SelectedOption = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOption)
                .WithErrorMessage(DeleteOrderModelValidator.AmendmentNoSelectionMadeErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Order_SelectedOptionsNull_SetsModelError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.IsAmendment = false;
            model.SelectedOption = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOption)
                .WithErrorMessage(DeleteOrderModelValidator.OrderNoSelectionMadeErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
