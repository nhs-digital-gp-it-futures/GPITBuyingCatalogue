using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;
using NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Comparers;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.CreateBuyer
{
    public static class CreateBuyerServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateBuyerService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        public static Task Create_NullOrEmptyEmailAddress_ThrowsException(
            string emailAddress,
            CreateBuyerService service)
        {
            return Assert.ThrowsAsync<ArgumentException>(() => service.Create(1, "a", "b", "c", emailAddress));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_SuccessfulApplicationUserValidation_ReturnsSuccess(
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            var context = CreateBuyerServiceTestContext.Setup(dbContext);

            var sut = context.CreateBuyerService;

            var actual = await sut.Create(1, "Test", "Smith", "0123456789", "a.b@c.com");

            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_ApplicationUserValidation_CalledOnce(
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            var context = CreateBuyerServiceTestContext.Setup(dbContext);
            var sut = context.CreateBuyerService;

            const int primaryOrganisationId = 1;

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

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_SuccessfulApplicationUserValidation_UserRepository_CalledOnce(
            [Frozen] BuyingCatalogueDbContext dbContext,
            int primaryOrganisationId,
            string firstName,
            string lastName)
        {
            var context = CreateBuyerServiceTestContext.Setup(dbContext);
            var sut = context.CreateBuyerService;

            await sut.Create(primaryOrganisationId, firstName, lastName, "0123456789", "a.b@c.com");

            var actual = await dbContext.AspNetUsers.SingleAsync(u => u.Email == "a.b@c.com");

            actual.PrimaryOrganisationId.Should().Be(primaryOrganisationId);
            actual.FirstName.Should().Be(firstName);
            actual.LastName.Should().Be(lastName);
            actual.PhoneNumber.Should().Be("0123456789");
            actual.Email.Should().Be("a.b@c.com");
            actual.OrganisationFunction.Should().Be("Buyer");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_ApplicationUserValidationFails_ReturnFailureResult(
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            var context = CreateBuyerServiceTestContext.Setup(dbContext);
            context.AspNetUserValidatorResult = Result.Failure(new List<ErrorDetails>());

            var sut = context.CreateBuyerService;

            var actual = await sut.Create(1, "Test", "Smith", "0123456789", "a.b@c.com");

            var expected = Result.Failure<int>(new List<ErrorDetails>());
            actual.Should().Be(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_NewApplicationUser_SendsEmail(
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            const string expectedToken = "TokenMcToken";
            const int primaryOrganisationId = 27;

            var context = CreateBuyerServiceTestContext.Setup(dbContext);

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

            context.EmailServiceMock.Verify(s => s.SendEmailAsync(It.IsAny<EmailMessage>()));
        }

        [Theory]
        [CommonAutoData]
        public static Task SendInitialEmailAsync_NullUser_ThrowsException(
            CreateBuyerService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.SendInitialEmailAsync(null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SendInitialEmailAsync_SendsEmail(
            [Frozen] BuyingCatalogueDbContext dbContext)
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
                dbContext,
                Mock.Of<IPasswordService>(),
                mockPasswordResetCallback,
                mockEmailService.Object,
                settings,
                Mock.Of<IAspNetUserValidator>());

            await createBuyerService.SendInitialEmailAsync(new PasswordResetToken("Token", user));

            mockEmailService.Verify(e => e.SendEmailAsync(It.IsNotNull<EmailMessage>()));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SendInitialEmailAsync_UsesExpectedTemplate(
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            // ReSharper disable once StringLiteralTypo
            const string subject = "Gozleme";

            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"))
            {
                Subject = subject,
            };

            var mockEmailService = new MockEmailService();

            var createBuyerService = new CreateBuyerService(
                dbContext,
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                mockEmailService,
                new RegistrationSettings { EmailMessage = template },
                Mock.Of<IAspNetUserValidator>());

            await createBuyerService.SendInitialEmailAsync(
                new PasswordResetToken("Token", AspNetUserBuilder.Create().Build()));

            mockEmailService.SentMessage.Subject.Should().Be(subject);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SendInitialEmailAsync_UsesExpectedRecipient(
            [Frozen] BuyingCatalogueDbContext dbContext)
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
                dbContext,
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

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SendInitialEmailAsync_UsesExpectedCallback(
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            const string expectedCallback = "https://callback.nhs.uk/";

            var callback = new Uri(expectedCallback);
            var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

            var passwordResetCallback = Mock.Of<IPasswordResetCallback>(
                c => c.GetPasswordResetCallback(It.IsNotNull<PasswordResetToken>()) == callback);

            var mockEmailService = new MockEmailService();

            var createBuyerService = new CreateBuyerService(
                dbContext,
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
            private CreateBuyerServiceTestContext(BuyingCatalogueDbContext dbContext)
            {
                AspNetUserValidatorMock = new Mock<IAspNetUserValidator>();
                AspNetUserValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<AspNetUser>()))
                    .ReturnsAsync(() => AspNetUserValidatorResult);

                PasswordServiceMock = new Mock<IPasswordService>();
                PasswordServiceMock.Setup(s => s.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync(new PasswordResetToken("123", AspNetUserBuilder.Create().Build()));

                EmailServiceMock = new Mock<IEmailService>();

                var template = new EmailMessageTemplate(new EmailAddressTemplate("from@sender.test"));

                CreateBuyerService = new CreateBuyerService(
                    dbContext,
                    PasswordServiceMock.Object,
                    Mock.Of<IPasswordResetCallback>(),
                    EmailServiceMock.Object,
                    new RegistrationSettings { EmailMessage = template },
                    AspNetUserValidatorMock.Object);
            }

            internal Mock<IAspNetUserValidator> AspNetUserValidatorMock { get; }

            internal Result AspNetUserValidatorResult { get; set; } = Result.Success();

            internal CreateBuyerService CreateBuyerService { get; }

            internal Mock<IPasswordService> PasswordServiceMock { get; }

            internal Mock<IEmailService> EmailServiceMock { get; }

            public static CreateBuyerServiceTestContext Setup(BuyingCatalogueDbContext dbContext)
            {
                return new CreateBuyerServiceTestContext(dbContext);
            }
        }
    }
}
