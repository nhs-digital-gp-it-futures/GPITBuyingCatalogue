using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeDesktop
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OperatingSystemsModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OperatingSystemsModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeDesktopOperatingSystemsDescription = "Some operating system details" };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem 
                { 
                    CatalogueItemId = "123",
                    Solution = new Solution { ClientApplication = json } 
                };
            
            var model = new OperatingSystemsModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-desktop", model.BackLink);
            Assert.AreEqual("Some operating system details", model.OperatingSystemsDescription);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new OperatingSystemsModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.OperatingSystemsDescription);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("Some operating system details", true)]
        public static void IsCompleteIsCorrectlySet(string operatingSystemsDescription, bool? expected )
        {
            var clientApplication = new ClientApplication { NativeDesktopOperatingSystemsDescription = operatingSystemsDescription};
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new OperatingSystemsModel(catalogueItem);
            
            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
