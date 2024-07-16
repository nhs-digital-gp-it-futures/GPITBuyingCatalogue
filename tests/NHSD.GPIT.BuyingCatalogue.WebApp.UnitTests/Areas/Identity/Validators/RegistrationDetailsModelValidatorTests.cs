using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Validators
{
    public static class RegistrationDetailsModelValidatorTests
    {
        private const string EmailAddress = "a@b.com";

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_AllPropertiesEmpty_ThrowsValidationError(
            string inputValue,
            RegistrationDetailsModel model,
            RegistrationDetailsModelValidator validator)
        {
            model.EmailAddress = inputValue;
            model.FullName = inputValue;
            model.OrganisationName = inputValue;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(RegistrationDetailsModelValidator.EmailAddressMissingErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.FullName)
                .WithErrorMessage(RegistrationDetailsModelValidator.FullNameErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.OrganisationName)
                .WithErrorMessage(RegistrationDetailsModelValidator.OrganisationNameErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EmailAddressWrongFormat_ThrowsValidationError(
            RegistrationDetailsModel model,
            RegistrationDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(RegistrationDetailsModelValidator.EmailAddressWrongFormatErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PrivacyPolicyNotChecked_ThrowsValidationError(
            RegistrationDetailsModel model,
            RegistrationDetailsModelValidator validator)
        {
            model.HasReadPrivacyPolicy = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.HasReadPrivacyPolicy)
                .WithErrorMessage(RegistrationDetailsModelValidator.PrivacyPolicyErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EverythingOk_NoErrors(
            RegistrationDetailsModel model,
            RegistrationDetailsModelValidator validator)
        {
            model.EmailAddress = EmailAddress;
            model.HasReadPrivacyPolicy = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
