using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeDesktop
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ConnectivityModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ConnectivityModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeDesktopMinimumConnectionSpeed = "15Mbs", MinimumDesktopResolution = "21:9 - 3440 x 1440" };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem 
                { 
                    CatalogueItemId = "123",
                    Solution = new Solution { ClientApplication = json } 
                };
            
            var model = new ConnectivityModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-desktop", model.BackLink);
            Assert.AreEqual("15Mbs", model.SelectedConnectionSpeed);            
            model.ConnectionSpeeds.Should().BeEquivalentTo(GetConnectionSpeeds());            
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ConnectivityModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.SelectedConnectionSpeed);            
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("15Mbs", true)]
        public static void IsCompleteIsCorrectlySet(string minimumConnectionSpeed, bool? expected )
        {
            var clientApplication = new ClientApplication { NativeDesktopMinimumConnectionSpeed = minimumConnectionSpeed};
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new ConnectivityModel(catalogueItem);
            
            Assert.AreEqual(expected, model.IsComplete);
        }

        private static List<SelectListItem> GetConnectionSpeeds()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{ Text = "Please select"},
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
    }
}
