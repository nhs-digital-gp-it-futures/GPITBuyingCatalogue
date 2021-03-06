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
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Builders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Users
{
    public static class CreateUserServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateUserService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        public static Task Create_NullOrEmptyEmailAddress_ThrowsException(
            string emailAddress,
            CreateUserService service)
        {
            return Assert.ThrowsAsync<ArgumentException>(
                () => service.Create(
                    1,
                    "a",
                    "b",
                    emailAddress,
                    OrganisationFunction.BuyerName));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_SuccessfulApplicationUserValidation_ReturnsSuccess(
            string expectedToken,
            [Frozen] Mock<IPasswordResetCallback> mockPasswordResetCallback,
            [Frozen] Mock<IPasswordService> mockPasswordService,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.Setup(p => p.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()))
                .Returns(new Uri("http://www.test.com"));

            mockPasswordService.Setup(
                p => p.GeneratePasswordResetTokenAsync(It.Is<string>(e => e == expectedUser.Email)))
                .ReturnsAsync(new PasswordResetToken(expectedToken, expectedUser));

            var actual = await service.Create(
                1,
                "Test",
                "Smith",
                "a.b@c.com",
                OrganisationFunction.BuyerName);

            actual.Id.Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_SuccessfulApplicationUserValidation_UserAddedToDbContext(
            string expectedToken,
            [Frozen] Mock<IPasswordResetCallback> mockPasswordResetCallback,
            [Frozen] Mock<IPasswordService> mockPasswordService,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.Setup(p => p.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()))
                .Returns(new Uri("http://www.test.com"));

            mockPasswordService.Setup(
                p => p.GeneratePasswordResetTokenAsync(It.Is<string>(e => e == expectedUser.Email)))
                .ReturnsAsync(new PasswordResetToken(expectedToken, expectedUser));

            var result = await service.Create(
                expectedUser.PrimaryOrganisationId,
                expectedUser.FirstName,
                expectedUser.LastName,
                expectedUser.Email,
                OrganisationFunction.BuyerName);

            var actual = await dbContext.AspNetUsers.SingleAsync(u => u.Id == result.Id);

            actual.PrimaryOrganisationId.Should().Be(expectedUser.PrimaryOrganisationId);
            actual.FirstName.Should().Be(expectedUser.FirstName);
            actual.LastName.Should().Be(expectedUser.LastName);
            actual.Email.Should().Be(expectedUser.Email);
            actual.OrganisationFunction.Should().Be("Buyer");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_SuccessfulApplicationUserValidation_AdminUser_UserAddedToDbContext(
            string expectedToken,
            [Frozen] Mock<IPasswordResetCallback> mockPasswordResetCallback,
            [Frozen] Mock<IPasswordService> mockPasswordService,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.Setup(p => p.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()))
                .Returns(new Uri("http://www.test.com"));

            mockPasswordService.Setup(
                p => p.GeneratePasswordResetTokenAsync(It.Is<string>(e => e == expectedUser.Email)))
                .ReturnsAsync(new PasswordResetToken(expectedToken, expectedUser));

            var result = await service.Create(
                expectedUser.PrimaryOrganisationId,
                expectedUser.FirstName,
                expectedUser.LastName,
                expectedUser.Email,
                OrganisationFunction.AuthorityName);

            var actual = await dbContext.AspNetUsers.SingleAsync(u => u.Id == result.Id);

            actual.PrimaryOrganisationId.Should().Be(expectedUser.PrimaryOrganisationId);
            actual.FirstName.Should().Be(expectedUser.FirstName);
            actual.LastName.Should().Be(expectedUser.LastName);
            actual.Email.Should().Be(expectedUser.Email);
            actual.OrganisationFunction.Should().Be("Authority");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task Create_NewApplicationUser_SendsEmail(
            string expectedToken,
            [Frozen] RegistrationSettings settings,
            [Frozen] Mock<IPasswordResetCallback> mockPasswordResetCallback,
            [Frozen] Mock<IPasswordService> mockPasswordService,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.Setup(p => p.GetPasswordResetCallback(It.IsAny<PasswordResetToken>()))
                .Returns(new Uri("http://www.test.com"));

            mockPasswordService.Setup(
                p => p.GeneratePasswordResetTokenAsync(It.Is<string>(e => e == expectedUser.Email)))
                .ReturnsAsync(new PasswordResetToken(expectedToken, expectedUser));

            await service.Create(
                expectedUser.PrimaryOrganisationId,
                expectedUser.FirstName,
                expectedUser.LastName,
                expectedUser.Email,
                OrganisationFunction.BuyerName);

            mockEmailService.Verify(e => e.SendEmailAsync(
                expectedUser.Email,
                settings.EmailTemplateId,
                It.IsAny<Dictionary<string, dynamic>>()));
        }

        private static AspNetUser CreateAspNetUser() => AspNetUserBuilder
                .Create()
                .WithFirstName("Test")
                .WithLastName("Smith")
                .WithPhoneNumber("0123456789")
                .WithEmailAddress("a.b@c.com")
                .WithPrimaryOrganisationId(1)
                .Build();
    }
}
