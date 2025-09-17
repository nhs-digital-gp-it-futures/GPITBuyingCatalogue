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
            AssociatedService associatedService)
        {
            var actual = new EditAssociatedServiceModel(supplier, associatedService.CatalogueItem);

            actual.SupplierId.Should().Be(supplier.Id);
            actual.SupplierName.Should().Be(supplier.Name);
            actual.AssociatedServiceId.Should().Be(associatedService.CatalogueItemId);
            actual.AssociatedServiceName.Should().Be(associatedService.CatalogueItem.Name);
            actual.SelectedPublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
            actual.AssociatedServicePublicationStatus.Should().Be(associatedService.CatalogueItem.PublishedStatus);
        }

        [Theory]
        [MockAutoData]
        public static void EditAssociatedServices_RelatedServices_SetsRelatedServices(
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> relatedSolutions,
            List<AdditionalService> relatedAdditionalServices)
        {
            var solutions = relatedSolutions.Select(x => x.CatalogueItem)
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            var additionalServices = relatedAdditionalServices.Select(x => x.CatalogueItem)
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            var relatedCatalogueItems = solutions.Concat(additionalServices).ToList();

            var actual = new EditAssociatedServiceModel(
                supplier,
                associatedService.CatalogueItem,
                relatedCatalogueItems);

            actual.RelatedSolutions.Should().BeEquivalentTo(solutions.Select(x => x.Solution));
            actual.RelatedAdditionalServices.Should().BeEquivalentTo(additionalServices.Select(x => x.AdditionalService));
        }
    }
}
