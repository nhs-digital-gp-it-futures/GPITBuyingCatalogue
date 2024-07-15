using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ManageCatalogueSolutionModelTests
    {
        [Theory]
        [MockAutoData]
        public static void PublicationStatuses_Returns_AllStatuses(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solution.CatalogueItem
                .PublishedStatus
                .GetAvailablePublicationStatuses(solution.CatalogueItem.CatalogueItemType)
                .Select(p => new SelectOption<string>(p.Description(), p.EnumMemberName()))
                .ToList();

            var actual = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem).PublicationStatuses;

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static void StatusFeatures_Returns_FromSolutionLoadingStatusesModel(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solutionLoadingStatuses.Features;
            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            var actual = model.FeaturesStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void StatusDescription_Returns_FromSolutionLoadingStatusesModel(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solutionLoadingStatuses.Description;
            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            var actual = model.DescriptionStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void StatusImplementation_Returns_FromSolutionLoadingStatusesModel(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solutionLoadingStatuses.Implementation;
            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            var actual = model.ImplementationStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void StatusRoadmap_Returns_FromSolutionLoadingStatusesModel(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solutionLoadingStatuses.DevelopmentPlans;
            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            var actual = model.RoadmapStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void StatusInteroperability_Returns_FromSolutionLoadingStatusesModel(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solutionLoadingStatuses.Interoperability;
            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            var actual = model.InteroperabilityStatus;

            actual.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void StatusListPrice_Returns_FromSolutionLoadingStatusesModel(
            Solution solution,
            SolutionLoadingStatusesModel solutionLoadingStatuses)
        {
            var expected = solutionLoadingStatuses.ListPrice;
            var model = new ManageCatalogueSolutionModel(solutionLoadingStatuses, solution.CatalogueItem);

            var actual = model.ListPriceStatus;

            actual.Should().Be(expected);
        }
    }
}
