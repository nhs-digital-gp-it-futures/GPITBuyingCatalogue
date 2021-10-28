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
            AssociatedService service,
            Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.Supplier.CatalogueItems.Add(service.CatalogueItem);

            var model = new AssociatedServicesModel(catalogueItem);

            model.Services.Count.Should().BeGreaterThan(0);
            model.HasServices().Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServices_NoService_ReturnsFalse(Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.Supplier.CatalogueItems.Clear();

            var model = new AssociatedServicesModel(catalogueItem);

            model.HasServices().Should().BeFalse();
        }
    }
}
