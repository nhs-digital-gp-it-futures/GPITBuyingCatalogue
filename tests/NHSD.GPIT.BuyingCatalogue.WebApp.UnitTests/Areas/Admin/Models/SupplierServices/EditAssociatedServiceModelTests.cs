using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierServices
{
    public static class EditAssociatedServiceModelTests
    {
        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            Supplier supplier,
            AssociatedService associatedService,
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId)
        {
            var actual = new EditAssociatedServiceModel(supplier, associatedService.CatalogueItem)
            {
                SolutionType = SolutionType.GPIT,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
            };
            actual.SupplierId.Should().Be(supplier.Id);
            actual.SupplierName.Should().Be(supplier.Name);
            actual.AssociatedServiceId.Should().Be(associatedService.CatalogueItemId);
            actual.AssociatedServiceName.Should().Be(associatedService.CatalogueItem.Name);
            actual.SelectedPublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
            actual.AssociatedServicePublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
            actual.SolutionType.Should().Be(SolutionType.GPIT);
            actual.SolutionId.Should().Be(solutionId);
            actual.AdditionalServiceId.Should().Be(additionalServiceId);
        }

        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_RelatedServices_SetsRelatedServices(
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> relatedSolutions,
            List<AdditionalService> relatedAdditionalServices,
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId)
        {
            var solutions = relatedSolutions.Select(x => x.CatalogueItem)
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            var additionalServices = relatedAdditionalServices.Select(x => x.CatalogueItem)
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            var relatedCatalogueItems = solutions.Concat(additionalServices).ToList();

            associatedService.CatalogueItem.Solution = new Solution
            {
                SolutionType = SolutionType.GPIT,
            };

            var actual = new EditAssociatedServiceModel(
                supplier,
                associatedService.CatalogueItem,
                relatedCatalogueItems)
            {
                SolutionType = SolutionType.GPIT,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
            };
            actual.RelatedSolutions.Should().BeEquivalentTo(solutions.Select(x => x.Solution));
            actual.RelatedAdditionalServices.Should().BeEquivalentTo(additionalServices.Select(x => x.AdditionalService));
            actual.SolutionType.Should().Be(SolutionType.GPIT);
            actual.SolutionId.Should().Be(solutionId);
            actual.AdditionalServiceId.Should().Be(additionalServiceId);
        }
    }
}
