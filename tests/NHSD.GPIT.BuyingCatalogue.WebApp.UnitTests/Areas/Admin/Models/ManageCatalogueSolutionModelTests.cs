using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ManageCatalogueSolutionModelTests
    {
        [Fact]
        public static void PublicationStatuses_Returns_AllStatuses()
        {
            var actual = new ManageCatalogueSolutionModel().PublicationStatuses;

            actual.Should().BeEquivalentTo(EnumsNET.Enums.GetValues<PublicationStatus>());
        }

        [Theory]
        [CommonAutoData]
        public static void StatusFeatures_Returns_FromFeaturesModel(CatalogueItem catalogueItem)
        {
            var expected = new FeaturesModel().FromCatalogueItem(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.FeaturesStatus();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusDescription_Returns_FromDescriptionModel(CatalogueItem catalogueItem)
        {
            var expected = new DescriptionModel(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.DescriptionStatus();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusImplementation_Returns_FromImplementationTimescaleModel(CatalogueItem catalogueItem)
        {
            var expected = new ImplementationTimescaleModel(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.ImplementationStatus();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusRoadmap_Returns_FromRoadmapModel(CatalogueItem catalogueItem)
        {
            var expected = new RoadmapModel().FromCatalogueItem(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.RoadmapStatus();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusInteroperability_Returns_FromInteroperabilityModel(CatalogueItem catalogueItem)
        {
            var expected = new InteroperabilityModel(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.InteroperabilityStatus();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusListPrice_Returns_FromManageListPricesModel(CatalogueItem catalogueItem)
        {
            var expected = new ManageListPricesModel(catalogueItem).Status();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.ListPriceStatus();

            actual.Should().Be(expected);
        }
    }
}
