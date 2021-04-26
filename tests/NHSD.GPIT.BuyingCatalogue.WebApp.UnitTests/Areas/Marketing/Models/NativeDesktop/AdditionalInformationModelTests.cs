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
    internal static class AdditionalInformationModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AdditionalInformationModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeDesktopAdditionalInformation = "Some additional information" };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem 
                { 
                    CatalogueItemId = "123",                    
                    Solution = new EntityFramework.Models.BuyingCatalogue.Solution { ClientApplication = json }
            };
            
            var model = new AdditionalInformationModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-desktop", model.BackLink);
            Assert.AreEqual("Some additional information", model.AdditionalInformation);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new AdditionalInformationModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.AdditionalInformation);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("Some aditional information", true)]
        public static void IsCompleteIsCorrectlySet(string additionalInformation, bool? expected )
        {
            var clientApplication = new ClientApplication { NativeDesktopAdditionalInformation = additionalInformation };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new EntityFramework.Models.BuyingCatalogue.Solution { ClientApplication = json } };

            var model = new AdditionalInformationModel(catalogueItem);
            
            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
