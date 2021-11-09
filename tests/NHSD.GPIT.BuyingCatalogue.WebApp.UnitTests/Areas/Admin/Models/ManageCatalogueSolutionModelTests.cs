using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ManageCatalogueSolutionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void PublicationStatuses_Returns_AllStatuses(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var expected = solution.CatalogueItem
                .PublishedStatus
                .GetAvailablePublicationStatuses(solution.CatalogueItem.CatalogueItemType)
                .Select(p => new SelectListItem(p.Description(), p.EnumMemberName()))
                .ToList();

            var actual = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList()).PublicationStatuses;

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusFeatures_Returns_FromFeaturesModel(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var expected = new FeaturesModel().FromCatalogueItem(solution.CatalogueItem).Status();
            var model = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList());

            var actual = model.FeaturesStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusDescription_Returns_FromDescriptionModel(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var expected = new DescriptionModel(solution.CatalogueItem).Status();
            var model = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList());

            var actual = model.DescriptionStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusImplementation_Returns_FromImplementationTimescaleModel(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var expected = new ImplementationTimescaleModel(solution.CatalogueItem).Status();
            var model = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList());

            var actual = model.ImplementationStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusRoadmap_Returns_FromRoadmapModel(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var expected = new RoadmapModel().FromCatalogueItem(solution.CatalogueItem).Status();
            var model = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList());

            var actual = model.RoadmapStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusInteroperability_Returns_FromInteroperabilityModel(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var catalogueItem = solution.CatalogueItem;

            var expected = new InteroperabilityModel(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList());

            var actual = model.InteroperabilityStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusListPrice_Returns_FromManageListPricesModel(
            Solution solution,
            List<AdditionalService> additionalServices,
            List<AssociatedService> associatedServices)
        {
            var expected = new ManageListPricesModel(solution.CatalogueItem).Status();
            var model = new ManageCatalogueSolutionModel(
                solution.CatalogueItem,
                additionalServices.Select(a => a.CatalogueItem).ToList(),
                associatedServices.Select(a => a.CatalogueItem).ToList());

            var actual = model.ListPriceStatus;

            actual.Should().Be(expected);
        }
    }
}
