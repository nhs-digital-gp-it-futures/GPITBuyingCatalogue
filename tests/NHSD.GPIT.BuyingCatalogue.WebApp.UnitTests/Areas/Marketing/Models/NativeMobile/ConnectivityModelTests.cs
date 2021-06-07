using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class ConnectivityModelModelTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            mapper = null;
        }

        [Test]
        public void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ConnectivityModel(null));
        }

        [Test]
        public void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                MobileConnectionDetails = new MobileConnectionDetails
                {
                    MinimumConnectionSpeed = "15Mbs",
                    ConnectionType = new HashSet<string> { "3G", "4G" },
                    Description = "A description"
                }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = mapper.Map<CatalogueItem, ConnectivityModel>(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-mobile", model.BackLink);
            Assert.AreEqual("15Mbs", model.SelectedConnectionSpeed);
            Assert.AreEqual("A description", model.Description);
            model.ConnectionSpeeds.Should().BeEquivalentTo(GetConnectionSpeeds());
            Assert.AreEqual(7, model.ConnectionTypes.Length);
            Assert.True(model.ConnectionTypes.Single(x => x.ConnectionType == "3G").Checked);
            Assert.True(model.ConnectionTypes.Single(x => x.ConnectionType == "4G").Checked);
            Assert.AreEqual(5, model.ConnectionTypes.Count(x => !x.Checked));
        }

        [Test]
        public void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ConnectivityModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.SelectedConnectionSpeed);
        }

        [Test]
        [TestCase(null, null, false, false)]
        [TestCase("", "", false, false)]
        [TestCase(" ", " ", false, false)]
        [TestCase("15Mbs", null, false, true)]
        [TestCase(null, "A description", false, true)]
        [TestCase(null, null, true, true)]
        public void IsCompleteIsCorrectlySet(string minimumConnectionSpeed, string description, bool hasConnectionType, bool? expected)
        {
            var clientApplication = new ClientApplication
            {
                MobileConnectionDetails = new MobileConnectionDetails
                {
                    MinimumConnectionSpeed = minimumConnectionSpeed,
                    ConnectionType = new HashSet<string>(),
                    Description = description
                }
            };

            if (hasConnectionType)
                clientApplication.MobileConnectionDetails.ConnectionType.Add("3G");

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = mapper.Map<CatalogueItem, ConnectivityModel>(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }

        private static List<SelectListItem> GetConnectionSpeeds() =>
            new()
            {
                new() { Text = "0.5Mbps", Value = "0.5Mbps" },
                new() { Text = "1Mbps", Value = "1Mbps" },
                new() { Text = "1.5Mbps", Value = "1.5Mbps" },
                new() { Text = "2Mbps", Value = "2Mbps" },
                new() { Text = "3Mbps", Value = "3Mbps" },
                new() { Text = "5Mbps", Value = "5Mbps" },
                new() { Text = "8Mbps", Value = "8Mbps" },
                new() { Text = "10Mbps", Value = "10Mbps" },
                new() { Text = "15Mbps", Value = "15Mbps" },
                new() { Text = "20Mbps", Value = "20Mbps" },
                new() { Text = "30Mbps", Value = "30Mbps" },
                new() { Text = "Higher than 30Mbps", Value = "Higher than 30Mbps" }
            };
    }
}
