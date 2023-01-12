using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity;

public static class PasswordValidatorTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(PasswordValidator).GetConstructors();

        assertion.Verify(constructors);
    }


    [Theory]
    [InMemoryDbAutoData]
    public static void ValidateAsync_ValidPassword_NotUsedBefore_ReturnsSuccessfulIdentityResult(
        [Frozen] BuyingCatalogueDbContext dbContext,
        AspNetUser user,
        UserManager<AspNetUser> userManager,
        Mock<IPasswordHasher<AspNetUser>> mockPasswordHash,
        PasswordResetSettings passwordResetSettings)
    {
        var password = "Pass123123!";
        var validator = new PasswordValidator(dbContext, mockPasswordHash.Object, passwordResetSettings);
        PasswordValidator.ConfigurePasswordOptions(userManager.Options.Password);

        var result = validator.ValidateAsync(userManager, user, password);
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
