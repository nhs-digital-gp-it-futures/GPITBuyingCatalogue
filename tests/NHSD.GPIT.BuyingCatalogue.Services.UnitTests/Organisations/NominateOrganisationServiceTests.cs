using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class NominateOrganisationServiceTests
    {
        private const int UserId = 1;
        private const int OrganisationId = 1;
        private const string EmailAddress = "a@b.com";

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(NominateOrganisationService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void NominateOrganisation_RequestIsNull_ThrowsError(
            NominateOrganisationService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void NominateOrganisation_UsersServiceReturnsNull_ThrowsError(
            [Frozen] IUsersService mockUsersService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService.GetUser(UserId).Returns((AspNetUser)null);

            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, new NominateOrganisationRequest()))
                .Should().ThrowAsync<ArgumentOutOfRangeException>();

            mockUsersService.Received().GetUser(UserId);
        }

        [Theory]
        [MockAutoData]
        public static void NominateOrganisation_UsersServiceReturnsUserWithoutEmailAddress_ThrowsError(
            [Frozen] IUsersService mockUsersService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService.GetUser(UserId).Returns(new AspNetUser
                {
                    Email = string.Empty,
                });

            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, new NominateOrganisationRequest()))
                .Should().ThrowAsync<ArgumentException>();

            mockUsersService.Received().GetUser(UserId);
        }

        [Theory]
        [MockAutoData]
        public static void NominateOrganisation_OrganisationsServiceReturnsNull_ThrowsError(
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService.GetUser(UserId).Returns(new AspNetUser
                {
                    Email = EmailAddress,
                    PrimaryOrganisationId = OrganisationId,
                });

            mockOrganisationsService.GetOrganisation(OrganisationId).Returns((Organisation)null);

            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, new NominateOrganisationRequest()))
                .Should().ThrowAsync<ArgumentException>();

            mockUsersService.Received().GetUser(UserId);
            mockOrganisationsService.Received().GetOrganisation(OrganisationId);
        }

        [Theory]
        [MockAutoData]
        public static async Task NominateOrganisation_RequestIsValid_SendsEmails(
            AspNetUser user,
            Organisation organisation,
            NominateOrganisationRequest request,
            NominateOrganisationMessageSettings settings,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IGovNotifyEmailService mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            user.PrimaryOrganisationId = OrganisationId;

            mockUsersService.GetUser(UserId).Returns(user);

            mockOrganisationsService.GetOrganisation(OrganisationId).Returns(organisation);

            mockEmailService.SendEmailAsync(user.Email, settings.UserTemplateId, null).Returns(Task.CompletedTask);

            mockEmailService.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Do<Dictionary<string, dynamic>>(x => tokens = x)).Returns(Task.CompletedTask);

            var systemUnderTest = new NominateOrganisationService(
                settings,
                mockEmailService,
                mockOrganisationsService,
                mockUsersService);

            await systemUnderTest.NominateOrganisation(UserId, request);

            await mockUsersService.Received().GetUser(UserId);
            await mockOrganisationsService.Received().GetOrganisation(OrganisationId);
            await mockEmailService.Received().SendEmailAsync(user.Email, settings.UserTemplateId, null);
            await mockEmailService.Received().SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Any<Dictionary<string, dynamic>>());

            tokens.Should().NotBeNull();

            var emailAddress = tokens.Should().ContainKey(NominateOrganisationService.EmailAddressToken).WhoseValue as string;
            var fullName = tokens.Should().ContainKey(NominateOrganisationService.FullNameToken).WhoseValue as string;
            var userOrganisationName = tokens.Should().ContainKey(NominateOrganisationService.OrganisationNameToken).WhoseValue as string;
            var userOdsCode = tokens.Should().ContainKey(NominateOrganisationService.OrganisationOdsCodeToken).WhoseValue as string;
            var phoneNumber = tokens.Should().ContainKey(NominateOrganisationService.PhoneNumberToken).WhoseValue as string;
            var nominatedOrganisationName = tokens.Should().ContainKey(NominateOrganisationService.NominatedOrganisationNameToken).WhoseValue as string;
            var nominatedOrganisationOdsCode = tokens.Should().ContainKey(NominateOrganisationService.NominatedOrganisationOdsCodeToken).WhoseValue as string;

            emailAddress.Should().Be(user.Email);
            fullName.Should().Be(user.FullName);
            userOrganisationName.Should().Be(organisation.Name);
            userOdsCode.Should().Be(organisation.ExternalIdentifier);
            phoneNumber.Should().Be(user.PhoneNumber);
            nominatedOrganisationName.Should().Be(request.OrganisationName);
            nominatedOrganisationOdsCode.Should().Be(request.OdsCode);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static async Task NominateOrganisation_RequestIsValid_NoOdsCode_SendsEmails(
            string odsCodeValue,
            AspNetUser user,
            Organisation organisation,
            NominateOrganisationRequest request,
            NominateOrganisationMessageSettings settings,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            [Frozen] IGovNotifyEmailService mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            user.PrimaryOrganisationId = OrganisationId;

            mockUsersService.GetUser(UserId).Returns(user);

            mockOrganisationsService.GetOrganisation(OrganisationId).Returns(organisation);

            mockEmailService.SendEmailAsync(user.Email, settings.UserTemplateId, null).Returns(Task.CompletedTask);

            mockEmailService.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Do<Dictionary<string, dynamic>>(x => tokens = x)).Returns(Task.CompletedTask);

            var systemUnderTest = new NominateOrganisationService(
                settings,
                mockEmailService,
                mockOrganisationsService,
                mockUsersService);

            request.OdsCode = odsCodeValue;

            await systemUnderTest.NominateOrganisation(UserId, request);

            await mockUsersService.Received().GetUser(UserId);
            await mockOrganisationsService.Received().GetOrganisation(OrganisationId);
            await mockEmailService.Received().SendEmailAsync(user.Email, settings.UserTemplateId, null);
            await mockEmailService.Received().SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Any<Dictionary<string, dynamic>>());

            tokens.Should().NotBeNull();

            var odsCode = tokens.Should().ContainKey(NominateOrganisationService.NominatedOrganisationOdsCodeToken).WhoseValue as string;

            odsCode.Should().Be(NominateOrganisationService.OdsCodeNotSupplied);
        }

        [Theory]
        [MockAutoData]
        public static void IsGpPractice_UsersServiceReturnsNull_ThrowsError(
            int userId,
            [Frozen] IUsersService mockUsersService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService.GetUser(userId).Returns((AspNetUser)null);

            FluentActions
                .Awaiting(() => systemUnderTest.IsGpPractice(userId))
                .Should().ThrowAsync<ArgumentOutOfRangeException>();

            mockUsersService.Received().GetUser(userId);
        }

        [Theory]
        [MockAutoData]
        public static void IsGpPractice_UserPrimaryOrganisationNull_ThrowsError(
            int userId,
            [Frozen] IUsersService mockUsersService,
            AspNetUser user,
            NominateOrganisationService systemUnderTest)
        {
            user.PrimaryOrganisation = null;

            mockUsersService.GetUser(userId).Returns(user);

            FluentActions
                .Awaiting(() => systemUnderTest.IsGpPractice(userId))
                .Should().ThrowAsync<ArgumentException>();

            mockUsersService.Received().GetUser(userId);
        }

        [Theory]
        [MockAutoData]
        public static async Task IsGpPractice_ValidGPUser_ReturnsExpectedValue(
            int userId,
            [Frozen] IUsersService mockUsersService,
            AspNetUser user,
            Organisation primaryOrganisation,
            NominateOrganisationService systemUnderTest)
        {
            primaryOrganisation.OrganisationType = OrganisationType.GP;
            user.PrimaryOrganisation = primaryOrganisation;

            mockUsersService.GetUser(userId).Returns(user);

            var result = await systemUnderTest.IsGpPractice(userId);
            await mockUsersService.Received().GetUser(userId);
            result.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static async Task IsGpPractice_ValidNonGPUser_ReturnsExpectedValue(
            int userId,
            [Frozen] IUsersService mockUsersService,
            AspNetUser user,
            Organisation primaryOrganisation,
            NominateOrganisationService systemUnderTest)
        {
            primaryOrganisation.OrganisationType = OrganisationType.CCG;
            user.PrimaryOrganisation = primaryOrganisation;

            mockUsersService.GetUser(userId).Returns(user);

            var result = await systemUnderTest.IsGpPractice(userId);
            await mockUsersService.Received().GetUser(userId);
            result.Should().BeFalse();
        }
    }
}
