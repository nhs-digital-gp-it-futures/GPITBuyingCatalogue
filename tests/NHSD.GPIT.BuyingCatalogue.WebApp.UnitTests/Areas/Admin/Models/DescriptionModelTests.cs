using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class DescriptionModelTests
    {
        [Theory]
        [MockAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            Solution solution,
            DescriptionModel expected)
        {
            var catalogueItem = solution.CatalogueItem;

            expected.Summary = solution.Summary;
            expected.Description = solution.FullDescription;
            expected.Link = solution.AboutUrl;
            expected.SolutionName = catalogueItem.Name;

            var actual = new DescriptionModel(catalogueItem);

            actual.Summary.Should().BeEquivalentTo(expected.Summary);
            actual.Description.Should().BeEquivalentTo(expected.Description);
            actual.Link.Should().BeEquivalentTo(expected.Link);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DescriptionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
