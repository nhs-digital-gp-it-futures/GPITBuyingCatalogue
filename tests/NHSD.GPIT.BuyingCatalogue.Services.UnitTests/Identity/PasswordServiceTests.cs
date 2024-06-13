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
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    public static class PasswordServiceTests
    {
        private static IUserStore<AspNetUser> MockUserStore => new IUserStore<AspNetUser>();

        private static Mock<UserManager<AspNetUser>> MockUserManager => new(
        MockUserStore.Object,
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
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
                    Mock.Of<IGovNotifyEmailService>(),
                    new PasswordSettings(),
                    MockUserManager.Object);

                return service.GeneratePasswordResetTokenAsync(emailAddress);
            }

            return Assert.ThrowsAsync<ArgumentException>(GeneratePasswordResetTokenAsync);
        }

        [Fact]
        public static async Task GeneratePasswordResetTokenAsync_UserNotFound_ReturnsNull()
        {
            var service = new PasswordService(
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                MockUserManager.Object);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().BeNull();
        }

        [Fact]
        public static async Task GeneratePasswordResetTokenAsync_UserFound_ReturnsExpectedToken()
        {
            const string emailAddress = "a@b.com";
            const string expectedToken = "HereBeToken";
            var expectedUser = AspNetUserBuilder.Create().Build();

            var mockUserManager = MockUserManager;
            mockUserManager.Setup(m => m.FindByEmailAsync(It.Is<string>(e => e == emailAddress)))
                .ReturnsAsync(expectedUser);

            mockUserManager
                .Setup(m => m.GeneratePasswordResetTokenAsync(It.Is<AspNetUser>(u => u == expectedUser)))
                .ReturnsAsync(expectedToken);

            var service = new PasswordService(
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager.Object);

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
                    Mock.Of<IGovNotifyEmailService>(),
                    new PasswordSettings(),
                    MockUserManager.Object);

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
                    Mock.Of<IGovNotifyEmailService>(),
                    new PasswordSettings(),
                    MockUserManager.Object);

                return service.SendResetEmailAsync(AspNetUserBuilder.Create().Build(), null);
            }

            return Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [Theory]
        [CommonAutoData]
        public static async Task SendResetEmailAsync_SendsEmail(
            [Frozen] PasswordSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> govNotifyEmailService,
            PasswordService passwordService)
        {
            var user = AspNetUserBuilder.Create().Build();
            await passwordService.SendResetEmailAsync(user, new Uri("https://duckduckgo.com/"));

            govNotifyEmailService.Verify(e => e.SendEmailAsync(
                user.Email,
                settings.EmailTemplateId,
                It.IsAny<Dictionary<string, dynamic>>()));
        }

        [Fact]
        public static async Task ResetPasswordAsync_WithUser_ReturnsIdentityResult()
        {
            const string email = "a@b.c";
            const string token = "I am a token, honest!";
            const string password = "Pass123321";

            var expectedResult = new IdentityResult();
            var user = AspNetUserBuilder.Create().Build();
            var mockUserManager = MockUserManager;
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(m => m.ResetPasswordAsync(user, token, password)).ReturnsAsync(() => expectedResult);

            var service = new PasswordService(
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager.Object);

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
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                MockUserManager.Object);

            var isValid = await service.IsValidPasswordResetTokenAsync(emailAddress, token);

            isValid.Should().BeFalse();
        }

        [Fact]
        public static async Task IsValidPasswordResetToken_InvokesVerifyUserTokenAsync()
        {
            const string emailAddress = "invalid@email.address.test";
            const string token = "Token";

            var expectedUser = AspNetUserBuilder.Create().Build();

            var mockUserManager = MockUserManager;
            mockUserManager.Setup(
                    u => u.FindByEmailAsync(It.Is<string>(e => e.Equals(emailAddress, StringComparison.Ordinal))))
                .ReturnsAsync(expectedUser);

            var service = new PasswordService(
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager.Object);

            await service.IsValidPasswordResetTokenAsync(emailAddress, token);

            mockUserManager.Verify(m => m.VerifyUserTokenAsync(
                It.Is<AspNetUser>(u => u == expectedUser),
                It.Is<string>(p => p.Equals(new IdentityOptions().Tokens.PasswordResetTokenProvider, StringComparison.Ordinal)),
                It.Is<string>(p => p.Equals(UserManager<AspNetUser>.ResetPasswordTokenPurpose, StringComparison.Ordinal)),
                It.Is<string>(t => t.Equals(token, StringComparison.Ordinal))));
        }

        [Fact]
        public static async Task ChangePasswordAsync_WithUser_ReturnsIdentityResult()
        {
            const string email = "a@b.c";
            const string currentPassword = "CurrentPassword";
            const string password = "Pass123321";

            var expectedResult = new IdentityResult();
            var user = AspNetUserBuilder.Create().Build();
            var mockUserManager = MockUserManager;
            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(m => m.ResetPasswordAsync(user, currentPassword, password)).ReturnsAsync(() => expectedResult);

            var service = new PasswordService(
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager.Object);

            var result = await service.ResetPasswordAsync(email, currentPassword, password);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdatePasswordChangedDate_WithUser_ReturnsIdentityResult(
            [Frozen] BuyingCatalogueDbContext context,
            Mock<UserManager<AspNetUser>> mockUserManager,
            AspNetUser user,
            string email)
        {
            var expectedResult = IdentityResult.Success;
            user.Email = email;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<AspNetUser>())).ReturnsAsync(() => expectedResult);

            var service = new PasswordService(
                Mock.Of<IGovNotifyEmailService>(),
                new PasswordSettings(),
                mockUserManager.Object);

            var result = await service.UpdatePasswordChangedDate(email);
            result.Should().Be(expectedResult);
        }
    }
}
