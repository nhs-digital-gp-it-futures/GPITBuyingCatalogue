using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class HostingTypeSectionModelTests
    {
        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            [Frozen] CatalogueItem catalogueItem,
            HostingTypeSectionModel expected)
        {
            var actual = new HostingTypeSectionModel(catalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new HostingTypeSectionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
