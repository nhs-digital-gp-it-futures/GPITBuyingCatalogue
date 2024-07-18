using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.MobileTabletBasedModels
{
    public static class HardwareRequirementsModelTests
    {
        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var actual = new HardwareRequirementsModel(catalogueItem);

            actual.Description.Should().Be(solution.ApplicationTypeDetail.NativeMobileHardwareRequirements);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new HardwareRequirementsModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
