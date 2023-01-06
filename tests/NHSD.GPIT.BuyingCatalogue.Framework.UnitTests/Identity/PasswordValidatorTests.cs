using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity;

public static class PasswordValidatorTests
{
    [Theory]
    [CommonInlineAutoData("Pass123123!")]
    [CommonInlineAutoData("Pass123$$$$$$")]
    [CommonInlineAutoData("PASs$$$123")]
    public static void ValidateAsync_ValidPassword_ReturnsSuccessfulIdentityResult(
        string password,
        UserManager<AspNetUser> userManager,
        PasswordValidator validator)
    {
        PasswordValidator.ConfigurePasswordOptions(userManager.Options.Password);

        var result = validator.ValidateAsync(userManager, null, password);
        result.Result.Succeeded.Should().BeTrue();
    }

    [Theory]
    [CommonInlineAutoData("")]
    [CommonInlineAutoData("Pass12312")]
    [CommonInlineAutoData("pass123123")]
    [CommonInlineAutoData("PASS123123")]
    [CommonInlineAutoData("$$$$123123")]
    [CommonInlineAutoData("pass$$$$$$")]
    [CommonInlineAutoData("PASS$$$$$$")]
    [CommonInlineAutoData("PASSOneTwoThree")]
    public static void ValidateAsync_InvalidPassword_ReturnsFailureIdentityResult(
        string password,
        UserManager<AspNetUser> userManager,
        PasswordValidator validator)
    {
        PasswordValidator.ConfigurePasswordOptions(userManager.Options.Password);

        var result = validator.ValidateAsync(userManager, null, password);

        result.Result.Succeeded.Should().BeFalse();
        result.Result.Errors.Count().Should().Be(1);
        var error = result.Result.Errors.First();
        error.Code.Should().Be(PasswordValidator.InvalidPasswordCode);
        error.Description.Should().Be(PasswordValidator.PasswordConditionsNotMet);
    }
}
