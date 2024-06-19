using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class AddAssociatedServiceModelTests
    {
        [Theory]
        [MockAutoData]
        public static void AddAssociatedService_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = new AddAssociatedServiceModel(catalogueItem);

            actual.SolutionId.Should().Be(catalogueItem.Id);
            actual.SupplierName.Should().Be(catalogueItem.Supplier.Name);
        }
    }
}
