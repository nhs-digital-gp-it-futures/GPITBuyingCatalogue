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
    internal static class LoginViewModelTests
    {
        private const string Password = "Password";
        private const string EmailAddress = "test@email.com";
        private const string NotAnEmailAddress = "NotAnEmailAddress";

        [Test]
        [TestCase(null, null, LoginViewModel.ErrorMessages.EmailAddressRequired, LoginViewModel.ErrorMessages.PasswordRequired)]
        [TestCase(Password, null, LoginViewModel.ErrorMessages.EmailAddressRequired)]
        [TestCase(null, EmailAddress, LoginViewModel.ErrorMessages.PasswordRequired)]
        [TestCase(Password, NotAnEmailAddress, LoginViewModel.ErrorMessages.EmailAddressInvalid)]
        [TestCase(Password, EmailAddress)]
        public static void InvalidModel_HasExpectedValidationErrors(string password, string emailAddress, params string[] expectedErrors)
        {
            var errors = new List<ValidationResult>();
            var model = new LoginViewModel { Password = password, EmailAddress = emailAddress };

            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

            isValid.Should().Be(expectedErrors.Length == 0);
            errors.Count.Should().Be(expectedErrors.Length);
            errors.Select(v => v.ErrorMessage).SequenceEqual(expectedErrors).Should().BeTrue();
        }
    }
}
