using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class RemoveSublocationsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_SelectionMade_NoValidationErrors(
            RemoveSublocationsModel model,
            RemoveSublocationsModelValidator systemUnderTest)
        {
            model.ConfirmRemove = true;

            TestValidationResult<RemoveSublocationsModel> result = systemUnderTest.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_NoSelectionMade_ThrowsValidationError(
            RemoveSublocationsModel model,
            RemoveSublocationsModelValidator systemUnderTest)
        {
            model.ConfirmRemove = null;

            TestValidationResult<RemoveSublocationsModel> result = systemUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor("ConfirmRemove")
                .WithErrorMessage(RemoveSublocationsModelValidator.GetNoSelectionMadeErrorMessage(model.Pluralisation));
        }
    }
}
