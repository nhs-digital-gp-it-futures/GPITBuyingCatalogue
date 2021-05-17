using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HardwareRequirementsModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HardwareRequirementsModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeMobileHardwareRequirements = "Some hardware requirements" };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new HardwareRequirementsModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-mobile", model.BackLink);
            Assert.AreEqual("Some hardware requirements", model.Description);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new HardwareRequirementsModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Description);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("Some hardware requirements", true)]
        public static void IsCompleteIsCorrectlySet(string hardwareRequirements, bool? expected)
        {
            var clientApplication = new ClientApplication { NativeMobileHardwareRequirements = hardwareRequirements };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new HardwareRequirementsModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
