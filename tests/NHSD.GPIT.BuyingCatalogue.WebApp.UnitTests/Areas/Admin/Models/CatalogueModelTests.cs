using System;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
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
        [CommonAutoData]
        public static void Constructor_SetsExpected_CatalogueItemId(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.CatalogueItemId.Should().Be(catalogueItem.CatalogueItemId.ToString());
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_SetsExpected_Name(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.CatalogueItemId.Should().Be(catalogueItem.CatalogueItemId.ToString());
        }
        
        [Theory]
        [CommonAutoData]
        public static void Constructor_ValidSupplierLastUpdated_SetAsLastUpdated(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Supplier.LastUpdated;
            expected.Should().BeAfter(DateTime.MinValue);
            
            var actual = new CatalogueModel(catalogueItem);

            actual.LastUpdated.Should().Be(expected);
        }
        
        [Theory]
        [CommonAutoData]
        public static void Constructor_SupplierIsNull_SetMinValueAsLastUpdated(CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier = null;
            
            var actual = new CatalogueModel(catalogueItem);

            actual.LastUpdated.Should().Be(DateTime.MinValue);
        }
        
        [Theory]
        [CommonAutoData]
        public static void Constructor_SetsExpected_PublishedStatus(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.PublishedStatus.Should().Be(catalogueItem.PublishedStatus.AsString(EnumFormat.DisplayName));
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_SetsExpected_PublishedStatusId(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.PublishedStatusId.Should().Be((int)catalogueItem.PublishedStatus);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_SupplierIsValid_SetsNameAsSupplier(CatalogueItem catalogueItem)
        {
            var actual = new CatalogueModel(catalogueItem);

            actual.Supplier.Should().Be(catalogueItem.Supplier.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_SupplierIsNull_SetsEmptyStringAsSupplier(CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier = null;
            
            var actual = new CatalogueModel(catalogueItem);

            actual.Supplier.Should().BeEmpty();
        }
    }
}
