using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation
{
    public static class DeleteOrderModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedOptionsNull_SetsModelError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.SelectedOption = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOption)
                .WithErrorMessage(DeleteOrderModelValidator.SelectOptionErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
