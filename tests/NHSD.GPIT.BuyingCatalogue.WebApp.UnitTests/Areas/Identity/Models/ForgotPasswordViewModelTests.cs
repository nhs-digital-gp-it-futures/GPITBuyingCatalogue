using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class ForgotPasswordViewModelTests
    {
        private const string EmailAddress = "test@email.com";
        private const string NotAnEmailAddress = "NotAnEmailAddress";

        [Theory]
        [InlineData(null, ForgotPasswordViewModel.ErrorMessages.EmailAddressRequired)]
        [InlineData(NotAnEmailAddress, ForgotPasswordViewModel.ErrorMessages.EmailAddressInvalid)]
        [InlineData(EmailAddress)]
        public static void InvalidModel_HasExpectedValidationErrors(string emailAddress, params string[] expectedErrors)
        {
            var errors = new List<ValidationResult>();
            var model = new ForgotPasswordViewModel { EmailAddress = emailAddress };

            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

            isValid.Should().Be(expectedErrors.Length == 0);
            errors.Count.Should().Be(expectedErrors.Length);
            errors.Select(v => v.ErrorMessage).SequenceEqual(expectedErrors).Should().BeTrue();
        }
    }
}
