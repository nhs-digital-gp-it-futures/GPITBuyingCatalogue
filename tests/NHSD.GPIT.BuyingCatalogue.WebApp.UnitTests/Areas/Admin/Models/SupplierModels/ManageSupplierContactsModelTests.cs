using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierModels
{
    public static class ManageSupplierContactsModelTests
    {
        [Fact]
        public static void Constructor_NullSupplierContact_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ManageSupplierContactsModel(null));

            actual.ParamName.Should().Be("supplier");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Supplier supplier)
        {
            var actual = new ManageSupplierContactsModel(supplier);

            actual.SupplierName.Should().Be(supplier.Name);
            actual.SupplierId.Should().Be(supplier.Id);
            actual.Contacts.Should().BeEquivalentTo(supplier.SupplierContacts);
        }
    }
}
