using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    public static class RequestAccountServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(RequestAccountService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void NominateOrganisation_RequestIsNull_ThrowsError(
            RequestAccountService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.RequestAccount(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [CommonInlineAutoData(true, "Yes")]
        [CommonInlineAutoData(false, "No")]
        public static async Task NominateOrganisation_RequestIsValid_SendsEmails(
            bool hasGivenUserResearchConsent,
            string userResearchConsentToken,
            NewAccountDetails request,
            RequestAccountMessageSettings settings,
            Mock<IGovNotifyEmailService> mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService
                .Setup(x => x.SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null))
                .Returns(Task.CompletedTask);

            mockEmailService
                .Setup(x => x.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, x) => tokens = x)
                .Returns(Task.CompletedTask);

            var systemUnderTest = new RequestAccountService(mockEmailService.Object, settings);

            request.HasGivenUserResearchConsent = hasGivenUserResearchConsent;

            await systemUnderTest.RequestAccount(request);

            mockEmailService.VerifyAll();

            tokens.Should().NotBeNull();

            var emailAddress = tokens.Should().ContainKey(RequestAccountService.EmailAddressToken).WhoseValue as string;
            var fullName = tokens.Should().ContainKey(RequestAccountService.FullNameToken).WhoseValue as string;
            var organisationName = tokens.Should().ContainKey(RequestAccountService.OrganisationNameToken).WhoseValue as string;
            var odsCode = tokens.Should().ContainKey(RequestAccountService.OrganisationOdsCodeToken).WhoseValue as string;
            var userResearchConsent = tokens.Should().ContainKey(RequestAccountService.UserResearchConsentToken).WhoseValue as string;

            emailAddress.Should().Be(request.EmailAddress);
            fullName.Should().Be(request.FullName);
            organisationName.Should().Be(request.OrganisationName);
            odsCode.Should().Be(request.OdsCode);
            userResearchConsent.Should().Be(userResearchConsentToken);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task NominateOrganisation_NoOdsCode_SendsEmails(
            string odsCodeValue,
            NewAccountDetails request,
            RequestAccountMessageSettings settings,
            Mock<IGovNotifyEmailService> mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService
                .Setup(x => x.SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null))
                .Returns(Task.CompletedTask);

            mockEmailService
                .Setup(x => x.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, x) => tokens = x)
                .Returns(Task.CompletedTask);

            var systemUnderTest = new RequestAccountService(mockEmailService.Object, settings);

            request.OdsCode = odsCodeValue;

            await systemUnderTest.RequestAccount(request);

            mockEmailService.VerifyAll();

            tokens.Should().NotBeNull();

            var odsCode = tokens.Should().ContainKey(RequestAccountService.OrganisationOdsCodeToken).WhoseValue as string;

            odsCode.Should().Be(RequestAccountService.OdsCodeNotSupplied);
        }
    }
}
