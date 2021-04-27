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
    internal static class PublicCloudModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new PublicCloudModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var hosting = new Hosting
            {                
                PublicCloud = new PublicCloud
                {                    
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
            
            var model = new PublicCloudModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            model.PublicCloud.Should().BeEquivalentTo(hosting.PublicCloud);
            Assert.True(model.RequiresHscnChecked);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new PublicCloudModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.PublicCloud);
            Assert.False(model.RequiresHscnChecked);
        }

        [Test]
        [TestCase(null, null, null, false)]
        [TestCase(null, null, null, false)]
        [TestCase(null, null, null, false)]
        [TestCase("", null, null, false)]
        [TestCase(" ", null, null, false)]
        [TestCase(null, "", null, false)]
        [TestCase(null, " ", null, false)]
        [TestCase(null, null, "", false)]
        [TestCase(null, null, " ", false)]        
        [TestCase("Link", null, null, true)]
        [TestCase(null, "Requires Hscn", null, true)]
        [TestCase(null, null, "Summary", true)]
        public static void IsCompleteIsCorrectlySet(string link, string requiresHscn, string summary, bool? expected )
        {
            var hosting = new Hosting
            {
                PublicCloud = new PublicCloud
                {                    
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

            var model = new PublicCloudModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }

        [Test]
        public static void RequiresHscnChecked_CorrectlySetsStringValue()
        {
            var hosting = new Hosting
            {
                PublicCloud = new PublicCloud()                
            };

            var json = JsonConvert.SerializeObject(hosting);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Hosting = json }
            };

            var model = new PublicCloudModel(catalogueItem);

            model.RequiresHscnChecked = false;
            Assert.Null(model.PublicCloud.RequiresHscn);
            model.RequiresHscnChecked = true;
            Assert.AreEqual("End user devices must be connected to HSCN/N3", model.PublicCloud.RequiresHscn);
        }
    }
}
