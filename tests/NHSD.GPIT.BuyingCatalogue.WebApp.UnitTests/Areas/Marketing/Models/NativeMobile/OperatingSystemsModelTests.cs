using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
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
            var clientApplication = new ClientApplication
            {
                MobileOperatingSystems = new MobileOperatingSystems
                {
                    OperatingSystems = new HashSet<string> { "Android", "Other" },                     
                    OperatingSystemsDescription = "A description"                    
                }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new EntityFramework.Models.BuyingCatalogue.Solution { ClientApplication = json }
            };

            var model = new OperatingSystemsModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-mobile", model.BackLink);
            
            Assert.True(model.OperatingSystems.Single(x => x.OperatingSystemName == "Android").Checked);
            Assert.True(model.OperatingSystems.Single(x => x.OperatingSystemName == "Other").Checked);
            Assert.False(model.OperatingSystems.Single(x => x.OperatingSystemName == "Apple IOS").Checked);            
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new OperatingSystemsModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.Description);
            Assert.Null(model.OperatingSystems);
        }

        [Test]
        [TestCase(false, null, false)]
        [TestCase(false, "", false)]
        [TestCase(false, " ", false)]
        [TestCase(false, "A description", false)]
        [TestCase(true, null, true)]
        public static void IsCompleteIsCorrectlySet(bool includeBrowser, string description, bool expected)
        {
            var operatingSystems = new HashSet<string>();

            if (includeBrowser)
                operatingSystems.Add("Other");

            var clientApplication = new ClientApplication
            {
                MobileOperatingSystems = new MobileOperatingSystems
                {
                    OperatingSystems = operatingSystems,
                    OperatingSystemsDescription = description
                }
            };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new EntityFramework.Models.BuyingCatalogue.Solution { ClientApplication = json }
            };

            var model = new OperatingSystemsModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
