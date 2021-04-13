using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ResetPasswordViewModelTests
    {
        private const string Password = "Password";
        private const string ConfirmPassword = "Password";
        private const string WrongPassword = "Password123";

        [Test]
        [TestCase(null, null, ResetPasswordViewModel.ErrorMessages.PasswordRequired)]
        [TestCase(null, ConfirmPassword, ResetPasswordViewModel.ErrorMessages.PasswordRequired, ResetPasswordViewModel.ErrorMessages.PasswordMismatch)]
        [TestCase(Password, null, ResetPasswordViewModel.ErrorMessages.PasswordMismatch)]
        [TestCase(Password, WrongPassword, ResetPasswordViewModel.ErrorMessages.PasswordMismatch)]
        [TestCase(Password, ConfirmPassword)]
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
