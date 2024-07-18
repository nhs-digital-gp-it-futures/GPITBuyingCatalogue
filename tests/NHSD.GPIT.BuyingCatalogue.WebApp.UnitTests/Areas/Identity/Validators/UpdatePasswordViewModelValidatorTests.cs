using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Validators
{
    public static class UpdatePasswordViewModelValidatorTests
    {
        private const string Password = "Password";

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_AllPropertiesEmpty_ThrowsValidationError(
            string inputValue,
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.CurrentPassword = inputValue;
            model.NewPassword = inputValue;
            model.ConfirmPassword = inputValue;
            model.IdentityResult = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.CurrentPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.CurrentPasswordRequired);

            result.ShouldHaveValidationErrorFor(m => m.NewPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.NewPasswordRequired);

            result.ShouldHaveValidationErrorFor(m => m.ConfirmPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.ConfirmPasswordRequired);
        }

        [Theory]
        [MockInlineAutoData("password1", "password2")]
        [MockInlineAutoData("password1", " password1")]
        [MockInlineAutoData("password1", "password1 ")]
        public static void Validate_NewPasswordAndConfirmNotMatched_ThrowsValidationError(
            string newPassword,
            string confirmPassword,
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.NewPassword = newPassword;
            model.ConfirmPassword = confirmPassword;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ConfirmPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.ConfirmPasswordMismatch);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EverythingOk_NoErrors(
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.CurrentPassword = Password;
            model.NewPassword = Password;
            model.ConfirmPassword = Password;
            model.IdentityResult = null;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_EverythingOk_With_IdentityResult_Success_NoErrors(
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.CurrentPassword = Password;
            model.NewPassword = Password;
            model.ConfirmPassword = Password;
            model.IdentityResult = IdentityResult.Success;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PreviouslyUsedPassword_ValidationError(
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.IdentityResult = IdentityResult.Failed(
            [
                new IdentityError()
                {
                    Code = PasswordValidator.PasswordAlreadyUsedCode,
                },
            ]);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.NewPassword)
                .WithErrorMessage(PasswordValidator.PasswordAlreadyUsed);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_PasswordMismatch_ValidationError(
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.IdentityResult = IdentityResult.Failed(
            [
                new IdentityError()
                {
                    Code = PasswordValidator.PasswordMismatchCode,
                },
            ]);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.CurrentPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.CurrentPasswordIncorrect);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_InvalidPassword_ValidationError(
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.IdentityResult = IdentityResult.Failed(
            [
                new IdentityError()
                {
                    Code = PasswordValidator.InvalidPasswordCode,
                },
            ]);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.NewPassword)
                .WithErrorMessage(PasswordValidator.PasswordConditionsNotMet);
        }
    }
}
