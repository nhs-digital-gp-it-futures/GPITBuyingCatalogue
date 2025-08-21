using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class AddAssociatedServiceModelTests
    {
        [Theory]
        [MockAutoData]
        public static void AddAssociatedService_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            Supplier supplier)
        {
            var actual = new AddAssociatedServiceModel(supplier);

            actual.SupplierId.Should().Be(supplier.Id);
            actual.SupplierName.Should().Be(supplier.Name);
        }
    }
}
