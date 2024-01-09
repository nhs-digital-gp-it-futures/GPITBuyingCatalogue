using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation
{
    public static class UploadOrSelectServiceRecipientModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NoSelectedOrderItemType_SetsModelError(
            UploadOrSelectServiceRecipientModel model,
            UploadOrSelectServiceRecipientModelValidator validator)
        {
            model.ShouldUploadRecipients = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ShouldUploadRecipients)
                .WithErrorMessage(UploadOrSelectServiceRecipientModelValidator.SelectedServiceRecipientOptionsError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedOrderItemType_NoModelError(
            UploadOrSelectServiceRecipientModel model,
            UploadOrSelectServiceRecipientModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
