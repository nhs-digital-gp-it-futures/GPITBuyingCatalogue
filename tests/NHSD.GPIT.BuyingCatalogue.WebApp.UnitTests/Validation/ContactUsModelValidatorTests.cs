using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation
{
    public static class ContactUsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_PropertiesEmpty_ThrowsValidationError(
            ContactUsModel model,
            ContactUsModelValidator validator)
        {
            model.FullName = null;
            model.Message = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.FullName)
                .WithErrorMessage(ContactUsModelValidator.FullNameMissingErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.Message)
                .WithErrorMessage(ContactUsModelValidator.MessageMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null, ContactUsModelValidator.EmailAddressMissingErrorMessage)]
        [CommonInlineAutoData("", ContactUsModelValidator.EmailAddressMissingErrorMessage)]
        [CommonInlineAutoData("abc", ContactUsModelValidator.EmailAddressWrongFormatErrorMessage)]
        public static void Validate_InvalidEmail_ThrowsValidationError(
            string emailAddress,
            string expectedErrorMessage,
            ContactUsModel model,
            ContactUsModelValidator validator)
        {
            model.EmailAddress = emailAddress;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(expectedErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PrivacyPolicyNotAccepted_ThrowsValidationError(
            ContactUsModel model,
            ContactUsModelValidator validator)
        {
            model.PrivacyPolicyAccepted = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PrivacyPolicyAccepted)
                .WithErrorMessage(ContactUsModelValidator.PrivacyPolicyNotAcceptedErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            string fullName,
            string message,
            ContactUsModel model,
            ContactUsModelValidator validator)
        {
            model.FullName = fullName;
            model.Message = message;
            model.PrivacyPolicyAccepted = true;
            model.EmailAddress = "a@a.com";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
