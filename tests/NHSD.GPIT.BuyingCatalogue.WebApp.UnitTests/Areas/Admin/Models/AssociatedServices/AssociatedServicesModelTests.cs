using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class AssociatedServicesModelTests
    {
        [Theory]
        [MockAutoData]
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
                PracticeReorganisation = s.PracticeReorganisationType,
            }).ToList();

            var actual = new AssociatedServicesModel(
                catalogueItem,
                associatedServices.Select(a => a.CatalogueItem).ToList());

            actual.SelectableAssociatedServices.Should().BeEquivalentTo(expected);
            actual.SolutionMergerAndSplits.Should().BeEquivalentTo(new SolutionMergerAndSplitTypesModel(catalogueItem.Name, Array.Empty<PracticeReorganisationTypeEnum>()));
            actual.SolutionName.Should().Be(catalogueItem.Name);
            actual.SolutionId.Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
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
                PracticeReorganisation = s.PracticeReorganisationType,
            }).ToList();

            var actual = new AssociatedServicesModel(
                catalogueItem,
                associatedServices.Select(a => a.CatalogueItem).ToList());

            actual.SelectableAssociatedServices.Should().BeEquivalentTo(expected);
            actual.SolutionMergerAndSplits.Should().BeEquivalentTo(new SolutionMergerAndSplitTypesModel(catalogueItem.Name, associatedServices.Select(s => s.PracticeReorganisationType)));
            actual.SolutionName.Should().Be(catalogueItem.Name);
            actual.SolutionId.Should().Be(catalogueItem.Id);
        }
    }
}
