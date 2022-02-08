﻿using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Validators
{
    public static class RegistrationDetailsModelValidatorTests
    {
        private const string EmailAddress = "a@b.com";

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_AllPropertiesEmpty_ThrowsValidationError(
            string inputValue,
            RegistrationDetailsModel model,
            RegistrationDetailsModelValidator validator)
        {
            model.EmailAddress = inputValue;
            model.FullName = inputValue;
            model.OrganisationName = inputValue;
            model.TelephoneNumber = inputValue;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(RegistrationDetailsModelValidator.EmailAddressMissingErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.FullName)
                .WithErrorMessage(RegistrationDetailsModelValidator.FullNameErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.OrganisationName)
                .WithErrorMessage(RegistrationDetailsModelValidator.OrganisationNameErrorMessage);

            result.ShouldHaveValidationErrorFor(m => m.TelephoneNumber)
                .WithErrorMessage(RegistrationDetailsModelValidator.TelephoneNumberErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_EmailAddressWrongFormat_ThrowsValidationError(
            RegistrationDetailsModel model,
            RegistrationDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EmailAddress)
                .WithErrorMessage(RegistrationDetailsModelValidator.EmailAddressWrongFormatErrorMessage);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
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
