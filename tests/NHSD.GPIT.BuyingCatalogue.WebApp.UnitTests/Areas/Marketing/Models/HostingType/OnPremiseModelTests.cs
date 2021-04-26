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
    internal static class OnPremiseModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HybridModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var hosting = new Hosting
            {
                OnPremise = new OnPremise
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
            
            var model = new OnPremiseModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            model.OnPremise.Should().BeEquivalentTo(hosting.OnPremise);
            Assert.True(model.RequiresHscnChecked);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new OnPremiseModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.OnPremise);
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
        public static void IsCompleteIsCorrectlySet(string hostingModel, string link, string requiresHscn, string summary, bool? expected )
        {
            var hosting = new Hosting
            {
                OnPremise = new OnPremise
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

            var model = new OnPremiseModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
