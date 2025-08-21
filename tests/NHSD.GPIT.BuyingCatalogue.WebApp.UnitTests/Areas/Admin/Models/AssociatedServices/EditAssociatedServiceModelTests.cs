using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class EditAssociatedServiceModelTests
    {
        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            AssociatedService associatedService)
        {
            var actual = new EditAssociatedServiceModel(supplier, catalogueItemId, associatedService.CatalogueItem);

            actual.SolutionId.Should().Be(catalogueItemId);
            actual.AssociatedServiceId.Should().Be(associatedService.CatalogueItemId);
            actual.AssociatedServiceName.Should().Be(associatedService.CatalogueItem.Name);
            actual.SelectedPublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
            actual.AssociatedServicePublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
        }

        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_RelatedServices_SetsRelatedServices(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            AssociatedService associatedService,
            List<Solution> relatedSolutions)
        {
            var expectedRelatedSolutions = relatedSolutions.Select(s => s.CatalogueItem).ToList();

            var actual = new EditAssociatedServiceModel(
                supplier,
                catalogueItemId,
                associatedService.CatalogueItem,
                expectedRelatedSolutions);

            actual.RelatedSolutions.Should().BeEquivalentTo(expectedRelatedSolutions);
        }
    }
}
