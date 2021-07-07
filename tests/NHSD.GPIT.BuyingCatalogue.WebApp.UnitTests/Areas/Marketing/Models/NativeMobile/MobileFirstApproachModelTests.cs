using System;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
{
    public static class MobileFirstApproachModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new MobileFirstApproachModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeMobileFirstDesign = true };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new MobileFirstApproachModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/native-mobile", model.BackLink);
            Assert.Equal("Yes", model.MobileFirstApproach);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new MobileFirstApproachModel();

            Assert.Equal("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.MobileFirstApproach);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public static void IsCompleteIsCorrectlySet(bool? mobileFirstDesign, bool? expected)
        {
            var clientApplication = new ClientApplication { NativeMobileFirstDesign = mobileFirstDesign };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new MobileFirstApproachModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("     ", null)]
        [InlineData("Yes", true)]
        [InlineData("YES", true)]
        [InlineData("No", false)]
        public static void MobileFirstDesign_DifferentValuesForMobileFirstApproach_ResultAsExpected(
            string mobileFirstApproach,
            bool? expected)
        {
            var model = new MobileFirstApproachModel { MobileFirstApproach = mobileFirstApproach };

            model.MobileFirstDesign().Should().Be(expected);
        }
    }
}
