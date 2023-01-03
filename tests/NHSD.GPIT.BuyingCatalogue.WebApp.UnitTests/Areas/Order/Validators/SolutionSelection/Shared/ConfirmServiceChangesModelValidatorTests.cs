using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Shared
{
    public class ConfirmServiceChangesModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_ConfirmChangesNull_ThrowsValidationError(
            ConfirmServiceChangesModel model,
            ConfirmServiceChangesModelValidator systemUnderTest)
        {
            model.ConfirmChanges = null;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmChanges)
                .WithErrorMessage(ConfirmServiceChangesModelValidator.ErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(true)]
        [CommonInlineAutoData(false)]
        public static void Validate_ConfirmChangesNotNull_NoValidationErrors(
            bool confirmChanges,
            ConfirmServiceChangesModel model,
            ConfirmServiceChangesModelValidator systemUnderTest)
        {
            model.ConfirmChanges = confirmChanges;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
