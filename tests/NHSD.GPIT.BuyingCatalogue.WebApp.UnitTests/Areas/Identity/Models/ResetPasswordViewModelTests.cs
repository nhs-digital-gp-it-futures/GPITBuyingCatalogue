using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class ResetPasswordViewModelTests
    {
        private const string Password = "Password";
        private const string ConfirmPassword = "Password";
        private const string WrongPassword = "Password123";

        [Theory]
        [InlineData(null, null, ResetPasswordViewModel.ErrorMessages.PasswordRequired)]
        [InlineData(null, ConfirmPassword, ResetPasswordViewModel.ErrorMessages.PasswordRequired, ResetPasswordViewModel.ErrorMessages.PasswordMismatch)]
        [InlineData(Password, null, ResetPasswordViewModel.ErrorMessages.PasswordMismatch)]
        [InlineData(Password, WrongPassword, ResetPasswordViewModel.ErrorMessages.PasswordMismatch)]
        [InlineData(Password, ConfirmPassword)]
        public static void InvalidModel_HasExpectedValidationErrors(string password, string confimPassword, params string[] expectedErrors)
        {
            var errors = new List<ValidationResult>();
            var model = new ResetPasswordViewModel { Password = password, ConfirmPassword = confimPassword };

            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

            isValid.Should().Be(expectedErrors.Length == 0);
            errors.Count.Should().Be(expectedErrors.Length);
            errors.Select(v => v.ErrorMessage).SequenceEqual(expectedErrors).Should().BeTrue();
        }
    }
}
