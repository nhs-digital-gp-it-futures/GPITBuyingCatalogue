using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class LoginViewModelTests
    {
        private const string Password = "Password";
        private const string EmailAddress = "test@email.com";
        private const string NotAnEmailAddress = "NotAnEmailAddress";

        [Theory]
        [InlineData(null, null, LoginViewModel.ErrorMessages.EmailAddressRequired, LoginViewModel.ErrorMessages.PasswordRequired)]
        [InlineData(Password, null, LoginViewModel.ErrorMessages.EmailAddressRequired)]
        [InlineData(null, EmailAddress, LoginViewModel.ErrorMessages.PasswordRequired)]
        [InlineData(Password, NotAnEmailAddress, LoginViewModel.ErrorMessages.EmailAddressInvalid)]
        [InlineData(Password, EmailAddress)]
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
