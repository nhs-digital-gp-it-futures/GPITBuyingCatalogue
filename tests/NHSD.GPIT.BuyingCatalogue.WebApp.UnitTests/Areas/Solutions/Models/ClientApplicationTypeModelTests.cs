using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ClientApplicationTypeModelTests
    {
        [Fact]
        public static void Constructor_NullClient_SetsDisplayFalse()
        {
            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ApplicationType.BrowserBased, null);

            Assert.False(model.DisplayApplicationType);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_BrowserBasedType_LoadsBrowserBasedDetails(
            ApplicationTypeDetail client,
            SupportedBrowser browser)
        {
            client.BrowsersSupported = new HashSet<SupportedBrowser>() { browser };

            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ApplicationType.BrowserBased, client);

            Assert.Equal("Browser-based application", model.Label);
            Assert.Equal("browser-based", model.DataTestTag);
            model.BrowsersSupported.Should().BeEquivalentTo(client.BrowsersSupported);
            Assert.True(model.DisplayApplicationType);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NativeMobileType_LoadsNativeMobileDetails(
            ApplicationTypeDetail client,
            string operatingSystem)
        {
            client.MobileOperatingSystems =
                new MobileOperatingSystems() { OperatingSystems = new HashSet<string>() { operatingSystem } };

            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ApplicationType.NativeMobile, client);

            Assert.Equal("Native mobile or tablet application", model.Label);
            Assert.Equal("native-mobile", model.DataTestTag);
            model.MobileOperatingSystems.Should().BeEquivalentTo(client.MobileOperatingSystems.OperatingSystems);
            Assert.True(model.DisplayApplicationType);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NativeDesktopType_LoadsNativeDesktopDetails(
            ApplicationTypeDetail client,
            string description)
        {
            client.NativeDesktopOperatingSystemsDescription = description;

            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ApplicationType.NativeDesktop, client);

            Assert.Equal("Native desktop application", model.Label);
            Assert.Equal("native-desktop", model.DataTestTag);
            Assert.Equal(description, model.OperatingSystemDescription);
            Assert.True(model.DisplayApplicationType);
        }
    }
}
