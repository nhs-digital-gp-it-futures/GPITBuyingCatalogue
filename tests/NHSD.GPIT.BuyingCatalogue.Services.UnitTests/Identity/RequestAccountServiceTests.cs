using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    public static class RequestAccountServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(RequestAccountService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void NominateOrganisation_RequestIsNull_ThrowsError(
            RequestAccountService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.RequestAccount(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInlineAutoData(true, "Yes")]
        [MockInlineAutoData(false, "No")]
        public static async Task NominateOrganisation_RequestIsValid_SendsEmails(
            bool hasGivenUserResearchConsent,
            string userResearchConsentToken,
            NewAccountDetails request,
            RequestAccountMessageSettings settings,
            IGovNotifyEmailService mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService.SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null).Returns(Task.CompletedTask);

            mockEmailService.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Do<Dictionary<string, dynamic>>(x => tokens = x)).Returns(Task.CompletedTask);

            var systemUnderTest = new RequestAccountService(mockEmailService, settings);

            request.HasGivenUserResearchConsent = hasGivenUserResearchConsent;

            await systemUnderTest.RequestAccount(request);

            await mockEmailService.Received().SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null);
            await mockEmailService.Received().SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Any<Dictionary<string, dynamic>>());

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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static async Task NominateOrganisation_NoOdsCode_SendsEmails(
            string odsCodeValue,
            NewAccountDetails request,
            RequestAccountMessageSettings settings,
            IGovNotifyEmailService mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService.SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null).Returns(Task.CompletedTask);

            mockEmailService.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Do<Dictionary<string, dynamic>>(x => tokens = x)).Returns(Task.CompletedTask);

            var systemUnderTest = new RequestAccountService(mockEmailService, settings);

            request.OdsCode = odsCodeValue;

            await systemUnderTest.RequestAccount(request);

            await mockEmailService.Received().SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null);
            await mockEmailService.Received().SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, Arg.Any<Dictionary<string, dynamic>>());

            tokens.Should().NotBeNull();

            var odsCode = tokens.Should().ContainKey(RequestAccountService.OrganisationOdsCodeToken).WhoseValue as string;

            odsCode.Should().Be(RequestAccountService.OdsCodeNotSupplied);
        }
    }
}
