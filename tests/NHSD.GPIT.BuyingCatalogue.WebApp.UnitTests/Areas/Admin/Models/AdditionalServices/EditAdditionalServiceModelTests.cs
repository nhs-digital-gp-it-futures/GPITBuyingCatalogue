using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AdditionalServices
{
    public static class EditAdditionalServiceModelTests
    {
        [Theory]
        [MockAutoData]
        public static void DetailsStatus_WithDetails_ReturnsCompleted(
            string additionalServiceName,
            string additionalServiceDescription,
            Solution solution,
            AdditionalService additionalService)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            additionalService.CatalogueItem.Name = additionalServiceName;
            additionalService.FullDescription = additionalServiceDescription;

            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem);

            model.DetailsStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void DetailsStatus_WithNoDetails_ReturnsNotStarted(
            Solution solution,
            AdditionalService additionalService)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            additionalService.CatalogueItem.Name = null;
            additionalService.FullDescription = null;

            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem);

            model.DetailsStatus.Should().Be(TaskProgress.NotStarted);
        }
    }
}
