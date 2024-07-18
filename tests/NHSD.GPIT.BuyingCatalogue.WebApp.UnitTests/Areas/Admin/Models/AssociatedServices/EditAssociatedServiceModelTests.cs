using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class EditAssociatedServiceModelTests
    {
        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            Solution solution,
            AssociatedService associatedService)
        {
            var actual = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem);

            actual.SolutionId.Should().Be(solution.CatalogueItemId);
            actual.SolutionName.Should().Be(solution.CatalogueItem.Name);
            actual.AssociatedServiceId.Should().Be(associatedService.CatalogueItemId);
            actual.AssociatedServiceName.Should().Be(associatedService.CatalogueItem.Name);
            actual.SelectedPublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
            actual.AssociatedServicePublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
        }

        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_RelatedServices_SetsRelatedServices(
            Solution solution,
            AssociatedService associatedService,
            List<Solution> relatedSolutions)
        {
            var expectedRelatedSolutions = relatedSolutions.Select(s => s.CatalogueItem).ToList();

            var actual = new EditAssociatedServiceModel(
                solution.CatalogueItem,
                associatedService.CatalogueItem,
                expectedRelatedSolutions);

            actual.RelatedSolutions.Should().BeEquivalentTo(expectedRelatedSolutions);
        }
    }
}
