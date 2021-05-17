using System;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PrivateCloudModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new PrivateCloudModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var hosting = new Hosting
            {
                PrivateCloud = new PrivateCloud
                {
                    HostingModel = "A hosting model",
                    Link = "A link",
                    RequiresHscn = "End user devices must be connected to HSCN/N3",
                    Summary = "A summary"
                }
            };

            var json = JsonConvert.SerializeObject(hosting);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Hosting = json }
            };

            var model = new PrivateCloudModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            model.PrivateCloud.Should().BeEquivalentTo(hosting.PrivateCloud);
            Assert.True(model.RequiresHscnChecked);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new PrivateCloudModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.PrivateCloud);
            Assert.False(model.RequiresHscnChecked);
        }

        [Test]
        [TestCase(null, null, null, null, false)]
        [TestCase("", null, null, null, false)]
        [TestCase(" ", null, null, null, false)]
        [TestCase(null, "", null, null, false)]
        [TestCase(null, " ", null, null, false)]
        [TestCase(null, null, "", null, false)]
        [TestCase(null, null, " ", null, false)]
        [TestCase(null, null, null, "", false)]
        [TestCase(null, null, null, " ", false)]
        [TestCase("Hosting model", null, null, null, true)]
        [TestCase(null, "Link", null, null, true)]
        [TestCase(null, null, "Requires Hscn", null, true)]
        [TestCase(null, null, null, "Summary", true)]
        public static void IsCompleteIsCorrectlySet(string hostingModel, string link, string requiresHscn, string summary, bool? expected)
        {
            var hosting = new Hosting
            {
                PrivateCloud = new PrivateCloud
                {
                    HostingModel = hostingModel,
                    Link = link,
                    RequiresHscn = requiresHscn,
                    Summary = summary
                }
            };

            var json = JsonConvert.SerializeObject(hosting);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Hosting = json }
            };

            var model = new PrivateCloudModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }

        [Test]
        public static void RequiresHscnChecked_CorrectlySetsStringValue()
        {
            var hosting = new Hosting
            {
                PrivateCloud = new PrivateCloud()
            };

            var json = JsonConvert.SerializeObject(hosting);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Hosting = json }
            };

            var model = new PrivateCloudModel(catalogueItem)
            {
                RequiresHscnChecked = false
            };
            Assert.Null(model.PrivateCloud.RequiresHscn);
            model.RequiresHscnChecked = true;
            Assert.AreEqual("End user devices must be connected to HSCN/N3", model.PrivateCloud.RequiresHscn);
        }
    }
}
