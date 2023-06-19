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
        [CommonInlineAutoData(ApplicationType.BrowserBased)]
        [CommonInlineAutoData(ApplicationType.MobileTablet)]
        [CommonInlineAutoData(ApplicationType.Desktop)]
        public static void ClientApplicationTypes_IsUpdatedCorrectly(
            ApplicationType clientApplicationType,
            ApplicationTypeDetail clientApplication)
        {
            clientApplication.EnsureApplicationTypePresent(clientApplicationType);

            clientApplication.ClientApplicationTypes.Should().Contain(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [CommonAutoData]
        public static void HasClientApplicationType_True(
            ApplicationTypeDetail clientApplication)
        {
            clientApplication.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            clientApplication.HasApplicationType(ApplicationType.BrowserBased).Should().BeTrue();
        }

        [Fact]
        public static void HasClientApplicationType_False()
        {
            var clientApplication = new ApplicationTypeDetail();

            clientApplication.HasApplicationType(ApplicationType.BrowserBased).Should().BeFalse();
        }
    }
}
