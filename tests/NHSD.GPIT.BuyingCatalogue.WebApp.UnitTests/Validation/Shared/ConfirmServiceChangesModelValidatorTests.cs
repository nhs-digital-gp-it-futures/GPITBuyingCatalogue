using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public class ConfirmServiceChangesModelValidatorTests
    {
        [Theory]
        [MockAutoData]
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
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
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
