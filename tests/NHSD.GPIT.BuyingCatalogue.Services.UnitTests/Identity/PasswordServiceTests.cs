using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PasswordServiceTests
    {
        private static Mock<IUserStore<AspNetUser>> MockUserStore => new();

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

        [Test]
        public static void Constructor_IEmailService_PasswordResetSettings_UserManagerApplicationUser_NullEmailService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new PasswordService(
                    null,
                    new PasswordResetSettings(),
                    MockUserManager.Object));
        }

        [Test]
        public static void Constructor_IEmailService_PasswordResetSettings_UserManagerApplicationUser_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new PasswordService(
                    Mock.Of<IEmailService>(),
                    null,
                    MockUserManager.Object));
        }

        [Test]
        public static void Constructor_IEmailService_PasswordResetSettings_UserManagerApplicationUser_NullUserManager_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public static void GeneratePasswordResetTokenAsync_EmptyOrWhiteSpaceEmailAddress_ThrowsException(string emailAddress)
        {
            async Task GeneratePasswordResetTokenAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.GeneratePasswordResetTokenAsync(emailAddress);
            }

            Assert.ThrowsAsync<ArgumentException>(GeneratePasswordResetTokenAsync);
        }

        [Test]
        public static async Task GeneratePasswordResetTokenAsync_UserNotFound_ReturnsNull()
        {
            var service = new PasswordService(
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                MockUserManager.Object);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().BeNull();
        }

        [Test]
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
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                mockUserManager.Object);

            var token = await service.GeneratePasswordResetTokenAsync("a@b.com");

            token.Should().NotBeNull();
            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }

        [Test]
        public static void SendResetEmailAsync_NullUser_ThrowsException()
        {
            static async Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.SendResetEmailAsync(null, new Uri("https://www.google.co.uk/"));
            }

            Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [Test]
        public static void SendResetEmailAsync_NullCallback_ThrowsException()
        {
            static async Task SendResetEmailAsync()
            {
                var service = new PasswordService(
                    Mock.Of<IEmailService>(),
                    new PasswordResetSettings(),
                    MockUserManager.Object);

                await service.SendResetEmailAsync(AspNetUserBuilder.Create().Build(), null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(SendResetEmailAsync);
        }

        [Test]
        public static async Task SendResetEmailAsync_SendsEmail()
        {
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));
            var mockEmailService = new Mock<IEmailService>();
            var registrationService = new PasswordService(
                mockEmailService.Object,
                new PasswordResetSettings { EmailMessageTemplate = template },
                MockUserManager.Object);

            await registrationService.SendResetEmailAsync(
                AspNetUserBuilder.Create().Build(),
                new Uri("https://duckduckgo.com/"));

            mockEmailService.Verify(e => e.SendEmailAsync(It.IsNotNull<EmailMessage>()));
        }

        [Test]
        public static async Task SendResetEmailAsync_UsesExpectedTemplate()
        {
            // ReSharper disable once StringLiteralTypo
            const string subject = "Gozleme";

            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                Subject = subject,
            };

            var mockEmailService = new MockEmailService();
            var registrationService = new PasswordService(
                mockEmailService,
                new PasswordResetSettings { EmailMessageTemplate = template },
                MockUserManager.Object);

            await registrationService.SendResetEmailAsync(
                AspNetUserBuilder.Create().Build(),
                new Uri("https://duckduckgo.com/"));

            mockEmailService.SentMessage.Subject.Should().Be(subject);
        }

        [Test]
        public static async Task SendResetEmailAsync_UsesExpectedRecipient()
        {
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));
            var mockEmailService = new MockEmailService();

            var user = AspNetUserBuilder
                .Create()
                .WithFirstName("Uncle")
                .WithLastName("Bob")
                .WithEmailAddress("uncle@bob.com")
                .Build();

            var registrationService = new PasswordService(
                mockEmailService,
                new PasswordResetSettings { EmailMessageTemplate = template },
                MockUserManager.Object);

            await registrationService.SendResetEmailAsync(
                user,
                new Uri("https://duckduckgo.com/"));

            var recipients = mockEmailService.SentMessage.Recipients;
            recipients.Should().HaveCount(1);

            var recipient = recipients[0];
            recipient.Address.Should().Be(user.Email);
            recipient.DisplayName.Should().Be($"{user.FirstName} {user.LastName}");
        }

        [Test]
        public static async Task SendResetEmailAsync_UsesExpectedCallback()
        {
            const string expectedCallback = "https://callback.nhs.uk/";

            var callback = new Uri(expectedCallback);
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

            var mockEmailService = new MockEmailService();
            var registrationService = new PasswordService(
                mockEmailService,
                new PasswordResetSettings { EmailMessageTemplate = template },
                MockUserManager.Object);

            await registrationService.SendResetEmailAsync(
                AspNetUserBuilder.Create().Build(),
                callback);

            mockEmailService.SentMessage.TextBody!.FormatItems.Should().HaveCount(1);
            mockEmailService.SentMessage.TextBody!.FormatItems[0].Should().Be(expectedCallback);
        }

        [Test]
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
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                mockUserManager.Object);

            var result = await service.ResetPasswordAsync(email, token, password);
            result.Should().Be(expectedResult);
        }

        [TestCase(null, "ValidToken")]
        [TestCase("", "ValidToken")]
        [TestCase("\t", "ValidToken")]
        [TestCase("valid@email.address.test", null)]
        [TestCase("valid@email.address.test", "")]
        [TestCase("valid@email.address.test", "\t")]
        [TestCase("invalid@email.address.test", "ValidToken")]
        public static async Task IsValidPasswordResetToken_BadInput_ReturnsFalse(string emailAddress, string token)
        {
            var service = new PasswordService(
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                MockUserManager.Object);

            var isValid = await service.IsValidPasswordResetTokenAsync(emailAddress, token);

            isValid.Should().BeFalse();
        }

        [Test]
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
                Mock.Of<IEmailService>(),
                new PasswordResetSettings(),
                mockUserManager.Object);

            await service.IsValidPasswordResetTokenAsync(emailAddress, token);

            mockUserManager.Verify(m => m.VerifyUserTokenAsync(
                It.Is<AspNetUser>(u => u == expectedUser),
                It.Is<string>(p => p.Equals(new IdentityOptions().Tokens.PasswordResetTokenProvider, StringComparison.Ordinal)),
                It.Is<string>(p => p.Equals(UserManager<AspNetUser>.ResetPasswordTokenPurpose, StringComparison.Ordinal)),
                It.Is<string>(t => t.Equals(token, StringComparison.Ordinal))));
        }
    }
}
