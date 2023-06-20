using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ClientApplicationTypeSectionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            ApplicationTypeSectionModel expected)
        {
            // CatalogueItem must be frozen so that the same instance is used to construct the Solution and
            // ClientApplicationTypeSectionModel instances
            _ = catalogueItem;
            var actual = new ApplicationTypeSectionModel(solution.CatalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ApplicationTypeSectionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
