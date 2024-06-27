using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.Services.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ProcurementHub
{
    public static class ProcurementHubServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ProcurementHubService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void ContactProcurementHub_RequestIsNull_ThrowsError(
            ProcurementHubService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ContactProcurementHub(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static async Task ContactProcurementHub_RequestIsValid_SendsEmail(
            ProcurementHubRequest request,
            ProcurementHubMessageSettings settings,
            IGovNotifyEmailService mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService.SendEmailAsync(settings.Recipient.Address, settings.TemplateId, Arg.Do<Dictionary<string, dynamic>>(x => tokens = x)).Returns(Task.CompletedTask);

            var systemUnderTest = new ProcurementHubService(settings, mockEmailService);

            await systemUnderTest.ContactProcurementHub(request);

            await mockEmailService.Received().SendEmailAsync(settings.Recipient.Address, settings.TemplateId, Arg.Any<Dictionary<string, dynamic>>());

            tokens.Should().NotBeNull();

            var emailAddress = tokens.Should().ContainKey(ProcurementHubService.EmailAddressToken).WhoseValue as string;
            var fullName = tokens.Should().ContainKey(ProcurementHubService.FullNameToken).WhoseValue as string;
            var organisationName = tokens.Should().ContainKey(ProcurementHubService.OrganisationNameToken).WhoseValue as string;
            var odsCode = tokens.Should().ContainKey(ProcurementHubService.OrganisationOdsCodeToken).WhoseValue as string;
            var query = tokens.Should().ContainKey(ProcurementHubService.QueryToken).WhoseValue as string;

            emailAddress.Should().Be(request.Email);
            fullName.Should().Be(request.FullName);
            organisationName.Should().Be(request.OrganisationName);
            odsCode.Should().Be(request.OdsCode);
            query.Should().Be(request.Query);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static async Task ContactProcurementHub_NoOdsCode_SendsEmail(
            string odsCodeValue,
            ProcurementHubRequest request,
            ProcurementHubMessageSettings settings,
            IGovNotifyEmailService mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService.SendEmailAsync(settings.Recipient.Address, settings.TemplateId, Arg.Do<Dictionary<string, dynamic>>(x => tokens = x)).Returns(Task.CompletedTask);

            var systemUnderTest = new ProcurementHubService(settings, mockEmailService);

            request.OdsCode = odsCodeValue;

            await systemUnderTest.ContactProcurementHub(request);

            await mockEmailService.Received().SendEmailAsync(settings.Recipient.Address, settings.TemplateId, Arg.Any<Dictionary<string, dynamic>>());

            tokens.Should().NotBeNull();

            var odsCode = tokens.Should().ContainKey(ProcurementHubService.OrganisationOdsCodeToken).WhoseValue as string;

            odsCode.Should().Be(ProcurementHubService.OdsCodeNotSupplied);
        }
    }
}
