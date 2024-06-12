using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity;

[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "Skipping tests that use Temporal queries")]
public static class PasswordValidatorTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(PasswordValidator).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory(Skip = "Temporal queries not supported in EF Core 7.")]
    [MockInMemoryDbAutoData]
    public static async Task ValidateAsync_ValidPassword_NoPasswordHistory_ReturnsSuccessfulIdentityResult(
        [Frozen] BuyingCatalogueDbContext dbContext,
        AspNetUser user,
        UserManager<AspNetUser> userManager,
        IPasswordHasher<AspNetUser> mockPasswordHash,
        PasswordSettings passwordResetSettings)
    {
        var password = "Pass123123!";
        user.PasswordHash = null;

        dbContext.AspNetUsers.Add(user);

        await dbContext.SaveChangesAsync();

        var validator = new PasswordValidator(dbContext, mockPasswordHash, passwordResetSettings);
        PasswordValidator.ConfigurePasswordOptions(userManager.Options.Password);

        var result = validator.ValidateAsync(userManager, user, password);

        mockPasswordHash.Received(0).VerifyHashedPassword(Arg.Any<AspNetUser>(), Arg.Any<string>(), Arg.Any<string>());
        result.Result.Succeeded.Should().BeTrue();
    }

    [Theory(Skip = "Temporal queries not supported in EF Core 7.")]
    [MockInMemoryDbAutoData]
    public static async Task ValidateAsync_ValidPassword_PasswordNotInHistory_ReturnsSuccessfulIdentityResult(
        AspNetUser user,
        [Frozen] BuyingCatalogueDbContext dbContext,
        [Frozen] IPasswordHasher<AspNetUser> mockPasswordHash,
        UserManager<AspNetUser> userManager,
        PasswordSettings passwordResetSettings)
    {
        var password = "Pass123123!";
        dbContext.AspNetUsers.Add(user);

        await dbContext.SaveChangesAsync();

        mockPasswordHash.VerifyHashedPassword(user, user.PasswordHash, password).Returns(PasswordVerificationResult.Failed);

        var validator = new PasswordValidator(dbContext, mockPasswordHash, passwordResetSettings);
        PasswordValidator.ConfigurePasswordOptions(userManager.Options.Password);

        var result = validator.ValidateAsync(userManager, user, password);

        mockPasswordHash.Received().VerifyHashedPassword(user, user.PasswordHash, password);

        result.Result.Succeeded.Should().BeTrue();
    }

    [Theory(Skip = "Temporal queries not supported in EF Core 7.")]
    [MockInMemoryDbAutoData]
    public static async Task ValidateAsync_ValidPassword_PasswordInHistory_ReturnsFailureIdentityResult(
        AspNetUser user,
        [Frozen] BuyingCatalogueDbContext dbContext,
        [Frozen] IPasswordHasher<AspNetUser> mockPasswordHash,
        UserManager<AspNetUser> userManager,
        PasswordSettings passwordResetSettings)
    {
        var password = "Pass123123!";
        dbContext.AspNetUsers.Add(user);

        await dbContext.SaveChangesAsync();

        mockPasswordHash.VerifyHashedPassword(user, user.PasswordHash, password).Returns(PasswordVerificationResult.Success);

        var validator = new PasswordValidator(dbContext, mockPasswordHash, passwordResetSettings);
        PasswordValidator.ConfigurePasswordOptions(userManager.Options.Password);

        var result = validator.ValidateAsync(userManager, user, password);

        mockPasswordHash.Received().VerifyHashedPassword(user, user.PasswordHash, password);

        result.Result.Succeeded.Should().BeFalse();
        result.Result.Errors.Count().Should().Be(1);
        var error = result.Result.Errors.First();
        error.Code.Should().Be(PasswordValidator.PasswordAlreadyUsedCode);
        error.Description.Should().Be(PasswordValidator.PasswordAlreadyUsed);
    }

    [Theory]
    [MockInlineAutoData("")]
    [MockInlineAutoData("Pass12312")]
    [MockInlineAutoData("pass123123")]
    [MockInlineAutoData("PASS123123")]
    [MockInlineAutoData("$$$$123123")]
    [MockInlineAutoData("pass$$$$$$")]
    [MockInlineAutoData("PASS$$$$$$")]
    [MockInlineAutoData("PASSOneTwoThree")]
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
