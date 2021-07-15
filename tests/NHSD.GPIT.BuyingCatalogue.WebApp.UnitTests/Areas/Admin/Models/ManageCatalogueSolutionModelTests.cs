using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
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
            var expected = new FeaturesModel().FromCatalogueItem(catalogueItem).StatusFeatures();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.StatusFeatures();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusFeaturesColor_Returns_FromFeaturesModel(CatalogueItem catalogueItem)
        {
            var expected = new FeaturesModel().FromCatalogueItem(catalogueItem).StatusFeaturesColor();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.StatusFeaturesColor();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusDescription_Returns_FromDescriptionModel(CatalogueItem catalogueItem)
        {
            var expected = new DescriptionModel(catalogueItem).StatusDescription();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.StatusDescription();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusDescriptionColor_Returns_FromDescriptionModel(CatalogueItem catalogueItem)
        {
            var expected = new DescriptionModel(catalogueItem).StatusDescriptionColor();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.StatusDescriptionColor();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusImplementation_Returns_FromImplementationTimescaleModel(CatalogueItem catalogueItem)
        {
            var expected = new ImplementationTimescaleModel(catalogueItem).StatusImplementation();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.StatusImplementation();

            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusImplementationColor_Returns_FromImplementationTimescaleModel(CatalogueItem catalogueItem)
        {
            var expected = new ImplementationTimescaleModel(catalogueItem).StatusImplementationColor();
            var model = new ManageCatalogueSolutionModel { Solution = catalogueItem };

            var actual = model.StatusImplementationColor();

            actual.Should().Be(expected);
        }
    }
}
