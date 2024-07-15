using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierModels
{
    public static class EditSupplierDetailsModelTests
    {
        [Fact]
        public static void Constructor_NullSupplier_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditSupplierDetailsModel(null));

            actual.ParamName.Should().Be("supplier");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Supplier supplier)
        {
            var actual = new EditSupplierDetailsModel(supplier);

            actual.SupplierName.Should().Be(supplier.Name);
            actual.SupplierLegalName.Should().Be(supplier.LegalName);
            actual.AboutSupplier.Should().Be(supplier.Summary);
            actual.SupplierWebsite.Should().Be(supplier.SupplierUrl);
            actual.SupplierDisplayName.Should().Be(supplier.Name);
        }
    }
}
