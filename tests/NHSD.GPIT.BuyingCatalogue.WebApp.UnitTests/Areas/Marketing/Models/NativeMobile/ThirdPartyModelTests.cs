using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ThirdPartyModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ThirdPartyModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                MobileThirdParty = new MobileThirdParty { ThirdPartyComponents = "Third party components", DeviceCapabilities = "Device capabilities" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json }
            };

            var model = new ThirdPartyModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/1-123/section/native-mobile", model.BackLink);
            Assert.AreEqual("Third party components", model.ThirdPartyComponents);
            Assert.AreEqual("Device capabilities", model.DeviceCapabilities);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ThirdPartyModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.ThirdPartyComponents);
            Assert.Null(model.DeviceCapabilities);
        }

        [Test]
        [TestCase(null, null, false)]
        [TestCase("", null, false)]
        [TestCase(" ", null, false)]
        [TestCase(null, "", false)]
        [TestCase(null, " ", false)]
        [TestCase("Third party", null, true)]
        [TestCase(null, "Device capability", true)]
        public static void IsCompleteIsCorrectlySet(string thirdPary, string deviceCapabilities, bool? expected)
        {
            var clientApplication = new ClientApplication
            {
                MobileThirdParty = new MobileThirdParty { ThirdPartyComponents = thirdPary, DeviceCapabilities = deviceCapabilities }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new ThirdPartyModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
