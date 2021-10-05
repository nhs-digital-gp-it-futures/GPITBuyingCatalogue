using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class AssociatedServicesModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(AssociatedServicesModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServices_ValidServices_ReturnsTrue(
            [Frozen] AssociatedService service,
            CatalogueItem catalogueItem)
        {
            service.CatalogueItem.AssociatedService = service;
            service.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            catalogueItem.Supplier.CatalogueItems.Add(service.CatalogueItem);

            var model = new AssociatedServicesModel(catalogueItem);

            model.Services.Count.Should().BeGreaterThan(0);
            model.HasServices().Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServices_NoService_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier.CatalogueItems.Clear();

            var model = new AssociatedServicesModel(catalogueItem);

            model.HasServices().Should().BeFalse();
        }
    }
}
