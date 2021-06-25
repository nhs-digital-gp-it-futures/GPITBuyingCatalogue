using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
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
    internal static class AdditionalInformationModelTests
    {
        [Test]
        public static void AdditionalInformation_StringLengthAttribute_ExpectedValue()
        {
            typeof(AdditionalInformationModel)
                .GetProperty(nameof(AdditionalInformationModel.AdditionalInformation))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should().Be(500);
        }

        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AdditionalInformationModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeMobileAdditionalInformation = "Some additional information" };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new AdditionalInformationModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/1-123/section/native-mobile", model.BackLink);
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
        public static void IsCompleteIsCorrectlySet(string additionalInformation, bool? expected)
        {
            var clientApplication = new ClientApplication { NativeMobileAdditionalInformation = additionalInformation };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new AdditionalInformationModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
