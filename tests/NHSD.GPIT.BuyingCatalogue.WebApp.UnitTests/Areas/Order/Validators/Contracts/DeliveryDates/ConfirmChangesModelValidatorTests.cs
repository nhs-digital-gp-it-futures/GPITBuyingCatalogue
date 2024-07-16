using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates
{
    public static class ConfirmChangesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ConfirmChangesIsNull_ThrowsValidationError(
            ConfirmChangesModel model,
            ConfirmChangesModelValidator validator)
        {
            model.ConfirmChanges = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmChanges)
                .WithErrorMessage(ConfirmChangesModelValidator.NoSelectionMadeErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static void Validate_ConfirmChangesIsNotNull_NoErrors(
            bool confirmChanges,
            ConfirmChangesModel model,
            ConfirmChangesModelValidator validator)
        {
            model.ConfirmChanges = confirmChanges;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
