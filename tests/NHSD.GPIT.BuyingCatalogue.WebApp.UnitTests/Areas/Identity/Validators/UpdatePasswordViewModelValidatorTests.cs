using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Validators
{
    public static class UpdatePasswordViewModelValidatorTests
    {
        private const string Password = "Password";

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_AllPropertiesEmpty_ThrowsValidationError(
            string inputValue,
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.CurrentPassword = inputValue;
            model.NewPassword = inputValue;
            model.ConfirmPassword = inputValue;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.CurrentPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.CurrentPasswordRequired);

            result.ShouldHaveValidationErrorFor(m => m.NewPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.NewPasswordRequired);

            result.ShouldHaveValidationErrorFor(m => m.ConfirmPassword)
                .WithErrorMessage(UpdatePasswordViewModelValidator.ConfirmPasswordRequired);
        }

        [Theory]
        [CommonInlineAutoData("password1", "password2")]
        [CommonInlineAutoData("password1", " password1")]
        [CommonInlineAutoData("password1", "password1 ")]
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
        [CommonAutoData]
        public static void Validate_EverythingOk_NoErrors(
            UpdatePasswordViewModel model,
            UpdatePasswordViewModelValidator validator)
        {
            model.CurrentPassword = Password;
            model.NewPassword = Password;
            model.ConfirmPassword = Password;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
