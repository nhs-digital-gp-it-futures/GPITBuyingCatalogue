using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
{
    public static class OperatingSystemsModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OperatingSystemsModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                MobileOperatingSystems = new MobileOperatingSystems
                {
                    OperatingSystems = new HashSet<string> { "Android", "Other" },
                    OperatingSystemsDescription = "A description",
                },
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new OperatingSystemsModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/native-mobile", model.BackLink);

            Assert.True(model.OperatingSystems.Single(s => s.OperatingSystemName == "Android").Checked);
            Assert.True(model.OperatingSystems.Single(s => s.OperatingSystemName == "Other").Checked);
            Assert.False(model.OperatingSystems.Single(s => s.OperatingSystemName == "Apple IOS").Checked);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new OperatingSystemsModel();

            Assert.Equal("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.Description);
            Assert.Null(model.OperatingSystems);
        }

        [Theory]
        [InlineData(false, null, false)]
        [InlineData(false, "", false)]
        [InlineData(false, " ", false)]
        [InlineData(false, "A description", false)]
        [InlineData(true, null, true)]
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
                    OperatingSystemsDescription = description,
                },
            };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new OperatingSystemsModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
