using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierModels
{
    public static class EditSupplierAddressModelTests
    {
        [Fact]
        public static void Constructor_NullSupplier_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditSupplierAddressModel(null));

            actual.ParamName.Should().Be("supplier");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Supplier supplier)
        {
            var actual = new EditSupplierAddressModel(supplier);

            actual.AddressLine1.Should().Be(supplier.Address.Line1);
            actual.AddressLine2.Should().Be(supplier.Address.Line2);
            actual.AddressLine3.Should().Be(supplier.Address.Line3);
            actual.AddressLine4.Should().Be(supplier.Address.Line4);
            actual.AddressLine5.Should().Be(supplier.Address.Line5);
            actual.Town.Should().Be(supplier.Address.Town);
            actual.County.Should().Be(supplier.Address.County);
            actual.PostCode.Should().Be(supplier.Address.Postcode);
            actual.Country.Should().Be(supplier.Address.Country);

            actual.SupplierName.Should().Be(supplier.Name);
        }
    }
}
