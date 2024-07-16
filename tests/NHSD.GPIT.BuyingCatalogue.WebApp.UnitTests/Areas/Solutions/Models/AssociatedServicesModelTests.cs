using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        [MockAutoData]
        public static void HasServices_ValidServices_ReturnsTrue(
            AssociatedService service,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.SupplierServiceAssociations.Add(new SupplierServiceAssociation { AssociatedServiceId = service.CatalogueItemId });
            List<CatalogueItem> associatedServices = new List<CatalogueItem> { service.CatalogueItem };

            var model = new AssociatedServicesModel(catalogueItem, associatedServices, contentStatus);

            model.Services.Count.Should().Be(1);
            model.HasServices().Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void HasServices_NoService_ReturnsFalse(Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;

            var model = new AssociatedServicesModel(catalogueItem, new List<CatalogueItem>(), new CatalogueItemContentStatus());

            model.HasServices().Should().BeFalse();
        }
    }
}
