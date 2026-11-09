using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DesktopBasedModels
{
    public static class OperatingSystemsModelTests
    {
        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var actual = new OperatingSystemsModel(catalogueItem) { SolutionType = SolutionType.GPIT };

            actual.Description.Should().Be(solution.ApplicationTypeDetail.NativeDesktopOperatingSystemsDescription);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(()
                => new OperatingSystemsModel(null) { SolutionType = SolutionType.GPIT });

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
