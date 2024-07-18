using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ApplicationTypeModelTests
    {
        [Fact]
        public static void Constructor_NullApplicationType_SetsDisplayFalse()
        {
            var model = new ApplicationTypeModel(ApplicationTypeModel.ApplicationType.BrowserBased, null);

            Assert.False(model.DisplayApplicationType);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_BrowserBasedType_LoadsBrowserBasedDetails(
            ApplicationTypeDetail applicationTypeDetail,
            SupportedBrowser browser)
        {
            applicationTypeDetail.BrowsersSupported = new HashSet<SupportedBrowser>() { browser };

            var model = new ApplicationTypeModel(ApplicationTypeModel.ApplicationType.BrowserBased, applicationTypeDetail);

            Assert.Equal("Browser-based application", model.Label);
            Assert.Equal("browser-based", model.DataTestTag);
            model.BrowsersSupported.Should().BeEquivalentTo(applicationTypeDetail.BrowsersSupported);
            Assert.True(model.DisplayApplicationType);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_NativeMobileType_LoadsNativeMobileDetails(
            ApplicationTypeDetail applicationTypeDetail,
            string operatingSystem)
        {
            applicationTypeDetail.MobileOperatingSystems =
                new MobileOperatingSystems() { OperatingSystems = new HashSet<string>() { operatingSystem } };

            var model = new ApplicationTypeModel(ApplicationTypeModel.ApplicationType.NativeMobile, applicationTypeDetail);

            Assert.Equal("Native mobile or tablet application", model.Label);
            Assert.Equal("native-mobile", model.DataTestTag);
            model.MobileOperatingSystems.Should().BeEquivalentTo(applicationTypeDetail.MobileOperatingSystems.OperatingSystems);
            Assert.True(model.DisplayApplicationType);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_NativeDesktopType_LoadsNativeDesktopDetails(
            ApplicationTypeDetail applicationTypeDetail,
            string description)
        {
            applicationTypeDetail.NativeDesktopOperatingSystemsDescription = description;

            var model = new ApplicationTypeModel(ApplicationTypeModel.ApplicationType.NativeDesktop, applicationTypeDetail);

            Assert.Equal("Native desktop application", model.Label);
            Assert.Equal("native-desktop", model.DataTestTag);
            Assert.Equal(description, model.OperatingSystemDescription);
            Assert.True(model.DisplayApplicationType);
        }
    }
}
