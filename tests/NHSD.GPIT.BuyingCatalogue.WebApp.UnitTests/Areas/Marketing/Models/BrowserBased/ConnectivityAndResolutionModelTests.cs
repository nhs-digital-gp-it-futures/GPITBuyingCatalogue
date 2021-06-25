using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    public sealed class ConnectivityAndResolutionModelTests : IDisposable
    {
        private IMapper mapper;

        public ConnectivityAndResolutionModelTests()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        public void Dispose()
        {
            mapper = null;
        }

        [Fact]
        public void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                MinimumConnectionSpeed = "15Mbs",
                MinimumDesktopResolution = "21:9 - 3440 x 1440",
            };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/browser-based", model.BackLink);
            Assert.Equal("15Mbs", model.SelectedConnectionSpeed);
            Assert.Equal("21:9 - 3440 x 1440", model.SelectedScreenResolution);
            model.ConnectionSpeeds.Should().BeEquivalentTo(GetConnectionSpeeds());
            model.ScreenResolutions.Should().BeEquivalentTo(GetResolutions());
        }

        [Fact]
        public void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ConnectivityAndResolutionModel();

            Assert.Equal("./", model.BackLink);
            Assert.Null(model.SelectedConnectionSpeed);
            Assert.Null(model.SelectedScreenResolution);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("15Mbs", true)]
        public void IsCompleteIsCorrectlySet(string minimumConnectionSpeed, bool? expected)
        {
            var clientApplication = new ClientApplication { MinimumConnectionSpeed = minimumConnectionSpeed };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }

        private static IEnumerable<SelectListItem> GetConnectionSpeeds()
        {
            return new List<SelectListItem>
            {
                new() { Text = "0.5Mbps", Value="0.5Mbps"},
                new() { Text = "1Mbps", Value="1Mbps"},
                new() { Text = "1.5Mbps", Value="1.5Mbps"},
                new() { Text = "2Mbps", Value="2Mbps"},
                new() { Text = "3Mbps", Value="3Mbps"},
                new() { Text = "5Mbps", Value="5Mbps"},
                new() { Text = "8Mbps", Value="8Mbps"},
                new() { Text = "10Mbps", Value="10Mbps"},
                new() { Text = "15Mbps", Value="15Mbps"},
                new() { Text = "20Mbps", Value="20Mbps"},
                new() { Text = "30Mbps", Value="30Mbps"},
                new() { Text = "Higher than 30Mbps", Value="Higher than 30Mbps"},
            };
        }

        private static IEnumerable<SelectListItem> GetResolutions()
        {
            return new List<SelectListItem>
            {
                new() { Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
                new() { Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
                new() { Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
                new() { Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
                new() { Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
                new() { Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
                new() { Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
                new() { Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
                new() { Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
                new() { Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
                new() { Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
                new() { Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
                new() { Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
                new() { Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
                new() { Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
                new() { Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
                new() { Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
                new() { Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
                new() { Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" },
            };
        }
    }
}
