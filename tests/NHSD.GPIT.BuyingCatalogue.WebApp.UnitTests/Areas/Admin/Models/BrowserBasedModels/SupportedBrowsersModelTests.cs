using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.BrowserBasedModels
{
    public static class SupportedBrowsersModelTests
    {
        private static readonly SupportedBrowserModel[] ExpectedSupportedBrowsers =
        {
            new() { BrowserName = "Google Chrome" },
            new() { BrowserName = "Microsoft Edge" },
            new() { BrowserName = "Mozilla Firefox" },
            new() { BrowserName = "Opera" },
            new() { BrowserName = "Safari" },
            new() { BrowserName = "Chromium" },
            new() { BrowserName = "Internet Explorer 11" },
            new() { BrowserName = "Internet Explorer 10" },
        };

        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var actual = new SupportedBrowsersModel(solution.CatalogueItem);

            actual.Browsers.Should().BeEquivalentTo(ExpectedSupportedBrowsers, config => config
                .Excluding(m => m.Checked));

            actual.MobileResponsive.Should().Be(solution.GetClientApplication().MobileResponsive.ToYesNo());

            actual.BackLink.Should().Be("./");
            actual.BackLinkText.Should().Be("Go back");
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new SupportedBrowsersModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Fact]
        public static void DefaultConstructor_PropertiesCorrectlySet()
        {
            var model = new SupportedBrowsersModel();

            model.BackLinkText.Should().Be("Go back");
            model.BackLink.Should().Be("./");
        }
    }
}
