using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    public static class PasswordServiceTests
    {
        private static IUserStore<AspNetUser> mockUserStore = Substitute.For<IUserStore<AspNetUser>>();

        private static UserManager<AspNetUser> mockUserManager = Substitute.For<UserManager<AspNetUser>>(
        mockUserStore,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null);

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(PasswordService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public static Task GeneratePasswordResetTokenAsync_EmptyOrWhiteSpaceEmailAddress_ThrowsException(string emailAddress)
        {
            Task GeneratePasswordResetTokenAsync()
            {
                var service = new PasswordService(
                    Substitute.For<IGovNotifyEmailService>(),
                    new PasswordSettings(),
                    mockUserManager);

                return service.GeneratePasswordResetTokenAsync(emailAddress);
            }

            return Assert.ThrowsAsync<ArgumentException>(GeneratePasswordResetTokenAsync);
        }

        [Fact]
        public static async Task GeneratePasswordResetTokenAsync_UserNotFound_ReturnsNull()
        {
            mockUserManager.FindByEmailAsync(Arg.Any<string>()).ReturnsNull();
            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().BeNull();
        }

        [Fact]
        public static async Task GeneratePasswordResetTokenAsync_UserFound_ReturnsExpectedToken()
        {
            const string emailAddress = "a@b.com";
            const string expectedToken = "HereBeToken";
            var expectedUser = AspNetUserBuilder.Create().Build();

            var mockUserManager = PasswordServiceTests.mockUserManager;
            mockUserManager.FindByEmailAsync(Arg.Is<string>(e => e == emailAddress)).Returns(expectedUser);

            mockUserManager.GeneratePasswordResetTokenAsync(Arg.Is<AspNetUser>(u => u == expectedUser)).Returns(expectedToken);

            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().NotBeNull();
            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }

        [Fact]
        public static Task SendResetEmailAsync_NullUser_ThrowsException()
        {
            static Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Substitute.For<IGovNotifyEmailService>(),
                    new PasswordSettings(),
                    mockUserManager);

                return service.SendResetEmailAsync(null, new Uri("https://www.google.co.uk/"));
            }

            return Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [Fact]
        public static Task SendResetEmailAsync_NullCallback_ThrowsException()
        {
            static Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Substitute.For<IGovNotifyEmailService>(),
                    new PasswordSettings(),
                    mockUserManager);

                return service.SendResetEmailAsync(AspNetUserBuilder.Create().Build(), null);
            }

            return Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [Theory]
        [MockAutoData]
        public static async Task SendResetEmailAsync_SendsEmail(
            [Frozen] PasswordSettings settings,
            [Frozen] IGovNotifyEmailService govNotifyEmailService,
            PasswordService passwordService)
        {
            var user = AspNetUserBuilder.Create().Build();
            await passwordService.SendResetEmailAsync(user, new Uri("https://duckduckgo.com/"));

            await govNotifyEmailService.Received().SendEmailAsync(
                user.Email,
                settings.EmailTemplateId,
                Arg.Any<Dictionary<string, dynamic>>());
        }

        [Fact]
        public static async Task ResetPasswordAsync_WithUser_ReturnsIdentityResult()
        {
            const string email = "a@b.c";
            const string token = "I am a token, honest!";
            const string password = "Pass123321";

            var expectedResult = new IdentityResult();
            var user = AspNetUserBuilder.Create().Build();
            var mockUserManager = PasswordServiceTests.mockUserManager;
            mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
            mockUserManager.ResetPasswordAsync(user, token, password).Returns(expectedResult);

            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            var result = await service.ResetPasswordAsync(email, token, password);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(null, "ValidToken")]
        [InlineData("", "ValidToken")]
        [InlineData("\t", "ValidToken")]
        [InlineData("valid@email.address.test", null)]
        [InlineData("valid@email.address.test", "")]
        [InlineData("valid@email.address.test", "\t")]
        [InlineData("invalid@email.address.test", "ValidToken")]
        public static async Task IsValidPasswordResetToken_BadInput_ReturnsFalse(string emailAddress, string token)
        {
            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            var isValid = await service.IsValidPasswordResetTokenAsync(emailAddress, token);

            isValid.Should().BeFalse();
        }

        [Fact]
        public static async Task IsValidPasswordResetToken_InvokesVerifyUserTokenAsync()
        {
            const string emailAddress = "invalid@email.address.test";
            const string token = "Token";

            var expectedUser = AspNetUserBuilder.Create().Build();

            var mockUserManager = PasswordServiceTests.mockUserManager;
            mockUserManager.FindByEmailAsync(Arg.Is<string>(e => e.Equals(emailAddress, StringComparison.Ordinal))).Returns(expectedUser);

            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            await service.IsValidPasswordResetTokenAsync(emailAddress, token);

            await mockUserManager.VerifyUserTokenAsync(
                Arg.Is<AspNetUser>(u => u == expectedUser),
                Arg.Is<string>(p => p.Equals(new IdentityOptions().Tokens.PasswordResetTokenProvider, StringComparison.Ordinal)),
                Arg.Is<string>(p => p.Equals(UserManager<AspNetUser>.ResetPasswordTokenPurpose, StringComparison.Ordinal)),
                Arg.Is<string>(t => t.Equals(token, StringComparison.Ordinal)));
        }

        [Fact]
        public static async Task ChangePasswordAsync_WithUser_ReturnsIdentityResult()
        {
            const string email = "a@b.c";
            const string currentPassword = "CurrentPassword";
            const string password = "Pass123321";

            var expectedResult = new IdentityResult();
            var user = AspNetUserBuilder.Create().Build();
            var mockUserManager = PasswordServiceTests.mockUserManager;
            mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
            mockUserManager.ResetPasswordAsync(user, currentPassword, password).Returns(expectedResult);

            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            var result = await service.ResetPasswordAsync(email, currentPassword, password);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task UpdatePasswordChangedDate_WithUser_ReturnsIdentityResult(
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            string email)
        {
            var mockUserStore = Substitute.For<IUserStore<AspNetUser>>();
            var mockUserManager = Substitute.For<UserManager<AspNetUser>>(
                mockUserStore,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            var expectedResult = IdentityResult.Success;
            user.Email = email;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            mockUserManager.UpdateAsync(Arg.Any<AspNetUser>()).Returns(expectedResult);

            var service = new PasswordService(
                Substitute.For<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager);

            var result = await service.UpdatePasswordChangedDate(email);
            result.Should().Be(expectedResult);
        }
    }
}
