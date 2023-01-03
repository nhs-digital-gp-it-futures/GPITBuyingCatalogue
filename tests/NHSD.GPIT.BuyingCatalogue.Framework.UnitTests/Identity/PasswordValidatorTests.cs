using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity
{
    public static class PasswordValidatorTests
    {
        [Theory]
        [InlineData("Pass123123$")]
        public static void ValidateAsync_ValidPassword_ReturnsSuccessfulIdentityResult(string password)
        {
            var validator = new PasswordValidator();
            var result = validator.ValidateAsync(null, null, password);
            result.Result.Succeeded.Should().BeTrue();
        }

        // ReSharper disable once StringLiteralTypo
        [Theory]
        [InlineData("")]
        [InlineData("Pass12312")]
        [InlineData("pass123123")]
        [InlineData("PASS123123")]
        [InlineData("$$$$123123")]
        [InlineData("pass$$$$$$")]
        [InlineData("PASS$$$$$$")]
        [InlineData("PASSonetwothree")]
        public static void ValidateAsync_InvalidPassword_ReturnsFailureIdentityResult(string password)
        {
            var validator = new PasswordValidator();
            var result = validator.ValidateAsync(null, null, password);

            result.Result.Succeeded.Should().BeFalse();
            result.Result.Errors.Count().Should().Be(1);
            var error = result.Result.Errors.First();
            error.Code.Should().Be(PasswordValidator.InvalidPasswordCode);
            error.Description.Should().Be(PasswordValidator.PasswordConditionsNotMet);
        }
    }
}
