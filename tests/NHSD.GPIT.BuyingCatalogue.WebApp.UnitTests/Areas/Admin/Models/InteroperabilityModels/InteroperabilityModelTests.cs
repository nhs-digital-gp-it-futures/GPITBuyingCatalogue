using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.InteroperabilityModels
{
    public static class InteroperabilityModelTests
    {
        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new InteroperabilityModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution,
            InteroperabilityModel expected)
        {
            var catalogueItem = solution.CatalogueItem;
            expected.SolutionName = catalogueItem.Name;
            expected.Link = solution.IntegrationsUrl;

            var actual = new InteroperabilityModel(catalogueItem);

            actual.CatalogueItemId.Should().Be(catalogueItem.Id);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
            actual.Link.Should().BeEquivalentTo(expected.Link);
        }
    }
}
