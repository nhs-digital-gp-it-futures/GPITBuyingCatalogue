using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ClientApplicationTests
    {
        [Theory]
        [CommonInlineAutoData(ClientApplicationType.BrowserBased)]
        [CommonInlineAutoData(ClientApplicationType.MobileTablet)]
        [CommonInlineAutoData(ClientApplicationType.Desktop)]
        public static void ClientApplicationTypes_IsUpdatedCorrectly(
            ClientApplicationType clientApplicationType,
            ClientApplication clientApplication)
        {
            clientApplication.EnsureClientApplicationTypePresent(clientApplicationType);

            clientApplication.ClientApplicationTypes.Should().Contain(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [CommonAutoData]
        public static void HasClientApplicationType_True(
            ClientApplication clientApplication)
        {
            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.BrowserBased);

            clientApplication.HasClientApplicationType(ClientApplicationType.BrowserBased).Should().BeTrue();
        }

        [Fact]
        public static void HasClientApplicationType_False()
        {
            var clientApplication = new ClientApplication();

            clientApplication.HasClientApplicationType(ClientApplicationType.BrowserBased).Should().BeFalse();
        }
    }
}
