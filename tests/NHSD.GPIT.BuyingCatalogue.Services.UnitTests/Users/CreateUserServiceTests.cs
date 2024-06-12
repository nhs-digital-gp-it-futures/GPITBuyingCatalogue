using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Builders;
using NSubstitute;
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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("\t")]
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
                    OrganisationFunction.Buyer.Name));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Create_SuccessfulApplicationUserValidation_ReturnsSuccess(
            string expectedToken,
            [Frozen] IPasswordResetCallback mockPasswordResetCallback,
            [Frozen] IPasswordService mockPasswordService,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.GetPasswordResetCallback(Arg.Any<PasswordResetToken>()).Returns(new Uri("http://www.test.com"));

            mockPasswordService.GeneratePasswordResetTokenAsync(Arg.Is<string>(e => e == expectedUser.Email))
                .Returns(new PasswordResetToken(expectedToken, expectedUser));

            var actual = await service.Create(
                1,
                "Test",
                "Smith",
                "a.b@c.com",
                OrganisationFunction.Buyer.Name);

            actual.Id.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("Authority")]
        [MockInMemoryDbInlineAutoData("Buyer")]
        [MockInMemoryDbInlineAutoData("AccountManager")]
        public static async Task Create_SuccessfulApplicationUserValidation_UserAddedToDbContext(
            string role,
            string expectedToken,
            [Frozen] IPasswordResetCallback mockPasswordResetCallback,
            [Frozen] IPasswordService mockPasswordService,
            [Frozen] UserManager<AspNetUser> userManager,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.GetPasswordResetCallback(Arg.Any<PasswordResetToken>()).Returns(new Uri("http://www.test.com"));

            mockPasswordService.GeneratePasswordResetTokenAsync(Arg.Is<string>(e => e == expectedUser.Email))
                .Returns(new PasswordResetToken(expectedToken, expectedUser));

            var result = await service.Create(
                expectedUser.PrimaryOrganisationId,
                expectedUser.FirstName,
                expectedUser.LastName,
                expectedUser.Email,
                role);

            var actual = await userManager.Users.Include(u => u.AspNetUserRoles).ThenInclude(r => r.Role).FirstAsync(u => u.Id == result.Id);

            actual.PrimaryOrganisationId.Should().Be(expectedUser.PrimaryOrganisationId);
            actual.FirstName.Should().Be(expectedUser.FirstName);
            actual.LastName.Should().Be(expectedUser.LastName);
            actual.Email.Should().Be(expectedUser.Email);
            actual.AspNetUserRoles.Select(r => r.Role).Should().Contain(x => x.Name == role);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Create_NewApplicationUser_SendsEmail(
            string expectedToken,
            [Frozen] RegistrationSettings settings,
            [Frozen] IPasswordResetCallback mockPasswordResetCallback,
            [Frozen] IPasswordService mockPasswordService,
            [Frozen] IGovNotifyEmailService mockEmailService,
            CreateUserService service)
        {
            var expectedUser = CreateAspNetUser();

            mockPasswordResetCallback.GetPasswordResetCallback(Arg.Any<PasswordResetToken>())
                .Returns(new Uri("http://www.test.com"));

            mockPasswordService.GeneratePasswordResetTokenAsync(Arg.Is<string>(e => e == expectedUser.Email))
                .Returns(new PasswordResetToken(expectedToken, expectedUser));

            await service.Create(
                expectedUser.PrimaryOrganisationId,
                expectedUser.FirstName,
                expectedUser.LastName,
                expectedUser.Email,
                OrganisationFunction.Buyer.Name);

            await mockEmailService.Received()
                .SendEmailAsync(
                    expectedUser.Email,
                    settings.EmailTemplateId,
                    Arg.Any<Dictionary<string, dynamic>>());
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
