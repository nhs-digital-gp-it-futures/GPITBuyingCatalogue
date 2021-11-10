using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class AssociatedServicesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void AssociatedServices_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            CatalogueItem catalogueItem,
            List<AssociatedService> associatedServices)
        {
            var expected = associatedServices.Select(s => new SelectableAssociatedService
            {
                Name = s.CatalogueItem.Name,
                Description = s.Description,
                PublishedStatus = s.CatalogueItem.PublishedStatus,
                CatalogueItemId = s.CatalogueItemId,
                Selected = false,
            }).ToList();

            var actual = new AssociatedServicesModel(
                catalogueItem,
                associatedServices.Select(a => a.CatalogueItem).ToList());

            actual.SelectableAssociatedServices.Should().BeEquivalentTo(expected);
            actual.Solution.Should().Be(catalogueItem);
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}");
        }

        [Theory]
        [CommonAutoData]
        public static void AssociatedServices_ValidCatalogueItem_RelatedServices_PropertiesSetAsExpected(
           CatalogueItem catalogueItem,
           List<AssociatedService> associatedServices)
        {
            catalogueItem.SupplierServiceAssociations.Clear();
            catalogueItem.SupplierServiceAssociations = associatedServices.Select(s => new SupplierServiceAssociation
            {
                CatalogueItemId = catalogueItem.Id,
                AssociatedServiceId = s.CatalogueItem.Id,
            }).ToList();

            var expected = associatedServices.Select(s => new SelectableAssociatedService
            {
                Name = s.CatalogueItem.Name,
                Description = s.Description,
                PublishedStatus = s.CatalogueItem.PublishedStatus,
                CatalogueItemId = s.CatalogueItemId,
                Selected = true,
            }).ToList();

            var actual = new AssociatedServicesModel(
                catalogueItem,
                associatedServices.Select(a => a.CatalogueItem).ToList());

            actual.SelectableAssociatedServices.Should().BeEquivalentTo(expected);
            actual.Solution.Should().Be(catalogueItem);
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItem.Id}");
        }
    }
}
