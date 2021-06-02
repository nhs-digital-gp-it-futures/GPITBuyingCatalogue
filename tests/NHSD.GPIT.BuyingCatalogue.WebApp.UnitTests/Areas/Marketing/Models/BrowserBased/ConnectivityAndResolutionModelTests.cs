using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class ConnectivityAndResolutionModelTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            mapper = null;
        }
        
        [Test]
        public void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                MinimumConnectionSpeed = "15Mbs", MinimumDesktopResolution = "21:9 - 3440 x 1440"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/browser-based", model.BackLink);
            Assert.AreEqual("15Mbs", model.SelectedConnectionSpeed);
            Assert.AreEqual("21:9 - 3440 x 1440", model.SelectedScreenResolution);
            model.ConnectionSpeeds.Should().BeEquivalentTo(GetConnectionSpeeds());
            model.ScreenResolutions.Should().BeEquivalentTo(GetResolutions());
        }

        [Test]
        public void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ConnectivityAndResolutionModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.SelectedConnectionSpeed);
            Assert.Null(model.SelectedScreenResolution);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("15Mbs", true)]
        public void IsCompleteIsCorrectlySet(string minimumConnectionSpeed, bool? expected)
        {
            var clientApplication = new ClientApplication { MinimumConnectionSpeed = minimumConnectionSpeed };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }

        private static List<SelectListItem> GetConnectionSpeeds()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{ Text = "0.5Mbps", Value="0.5Mbps"},
                new SelectListItem{ Text = "1Mbps", Value="1Mbps"},
                new SelectListItem{ Text = "1.5Mbps", Value="1.5Mbps"},
                new SelectListItem{ Text = "2Mbps", Value="2Mbps"},
                new SelectListItem{ Text = "3Mbps", Value="3Mbps"},
                new SelectListItem{ Text = "5Mbps", Value="5Mbps"},
                new SelectListItem{ Text = "8Mbps", Value="8Mbps"},
                new SelectListItem{ Text = "10Mbps", Value="10Mbps"},
                new SelectListItem{ Text = "15Mbps", Value="15Mbps"},
                new SelectListItem{ Text = "20Mbps", Value="20Mbps"},
                new SelectListItem{ Text = "30Mbps", Value="30Mbps"},
                new SelectListItem{ Text = "Higher than 30Mbps", Value="Higher than 30Mbps"}
            };
        }

        private static List<SelectListItem> GetResolutions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{ Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
                new SelectListItem{ Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
                new SelectListItem{ Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
                new SelectListItem{ Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
                new SelectListItem{ Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
                new SelectListItem{ Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
                new SelectListItem{ Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
                new SelectListItem{ Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
                new SelectListItem{ Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
                new SelectListItem{ Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
                new SelectListItem{ Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
                new SelectListItem{ Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
                new SelectListItem{ Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
                new SelectListItem{ Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
                new SelectListItem{ Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
                new SelectListItem{ Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
                new SelectListItem{ Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
                new SelectListItem{ Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
                new SelectListItem{ Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" }
            };
        }
    }
}
