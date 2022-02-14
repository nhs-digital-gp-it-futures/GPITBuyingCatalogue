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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.Services.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ProcurementHub
{
    public static class ProcurementHubServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ProcurementHubService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void ContactProcurementHub_RequestIsNull_ThrowsError(
            ProcurementHubService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ContactProcurementHub(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task ContactProcurementHub_RequestIsValid_SendsEmail(
            ProcurementHubRequest request,
            ProcurementHubMessageSettings settings,
            Mock<IGovNotifyEmailService> mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService
                .Setup(x => x.SendEmailAsync(settings.Recipient.Address, settings.TemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, x) => tokens = x)
                .Returns(Task.CompletedTask);

            var systemUnderTest = new ProcurementHubService(settings, mockEmailService.Object);

            await systemUnderTest.ContactProcurementHub(request);

            mockEmailService.VerifyAll();

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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task ContactProcurementHub_NoOdsCode_SendsEmail(
            string odsCodeValue,
            ProcurementHubRequest request,
            ProcurementHubMessageSettings settings,
            Mock<IGovNotifyEmailService> mockEmailService)
        {
            Dictionary<string, dynamic> tokens = null;

            mockEmailService
                .Setup(x => x.SendEmailAsync(settings.Recipient.Address, settings.TemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, x) => tokens = x)
                .Returns(Task.CompletedTask);

            var systemUnderTest = new ProcurementHubService(settings, mockEmailService.Object);

            request.OdsCode = odsCodeValue;

            await systemUnderTest.ContactProcurementHub(request);

            mockEmailService.VerifyAll();

            tokens.Should().NotBeNull();

            var odsCode = tokens.Should().ContainKey(ProcurementHubService.OrganisationOdsCodeToken).WhoseValue as string;

            odsCode.Should().Be(ProcurementHubService.OdsCodeNotSupplied);
        }
    }
}
