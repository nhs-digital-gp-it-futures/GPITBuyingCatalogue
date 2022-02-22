using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(NominateOrganisationService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void NominateOrganisation_RequestIsNull_ThrowsError(
            NominateOrganisationService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void NominateOrganisation_UsersServiceReturnsNull_ThrowsError(
            [Frozen] Mock<IUsersService> mockUsersService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync((AspNetUser)null);

            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, new NominateOrganisationRequest()))
                .Should().ThrowAsync<ArgumentOutOfRangeException>();

            mockUsersService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static void NominateOrganisation_UsersServiceReturnsUserWithoutEmailAddress_ThrowsError(
            [Frozen] Mock<IUsersService> mockUsersService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser
                {
                    Email = string.Empty,
                });

            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, new NominateOrganisationRequest()))
                .Should().ThrowAsync<ArgumentException>();

            mockUsersService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static void NominateOrganisation_OrganisationsServiceReturnsNull_ThrowsError(
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            NominateOrganisationService systemUnderTest)
        {
            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser
                {
                    Email = EmailAddress,
                    PrimaryOrganisationId = OrganisationId,
                });

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(OrganisationId))
                .ReturnsAsync((Organisation)null);

            FluentActions
                .Awaiting(() => systemUnderTest.NominateOrganisation(UserId, new NominateOrganisationRequest()))
                .Should().ThrowAsync<ArgumentException>();

            mockUsersService.VerifyAll();
            mockOrganisationsService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task NominateOrganisation_RequestIsValid_SendsEmails(
            AspNetUser user,
            Organisation organisation,
            NominateOrganisationRequest request,
            NominateOrganisationMessageSettings settings,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            user.PrimaryOrganisationId = OrganisationId;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(OrganisationId))
                .ReturnsAsync(organisation);

            mockEmailService
                .Setup(x => x.SendEmailAsync(user.Email, settings.UserTemplateId, null))
                .Returns(Task.CompletedTask);

            mockEmailService
                .Setup(x => x.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, x) => tokens = x)
                .Returns(Task.CompletedTask);

            var systemUnderTest = new NominateOrganisationService(
                settings,
                mockEmailService.Object,
                mockOrganisationsService.Object,
                mockUsersService.Object);

            await systemUnderTest.NominateOrganisation(UserId, request);

            mockUsersService.VerifyAll();
            mockOrganisationsService.VerifyAll();
            mockEmailService.VerifyAll();

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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task NominateOrganisation_RequestIsValid_NoOdsCode_SendsEmails(
            string odsCodeValue,
            AspNetUser user,
            Organisation organisation,
            NominateOrganisationRequest request,
            NominateOrganisationMessageSettings settings,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            user.PrimaryOrganisationId = OrganisationId;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(user);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(OrganisationId))
                .ReturnsAsync(organisation);

            mockEmailService
                .Setup(x => x.SendEmailAsync(user.Email, settings.UserTemplateId, null))
                .Returns(Task.CompletedTask);

            mockEmailService
                .Setup(x => x.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, x) => tokens = x)
                .Returns(Task.CompletedTask);

            var systemUnderTest = new NominateOrganisationService(
                settings,
                mockEmailService.Object,
                mockOrganisationsService.Object,
                mockUsersService.Object);

            request.OdsCode = odsCodeValue;

            await systemUnderTest.NominateOrganisation(UserId, request);

            mockUsersService.VerifyAll();
            mockOrganisationsService.VerifyAll();
            mockEmailService.VerifyAll();

            tokens.Should().NotBeNull();

            var odsCode = tokens.Should().ContainKey(NominateOrganisationService.NominatedOrganisationOdsCodeToken).WhoseValue as string;

            odsCode.Should().Be(NominateOrganisationService.OdsCodeNotSupplied);
        }
    }
}
