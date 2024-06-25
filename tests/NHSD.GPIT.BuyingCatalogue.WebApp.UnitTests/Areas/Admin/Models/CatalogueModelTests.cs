using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class CatalogueModelTests
    {
        [Fact]
        public static void Constructor_CatalogueItemNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new CatalogueModel(null)).ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_SetsExpected_CatalogueItemId(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.CatalogueItemId.Should().Be(catalogueItem.Id.ToString());
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_SetsExpected_Name(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.CatalogueItemId.Should().Be(catalogueItem.Id.ToString());
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_ValidSupplierLastUpdated_SetAsLastUpdated(Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var expected = solution.LastUpdated;
            expected.Should().BeAfter(DateTime.MinValue);

            var actual = new CatalogueModel(catalogueItem);

            actual.LastUpdated.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_SolutionIsNull_SetMinValueAsLastUpdated(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = new CatalogueModel(catalogueItem);

            actual.LastUpdated.Should().Be(DateTime.MinValue);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_SetsExpected_PublishedStatus(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.PublishedStatus.Should().Be(catalogueItem.PublishedStatus);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_SupplierIsValid_SetsNameAsSupplier(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.Supplier.Should().Be(catalogueItem.Supplier.Name);
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_SupplierIsNull_SetsEmptyStringAsSupplier(CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier = null;

            var actual = new CatalogueModel(catalogueItem);

            actual.Supplier.Should().BeEmpty();
        }
    }
}
