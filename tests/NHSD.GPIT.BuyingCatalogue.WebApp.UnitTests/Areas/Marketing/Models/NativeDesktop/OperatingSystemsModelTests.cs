using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeDesktop
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
            var clientApplication = new ClientApplication { NativeDesktopOperatingSystemsDescription = "Some operating system details" };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new OperatingSystemsModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/native-desktop", model.BackLink);
            Assert.Equal("Some operating system details", model.OperatingSystemsDescription);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new OperatingSystemsModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.OperatingSystemsDescription);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("Some operating system details", true)]
        public static void IsCompleteIsCorrectlySet(string operatingSystemsDescription, bool? expected)
        {
            var clientApplication = new ClientApplication { NativeDesktopOperatingSystemsDescription = operatingSystemsDescription };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new OperatingSystemsModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
