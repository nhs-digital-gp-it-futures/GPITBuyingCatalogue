using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
{
    public static class ThirdPartyComponentsModelTests
    {
        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var actual = new ThirdPartyComponentsModel(catalogueItem);

            actual.ThirdPartyComponents.Should().Be(solution.ApplicationTypeDetail.MobileThirdParty.ThirdPartyComponents);
            actual.DeviceCapabilities.Should().Be(solution.ApplicationTypeDetail.MobileThirdParty.DeviceCapabilities);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ThirdPartyComponentsModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
