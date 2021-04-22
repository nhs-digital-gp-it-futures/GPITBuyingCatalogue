using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MailKit;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class MailKitEmailServiceTests
    {
        private static EmailMessageTemplate BasicTemplate => new(new EmailAddressTemplate("from@sender.test"))
        {
            Subject = "subject",
        };

        private static ICollection<EmailAddress> SingleRecipient => new[] { new EmailAddress("to@recipient.test") };

        [Test]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IMailTransport_SmtpSettings_AllowInvalidCertificateFalse_DoesNotSetCertificateValidationCallback()
        {
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.SetupProperty(t => t.ServerCertificateValidationCallback);

            new MailKitEmailService(
                mockTransport.Object,
                new SmtpSettings(),
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            mockTransport.Object.ServerCertificateValidationCallback.Should().BeNull();
        }

        [Test]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IMailTransport_SmtpSettings_AllowInvalidCertificateTrue_SetsCertificateValidationCallback()
        {
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.SetupProperty(t => t.ServerCertificateValidationCallback);

            new MailKitEmailService(
                mockTransport.Object,
                new SmtpSettings { AllowInvalidCertificate = true },
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            var callback = mockTransport.Object.ServerCertificateValidationCallback;

            callback.Invoke(null!, null, null, SslPolicyErrors.None).Should().BeTrue();
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IMailTransport_SmtpSettings_NullMailTransport_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new MailKitEmailService(
                null!,
                new SmtpSettings(),
                Mock.Of<ILogWrapper<MailKitEmailService>>()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing")]
        public static void Constructor_IMailTransport_SmtpSettings_NullSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new MailKitEmailService(
                Mock.Of<IMailTransport>(),
                null!,
                Mock.Of<ILogWrapper<MailKitEmailService>>()));
        }

        [Test]
        public static async Task SendEmailAsync_ConnectsWithExpectedSettings()
        {
            const string host = "host";
            const int port = 125;

            var settings = new SmtpSettings
            {
                Authentication = new SmtpAuthenticationSettings(),
                Host = host,
                Port = port,
            };

            var mockTransport = new Mock<IMailTransport>();

            var service = new MailKitEmailService(
                mockTransport.Object,
                settings,
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            await service.SendEmailAsync(new EmailMessage(BasicTemplate, SingleRecipient));

            mockTransport.Verify(
                t => t.ConnectAsync(
                    It.Is<string>(h => h == settings.Host),
                    It.Is<int>(p => p == settings.Port),
                    It.Is<SecureSocketOptions>(s => s == SecureSocketOptions.Auto),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public static void SendEmailAsync_ExceptionConnected_DoesDisconnect()
        {
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.Setup(t => t.IsConnected).Returns(true);
            mockTransport.Setup(
                t => t.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()))
                .ThrowsAsync(new ServiceNotAuthenticatedException());

            var message = new EmailMessage(BasicTemplate, SingleRecipient);

            var service = new MailKitEmailService(
                mockTransport.Object,
                new SmtpSettings(),
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            Assert.ThrowsAsync<ServiceNotAuthenticatedException>(async () => await service.SendEmailAsync(message));

            mockTransport.Verify(
                t => t.DisconnectAsync(
                    It.Is<bool>(q => q),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public static void SendEmailAsync_ExceptionNotConnected_DoesNotDisconnect()
        {
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.Setup(
                t => t.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()))
                .ThrowsAsync(new ServiceNotConnectedException());

            var message = new EmailMessage(BasicTemplate, SingleRecipient);

            var service = new MailKitEmailService(
                mockTransport.Object,
                new SmtpSettings(),
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            Assert.ThrowsAsync<ServiceNotConnectedException>(async () => await service.SendEmailAsync(message));

            mockTransport.Verify(
                t => t.DisconnectAsync(
                    It.Is<bool>(q => q),
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Test]
        public static void SendEmailAsync_NullEmailMessage_ThrowsException()
        {
            var emailService = new MailKitEmailService(
                Mock.Of<IMailTransport>(),
                new SmtpSettings(),
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await emailService.SendEmailAsync(null!));
        }

        [Test]
        public static async Task SendEmailAsync_RequireAuthenticationIsTrue_AuthenticatesUsingExpectedCredentials()
        {
            const string userName = "User";
            const string password = "Password";

            var authentication = new SmtpAuthenticationSettings
            {
                IsRequired = true,
                UserName = userName,
                Password = password,
            };

            var settings = new SmtpSettings { Authentication = authentication };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(
                mockTransport.Object,
                settings,
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            await service.SendEmailAsync(new EmailMessage(BasicTemplate, SingleRecipient));

            mockTransport.Verify(
                t => t.AuthenticateAsync(
                    It.Is<string>(u => u == authentication.UserName),
                    It.Is<string>(p => p == authentication.Password),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public static async Task SendEmailAsync_RequireAuthenticationIsFalse_DoesNotAuthenticate()
        {
            var settings = new SmtpSettings { Authentication = new SmtpAuthenticationSettings() };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(
                mockTransport.Object,
                settings,
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            await service.SendEmailAsync(new EmailMessage(BasicTemplate, SingleRecipient));

            mockTransport.Verify(
                t => t.AuthenticateAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Test]
        public static async Task SendEmailAsync_SendsExpectedMessage()
        {
            var settings = new SmtpSettings { Authentication = new SmtpAuthenticationSettings() };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(
                mockTransport.Object,
                settings,
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            var subject = Guid.NewGuid().ToString();
            var template = BasicTemplate with { Subject = subject };

            await service.SendEmailAsync(new EmailMessage(template, SingleRecipient));

            mockTransport.Verify(
                t => t.SendAsync(
                    It.Is<MimeMessage>(m => m.Subject == subject),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()),
                Times.Once());
        }

        [Test]
        public static async Task SendEmailAsync_UsesSettingsSubjectPrefix()
        {
            const string subjectPrefix = "Bananas";

            var settings = new SmtpSettings { EmailSubjectPrefix = subjectPrefix };
            var mockTransport = new Mock<IMailTransport>();
            var service = new MailKitEmailService(
                mockTransport.Object,
                settings,
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            var template = BasicTemplate with { Subject = null };

            await service.SendEmailAsync(new EmailMessage(template, SingleRecipient));

            mockTransport.Verify(
                t => t.SendAsync(
                    It.Is<MimeMessage>(m => m.Subject == subjectPrefix),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()),
                Times.Once());
        }

        [Test]
        public static async Task SendEmailAsync_Connected_Disconnects()
        {
            var settings = new SmtpSettings { Authentication = new SmtpAuthenticationSettings() };
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.Setup(t => t.IsConnected).Returns(true);

            var service = new MailKitEmailService(
                mockTransport.Object,
                settings,
                Mock.Of<ILogWrapper<MailKitEmailService>>());

            await service.SendEmailAsync(new EmailMessage(BasicTemplate, SingleRecipient));

            mockTransport.Verify(
                t => t.DisconnectAsync(
                    It.Is<bool>(q => q),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public static void SendEmailAsync_Exception_LogsError()
        {
            var mockLogger = new Mock<ILogWrapper<MailKitEmailService>>();
            var mockTransport = new Mock<IMailTransport>();
            mockTransport.Setup(
                t => t.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()))
                .ThrowsAsync(new SmtpFailedRecipientException(SmtpStatusCode.ServiceNotAvailable, "to@recipient.test"));

            var message = new EmailMessage(BasicTemplate, SingleRecipient);

            var service = new MailKitEmailService(
                mockTransport.Object,
                new SmtpSettings(),
                mockLogger.Object);

            Assert.ThrowsAsync<SmtpFailedRecipientException>(async () => await service.SendEmailAsync(message));

            mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),Times.Once);
        }
    }
}
