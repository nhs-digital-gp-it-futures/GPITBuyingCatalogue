using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DesktopBasedModels
{
    public static class HardwareRequirementsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var actual = new HardwareRequirementsModel(catalogueItem);

            actual.Description.Should().Be(solution.ApplicationTypeDetail.NativeDesktopHardwareRequirements);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new HardwareRequirementsModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
