using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class InteroperabilityControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(InteroperabilityController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(InteroperabilityController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(InteroperabilityController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(InteroperabilityController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            catalogueItem.Solution.Integrations = JsonConvert.SerializeObject(integrations);

            mockService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Interoperability(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new InteroperabilityModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Interoperability(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Interoperability_Saves_And_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            InteroperabilityModel model,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller)
        {
            var actual = (await controller.Interoperability(catalogueItemId, model)).As<RedirectToActionResult>();

            mockInteroperabilityService.Verify(s => s.SaveIntegrationLink(catalogueItemId, model.Link));
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddIm1Integration_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            catalogueItem.Solution.Integrations = JsonConvert.SerializeObject(integrations);

            mockService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AddIm1Integration(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new AddIm1IntegrationModel());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddIm1Integration_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddIm1Integration(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddIm1Integration_Saves_And_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            AddIm1IntegrationModel model,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller)
        {
            var expectedIntegration = new Integration
            {
                IntegrationType = "IM1",
                IntegratesWith = model.IntegratesWith,
                Description = model.Description,
                Qualifier = model.SelectedIntegrationType,
                IsConsumer = model.SelectedProviderOrConsumer == "Consumer",
            };

            Integration savedIntegration = null;
            mockInteroperabilityService.Setup(s => s.AddIntegration(It.IsAny<CatalogueItemId>(), It.IsAny<Integration>()))
                .Callback<CatalogueItemId, Integration>((a1,a2) => { savedIntegration = a2; });

            var actual = (await controller.AddIm1Integration(catalogueItemId, model)).As<RedirectToActionResult>();

            mockInteroperabilityService.Verify(s => s.AddIntegration(catalogueItemId, It.IsAny<Integration>()));
            savedIntegration.Should().BeEquivalentTo(expectedIntegration);
            actual.ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }
    }
}
