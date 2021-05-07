using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;
using NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Comparers;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.CreateBuyer
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CreateBuyerServiceTests
    {

        [Test]
        public static void Constructor_NullApplicationUserValidator_ThrowsException()
        {            
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                Mock.Of<ILogWrapper<CreateBuyerService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                Mock.Of<IEmailService>(),
                new RegistrationSettings(),
                null));
        }

        [Test]
        public static void Constructor_NullUserRepository_ThrowsException()
        {            
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                  Mock.Of<ILogWrapper<CreateBuyerService>>(),
                  null,
                  Mock.Of<IPasswordService>(),
                  Mock.Of<IPasswordResetCallback>(),
                  Mock.Of<IEmailService>(),
                  new RegistrationSettings(),
                  Mock.Of<IAspNetUserValidator>()));
        }

        [Test]
        public static void Constructor_NullPasswordService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                  Mock.Of<ILogWrapper<CreateBuyerService>>(),
                  Mock.Of<IUsersDbRepository<AspNetUser>>(),
                  null,
                  Mock.Of<IPasswordResetCallback>(),
                  Mock.Of<IEmailService>(),
                  new RegistrationSettings(),
                  Mock.Of<IAspNetUserValidator>()));
        }

        [Test]
        public static void Constructor_NullLogger_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                  null,
                  Mock.Of<IUsersDbRepository<AspNetUser>>(),
                  Mock.Of<IPasswordService>(),
                  Mock.Of<IPasswordResetCallback>(),
                  Mock.Of<IEmailService>(),
                  new RegistrationSettings(),
                  Mock.Of<IAspNetUserValidator>()));
        }

        [Test]
        public static void Constructor_NullPasswordCallback_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                  Mock.Of<ILogWrapper<CreateBuyerService>>(),
                  Mock.Of<IUsersDbRepository<AspNetUser>>(),
                  Mock.Of<IPasswordService>(),
                  null,
                  Mock.Of<IEmailService>(),
                  new RegistrationSettings(),
                  Mock.Of<IAspNetUserValidator>()));
        }

        [Test]
        public static void Constructor_NullEmailServiceCallback_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                  Mock.Of<ILogWrapper<CreateBuyerService>>(),
                  Mock.Of<IUsersDbRepository<AspNetUser>>(),
                  Mock.Of<IPasswordService>(),
                  Mock.Of<IPasswordResetCallback>(),
                  null,
                  new RegistrationSettings(),
                  Mock.Of<IAspNetUserValidator>()));
        }

        [Test]
        public static void Constructor_NullRegistrationServiceCallback_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CreateBuyerService(
                  Mock.Of<ILogWrapper<CreateBuyerService>>(),
                  Mock.Of<IUsersDbRepository<AspNetUser>>(),
                  Mock.Of<IPasswordService>(),
                  Mock.Of<IPasswordResetCallback>(),
                  Mock.Of<IEmailService>(),
                  null,
                  Mock.Of<IAspNetUserValidator>()));
        }


        [Test]
        public static async Task CreateAsync_SuccessfulApplicationUserValidation_ReturnsSuccess()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            var sut = context.CreateBuyerService;
            
            var actual = await sut.Create(Guid.NewGuid(), "Test", "Smith", "0123456789", "a.b@c.com");

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull();
        }

        [Test]
        public static async Task CreateAsync_ApplicationUserValidation_CalledOnce()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            var sut = context.CreateBuyerService;

            var primaryOrganisationId = Guid.NewGuid();

            await sut.Create(primaryOrganisationId, "Test", "Smith", "0123456789", "a.b@c.com");

            var expected = AspNetUserBuilder
                .Create()
                .WithFirstName("Test")
                .WithLastName("Smith")
                .WithPhoneNumber("0123456789")
                .WithEmailAddress("a.b@c.com")
                .WithPrimaryOrganisationId(primaryOrganisationId)
                .Build();

            context.AspNetUserValidatorMock.Verify(v => v.ValidateAsync(
                It.Is<AspNetUser>(actual => AspNetUserEditableInformationComparer.Instance.Equals(expected, actual))));
        }

        [Test]
        public static async Task CreateAsync_SuccessfulApplicationUserValidation_UserRepository_CalledOnce()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            var sut = context.CreateBuyerService;

            var primaryOrganisationId = Guid.NewGuid();

            await sut.Create(primaryOrganisationId, "Test", "Smith", "0123456789", "a.b@c.com");

            var expected = AspNetUserBuilder
                .Create()
                .WithFirstName("Test")
                .WithLastName("Smith")
                .WithPhoneNumber("0123456789")
                .WithEmailAddress("a.b@c.com")
                .WithPrimaryOrganisationId(primaryOrganisationId)
                .Build();

            context.UsersRepositoryMock.Verify(r => r.Add(
                It.Is<AspNetUser>(actual => AspNetUserEditableInformationComparer.Instance.Equals(expected, actual))));
        }

        [Test]
        public static async Task CreateAsync_ApplicationUserValidationFails_ReturnFailureResult()
        {
            var context = CreateBuyerServiceTestContext.Setup();
            context.AspNetUserValidatorResult = Result.Failure(new List<ErrorDetails>());

            var sut = context.CreateBuyerService;
            
            var actual = await sut.Create(Guid.NewGuid(), "Test", "Smith", "0123456789", "a.b@c.com");

            var expected = Result.Failure<string>(new List<ErrorDetails>());
            actual.Should().Be(expected);
        }

        [Test]
        public static async Task CreateBuyerAsync_NewApplicationUser_SendsEmail()
        {
            const string expectedToken = "TokenMcToken";
            var primaryOrganisationId = Guid.NewGuid();

            var context = CreateBuyerServiceTestContext.Setup();

            var expectedUser = AspNetUserBuilder
                .Create()
                .WithFirstName("Test")
                .WithLastName("Smith")
                .WithPhoneNumber("0123456789")
                .WithEmailAddress("a.b@c.com")
                .WithPrimaryOrganisationId(primaryOrganisationId)
                .Build();

            context.PasswordServiceMock.Setup(
                p => p.GeneratePasswordResetTokenAsync(It.Is<string>(e => e == "a.b@c.com")))
                .ReturnsAsync(new PasswordResetToken(expectedToken, expectedUser));

            var sut = context.CreateBuyerService;
            await sut.Create(primaryOrganisationId, "Test", "Smith", "0123456789", "a.b@c.com");

            Expression<Func<PasswordResetToken, bool>> expected = t =>
                t.Token.Equals(expectedToken, StringComparison.Ordinal)
                && AspNetUserEditableInformationComparer.Instance.Equals(expectedUser, t.User);

            context.EmailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>()));
        }
        
        [Test]
        public static void SendInitialEmailAsync_NullUser_ThrowsException()
        {
            static async Task SendEmail()
            {
                var createBuyerService = new CreateBuyerService(
                    Mock.Of<ILogWrapper<CreateBuyerService>>(),
                    Mock.Of<IUsersDbRepository<AspNetUser>>(),
                    Mock.Of<IPasswordService>(),
                    Mock.Of<IPasswordResetCallback>(),
                    Mock.Of<IEmailService>(),
                    new RegistrationSettings(),
                    Mock.Of<IAspNetUserValidator>());
                    
                await createBuyerService.SendInitialEmailAsync(null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(SendEmail);
        }

        [Test]
        public static async Task SendInitialEmailAsync_SendsEmail()
        {
            var inputMessage = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                HtmlContent = "HTML",
                PlainTextContent = "Text",
            };

            var settings = new RegistrationSettings { EmailMessage = inputMessage };
            var mockEmailService = new Mock<IEmailService>();
            var mockPasswordResetCallback = Mock.Of<IPasswordResetCallback>(
                c => c.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()) == new Uri("https://www.google.co.uk/"));

            var user = AspNetUserBuilder
                .Create()
                .WithEmailAddress("ricardo@burton.com")
                .Build();
    
            var createBuyerService = new CreateBuyerService(
                Mock.Of<ILogWrapper<CreateBuyerService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>(),
                Mock.Of<IPasswordService>(),
                mockPasswordResetCallback,
                mockEmailService.Object,
                settings,
                Mock.Of<IAspNetUserValidator>());

            await createBuyerService.SendInitialEmailAsync(new PasswordResetToken("Token", user));

            mockEmailService.Verify(e => e.SendEmailAsync(It.IsNotNull<EmailMessage>()));
        }

        [Test]
        public static async Task SendInitialEmailAsync_UsesExpectedTemplate()
        {
            // ReSharper disable once StringLiteralTypo
            const string subject = "Gozleme";

            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                Subject = subject,
            };

            var mockEmailService = new MockEmailService();

            var createBuyerService = new CreateBuyerService(
                Mock.Of<ILogWrapper<CreateBuyerService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                mockEmailService,
                new RegistrationSettings { EmailMessage = template },
                Mock.Of<IAspNetUserValidator>());

            await createBuyerService.SendInitialEmailAsync(
                new PasswordResetToken("Token", AspNetUserBuilder.Create().Build()));

            mockEmailService.SentMessage.Subject.Should().Be(subject);
        }

        [Test]
        public static async Task SendInitialEmailAsync_UsesExpectedRecipient()
        {
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));
            var mockEmailService = new MockEmailService();

            var user = AspNetUserBuilder
                .Create()
                .WithFirstName("Uncle")
                .WithLastName("Bob")
                .WithEmailAddress("uncle@bob.com")
                .Build();

            var createBuyerService = new CreateBuyerService(
                Mock.Of<ILogWrapper<CreateBuyerService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                mockEmailService,
                new RegistrationSettings { EmailMessage = template },
                Mock.Of<IAspNetUserValidator>());

            await createBuyerService.SendInitialEmailAsync(new PasswordResetToken("Token", user));

            var recipients = mockEmailService.SentMessage.Recipients;
            recipients.Should().HaveCount(1);

            var recipient = recipients[0];
            recipient.Address.Should().Be(user.Email);
            recipient.DisplayName.Should().Be(user.GetDisplayName());
        }

        [Test]
        public static async Task SendInitialEmailAsync_UsesExpectedCallback()
        {
            const string expectedCallback = "https://callback.nhs.uk/";

            var callback = new Uri(expectedCallback);
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

            var passwordResetCallback = Mock.Of<IPasswordResetCallback>(
                c => c.GetPasswordResetCallback(It.IsNotNull<PasswordResetToken>()) == callback);
                                                
            var mockEmailService = new MockEmailService();

            var createBuyerService = new CreateBuyerService(
                Mock.Of<ILogWrapper<CreateBuyerService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>(),
                Mock.Of<IPasswordService>(),
                passwordResetCallback,
                mockEmailService,
                new RegistrationSettings { EmailMessage = template },
                Mock.Of<IAspNetUserValidator>());

            await createBuyerService.SendInitialEmailAsync(
                new PasswordResetToken("Token", AspNetUserBuilder.Create().Build()));

            mockEmailService.SentMessage.TextBody!.FormatItems.Should().HaveCount(1);
            mockEmailService.SentMessage.TextBody!.FormatItems[0].Should().Be(expectedCallback);
        }

        private sealed class CreateBuyerServiceTestContext
        {
            private CreateBuyerServiceTestContext()
            {
                AspNetUserValidatorMock = new Mock<IAspNetUserValidator>();
                AspNetUserValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<AspNetUser>()))
                    .ReturnsAsync(() => AspNetUserValidatorResult);

                UsersRepositoryMock = new Mock<IUsersDbRepository<AspNetUser>>();
                UsersRepositoryMock.Setup(r => r.Add(It.IsAny<AspNetUser>()));

                PasswordServiceMock = new Mock<IPasswordService>();
                PasswordServiceMock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync(new PasswordResetToken("123", AspNetUserBuilder.Create().Build()));

                EmailServiceMock = new Mock<IEmailService>();

                var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

                CreateBuyerService = new CreateBuyerService(
                    Mock.Of<ILogWrapper<CreateBuyerService>>(),
                    UsersRepositoryMock.Object,
                    PasswordServiceMock.Object,
                    Mock.Of<IPasswordResetCallback>(),
                    EmailServiceMock.Object,
                    new RegistrationSettings { EmailMessage = template },
                    AspNetUserValidatorMock.Object);
            }

            internal Mock<IAspNetUserValidator> AspNetUserValidatorMock { get; }

            internal Result AspNetUserValidatorResult { get; set; } = Result.Success();

            internal Mock<IUsersDbRepository<AspNetUser>> UsersRepositoryMock { get; }

            internal CreateBuyerService CreateBuyerService { get; }

            internal Mock<IPasswordService> PasswordServiceMock { get; }

            internal Mock<IEmailService> EmailServiceMock { get; }

            public static CreateBuyerServiceTestContext Setup()
            {
                return new();
            }
        }
    }
}
