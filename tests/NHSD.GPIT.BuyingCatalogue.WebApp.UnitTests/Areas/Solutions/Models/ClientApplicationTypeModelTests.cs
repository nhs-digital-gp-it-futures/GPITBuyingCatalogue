﻿using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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
            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.BrowserBased, null);

            Assert.False(model.DisplayClientApplication);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_BrowserBasedType_LoadsBrowserBasedDetails(
            ClientApplication client,
            SupportedBrowser browser)
        {
            client.BrowsersSupported = new HashSet<SupportedBrowser>() { browser };

            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.BrowserBased, client);

            Assert.Equal("Browser-based application", model.Label);
            Assert.Equal("browser-based", model.DataTestTag);
            model.BrowsersSupported.Should().BeEquivalentTo(client.BrowsersSupported);
            Assert.True(model.DisplayClientApplication);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NativeMobileType_LoadsNativeMobileDetails(
            ClientApplication client,
            string operatingSystem)
        {
            client.MobileOperatingSystems =
                new MobileOperatingSystems() { OperatingSystems = new HashSet<string>() { operatingSystem } };

            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.NativeMobile, client);

            Assert.Equal("Native mobile or tablet application", model.Label);
            Assert.Equal("native-mobile", model.DataTestTag);
            model.MobileOperatingSystems.Should().BeEquivalentTo(client.MobileOperatingSystems.OperatingSystems);
            Assert.True(model.DisplayClientApplication);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_NativeDesktopType_LoadsNativeDesktopDetails(
            ClientApplication client,
            string description)
        {
            client.NativeDesktopOperatingSystemsDescription = description;

            var model = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.NativeDesktop, client);

            Assert.Equal("Native desktop application", model.Label);
            Assert.Equal("native-desktop", model.DataTestTag);
            Assert.Equal(description, model.OperatingSystemDescription);
            Assert.True(model.DisplayClientApplication);
        }
    }
}
