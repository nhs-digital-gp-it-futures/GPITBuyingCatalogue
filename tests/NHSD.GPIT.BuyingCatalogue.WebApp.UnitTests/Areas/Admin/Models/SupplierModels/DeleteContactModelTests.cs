using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierModels
{
    public static class DeleteContactModelTests
    {
        [Fact]
        public static void Constructor_NullSupplierContact_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DeleteContactModel(null, string.Empty));

            actual.ParamName.Should().Be("supplierContact");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            SupplierContact supplierContact,
            string supplierName)
        {
            var actual = new DeleteContactModel(supplierContact, supplierName);

            actual.SupplierId.Should().Be(supplierContact.SupplierId);
            actual.ContactId.Should().Be(supplierContact.Id);
            actual.FirstName.Should().Be(supplierContact.FirstName);
            actual.LastName.Should().Be(supplierContact.LastName);
            actual.SupplierName.Should().Be(supplierName);
        }
    }
}
