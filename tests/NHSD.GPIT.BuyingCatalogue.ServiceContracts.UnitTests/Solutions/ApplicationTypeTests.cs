using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ApplicationTypeTests
    {
        [Theory]
        [CommonInlineAutoData(ClientApplicationType.BrowserBased)]
        [CommonInlineAutoData(ClientApplicationType.MobileTablet)]
        [CommonInlineAutoData(ClientApplicationType.Desktop)]
        public static void ClientApplicationTypes_IsUpdatedCorrectly(
            ClientApplicationType clientApplicationType,
            ClientApplication clientApplication)
        {
            applicationTypes.EnsureApplicationTypePresent(applicationType);

            applicationTypes.ClientApplicationTypes.Should().Contain(applicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [CommonAutoData]
        public static void HasApplicationType_True(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            applicationTypes.HasApplicationType(ApplicationType.BrowserBased).Should().BeTrue();
        }

        [Fact]
        public static void HasApplicationType_False()
        {
            var applicationTypes = new ApplicationTypes();

            applicationTypes.HasApplicationType(ApplicationType.BrowserBased).Should().BeFalse();
        }
    }
}
