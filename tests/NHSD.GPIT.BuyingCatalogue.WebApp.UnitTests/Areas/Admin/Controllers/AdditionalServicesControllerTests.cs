using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AdditionalServicesControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Index_WithInvalidSolutionId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.Index(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Index_WithValidSolutionId_ReturnsModel(
            CatalogueItem catalogueItem,
            List<CatalogueItem> additionalServices,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            var expectedResult = new AdditionalServicesModel(catalogueItem, additionalServices);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalServicesBySolutionId(catalogueItem.Id))
                .ReturnsAsync(additionalServices);

            var result = await controller.Index(catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task EditAdditionalService_InvalidSolutionId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalService(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task EditAdditionalService_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalService(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task EditAdditionalService_WithValidIds_ReturnsModel(
            CatalogueItem catalogueItem,
            CatalogueItem additionalService,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            var expectedResult = new EditAdditionalServiceModel(catalogueItem, additionalService);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.Id))
                .ReturnsAsync(additionalService);

            var result = await controller.EditAdditionalService(catalogueItem.Id, additionalService.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedResult, opt => opt.Excluding(model => model.BackLink));
        }
    }
}
