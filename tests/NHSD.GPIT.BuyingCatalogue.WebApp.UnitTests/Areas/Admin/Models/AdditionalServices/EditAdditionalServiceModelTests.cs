using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AdditionalServices
{
    public static class EditAdditionalServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void DetailsStatus_WithDetails_ReturnsCompleted(
            string additionalServiceName,
            string additionalServiceDescription,
            Solution solution,
            AdditionalService additionalService)
        {
            additionalService.CatalogueItem.Name = additionalServiceName;
            additionalService.FullDescription = additionalServiceDescription;

            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem);

            model.DetailsStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void DetailsStatus_WithNoDetails_ReturnsNotStarted(
            Solution solution,
            AdditionalService additionalService)
        {
            additionalService.CatalogueItem.Name = null;
            additionalService.FullDescription = null;

            var model = new EditAdditionalServiceModel(solution.CatalogueItem, additionalService.CatalogueItem);

            model.DetailsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void ListPriceStatus_WithCataloguePrices_ReturnsCompleted(
            List<CataloguePrice> cataloguePrices,
            CatalogueItem catalogueItem,
            CatalogueItem additionalService)
        {
            additionalService.CataloguePrices = cataloguePrices;

            var model = new EditAdditionalServiceModel(catalogueItem, additionalService);

            model.ListPriceStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void ListPriceStatus_WithNoCataloguePrices_ReturnsCompleted(
            CatalogueItem catalogueItem,
            CatalogueItem additionalService)
        {
            additionalService.CataloguePrices = new HashSet<CataloguePrice>();

            var model = new EditAdditionalServiceModel(catalogueItem, additionalService);

            model.ListPriceStatus().Should().Be(TaskProgress.NotStarted);
        }
    }
}
