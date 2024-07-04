using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class InteroperabilityControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(InteroperabilityController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Interoperability_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<SolutionIntegration> integrations,
            List<IntegrationType> integrationTypes,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            integrations.Zip(integrationTypes).ToList().ForEach(zipped => zipped.First.IntegrationType = zipped.Second);

            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = integrations;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var actual = (await controller.Interoperability(catalogueItem.Id)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItem.Id);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new InteroperabilityModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Interoperability_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.Interoperability(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Interoperability_Saves_And_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            InteroperabilityModel model,
            [Frozen] IInteroperabilityService mockInteroperabilityService,
            InteroperabilityController controller)
        {
            var actual = (await controller.Interoperability(catalogueItemId, model)).As<RedirectToActionResult>();

            await mockInteroperabilityService.Received().SaveIntegrationLink(catalogueItemId, model.Link);
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddIm1Integration_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<IntegrationType> integrationTypes,
            [Frozen] ISolutionsService mockService,
            [Frozen] IIntegrationsService integrationsService,
            InteroperabilityController controller)
        {
            var catalogueItem = solution.CatalogueItem;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);
            integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.Im1).Returns(integrationTypes);

            var actual = (await controller.AddIm1Integration(catalogueItem.Id)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItem.Id);
            actual.ViewName.Should().Be("AddEditIm1Integration");

            var expectedModel = new AddEditIm1IntegrationModel(catalogueItem, integrationTypes);

            actual.Model.Should().BeEquivalentTo(expectedModel, opt =>
                opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddIm1Integration_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.AddIm1Integration(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddIm1Integration_Saves_And_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            AddEditIm1IntegrationModel model,
            [Frozen] IInteroperabilityService mockInteroperabilityService,
            InteroperabilityController controller)
        {
            var actual = (await controller.AddIm1Integration(catalogueItemId, model)).As<RedirectToActionResult>();

            await mockInteroperabilityService.Received().AddIntegration(catalogueItemId, Arg.Any<SolutionIntegration>());
            actual.ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditIm1Integration_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            List<SolutionIntegration> integrations)
        {
            solution.Integrations = integrations;

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrations[0].Id);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewName.Should().Be("AddEditIm1Integration");
            actual.As<ViewResult>().Model.Should().BeOfType<AddEditIm1IntegrationModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditIm1Integration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns((CatalogueItem)default);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditIm1Integration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditIm1Integration_Saves_And_RedirectsToManageIntegrations(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            List<SolutionIntegration> integrations,
            AddEditIm1IntegrationModel model)
        {
            solution.Integrations = integrations;

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditIm1Integration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId,
            AddEditIm1IntegrationModel model)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns((CatalogueItem)default);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditIm1Integration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId,
            AddEditIm1IntegrationModel model)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteIm1Integration_ValidId_ReturnsView(
            Solution solution,
            List<SolutionIntegration> integrations,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            solution.Integrations = integrations;

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integrations[0].Id);

            actual.Should().NotBeNull()
                .And.BeOfType<ViewResult>();

            actual.As<ViewResult>().Model.Should().BeOfType<DeleteIntegrationModel>();

            var expectedModel = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = integrations[0].Id,
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteIm1Integration_NullSolutionId_ReturnsBadRequestObjectResult(
            int integrationId,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            mockService.GetSolutionThin(itemId).Returns((CatalogueItem)default);

            var actual = await controller.DeleteIm1Integration(itemId, integrationId);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteIm1Integration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteIm1Integration_ValidId_ReturnsRedirectToView(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            List<SolutionIntegration> integrations)
        {
            solution.Integrations = integrations;
            var integration = integrations[0];

            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = integration.Id,
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integration.Id, model);

            actual.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Interoperability));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteIm1Integration_NullSolutionId_ReturnsBadRequestObjectResult(
            int integrationId,
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.GetSolutionThin(itemId).Returns((CatalogueItem)default);

            var actual = await controller.DeleteIm1Integration(itemId, integrationId, model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteIm1Integration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddGpConnectIntegration_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<SolutionIntegration> integrations,
           List<IntegrationType> integrationTypes,
           [Frozen] ISolutionsService mockService,
           [Frozen] IIntegrationsService integrationsService,
           InteroperabilityController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = integrations;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);
            integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.GpConnect)
                .Returns(integrationTypes);

            var actual = (await controller.AddGpConnectIntegration(catalogueItem.Id)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItem.Id);
            actual.ViewName.Should().NotBeNull();
            actual.ViewName.Should().Be("AddEditGpConnectIntegration");
            actual.Model.Should().BeEquivalentTo(new AddEditGpConnectIntegrationModel(catalogueItem, integrationTypes), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddGpConnectIntegration_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.AddGpConnectIntegration(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddGpConnectIntegration_Saves_And_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            AddEditGpConnectIntegrationModel model,
            [Frozen] IInteroperabilityService mockInteroperabilityService,
            InteroperabilityController controller)
        {
            var actual = (await controller.AddGpConnectIntegration(catalogueItemId, model)).As<RedirectToActionResult>();

            await mockInteroperabilityService.Received().AddIntegration(catalogueItemId, Arg.Any<SolutionIntegration>());

            actual.ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditGpConnectIntegration_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<IntegrationType> integrationTypes,
            [Frozen] ISolutionsService mockService,
            [Frozen] IIntegrationsService integrationsService,
            InteroperabilityController controller,
            List<SolutionIntegration> integrations)
        {
            solution.Integrations = integrations;

            integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.GpConnect)
                .Returns(integrationTypes);
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewName.Should().Be("AddEditGpConnectIntegration");
            actual.As<ViewResult>().Model.Should().BeOfType<AddEditGpConnectIntegrationModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditGpConnectIntegration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns((CatalogueItem)default);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditGpConnectIntegration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditGpConnectIntegration_Saves_And_RedirectsToManageIntegrations(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            List<SolutionIntegration> integrations,
            AddEditGpConnectIntegrationModel model)
        {
            solution.Integrations = integrations;

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditGpConnectIntegration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId,
            AddEditGpConnectIntegrationModel model)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns((CatalogueItem)default);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditGpConnectIntegration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId,
            AddEditGpConnectIntegrationModel model)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteGpConnectIntegration_ValidId_ReturnsView(
            Solution solution,
            List<SolutionIntegration> integrations,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            solution.Integrations = integrations;

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id);

            actual.Should().NotBeNull()
                .And.BeOfType<ViewResult>();

            actual.As<ViewResult>().Model.Should().BeOfType<DeleteIntegrationModel>();

            var expectedModel = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = integrations[0].Id,
                IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType,
            };

            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteGpConnectIntegration_NullSolutionId_ReturnsBadRequestObjectResult(
            int integrationId,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            mockService.GetSolutionThin(itemId).Returns((CatalogueItem)default);

            var actual = await controller.DeleteGpConnectIntegration(itemId, integrationId);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteGpConnectIntegration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteGpConnectIntegration_ValidId_ReturnsRedirectToView(
            Solution solution,
            DeleteIntegrationModel model,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            List<SolutionIntegration> integrations)
        {
            solution.Integrations = integrations;

            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Interoperability));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteGpConnectIntegration_NullSolutionId_ReturnsBadRequestObjectResult(
            DeleteIntegrationModel model,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            mockService.GetSolutionThin(itemId).Returns((CatalogueItem)default);

            var actual = await controller.DeleteGpConnectIntegration(itemId, model.IntegrationId, model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteGpConnectIntegration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            DeleteIntegrationModel model,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller,
            int integrationId)
        {
            mockService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddNhsAppIntegration_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<SolutionIntegration> integrations,
           List<IntegrationType> integrationTypes,
           [Frozen] ISolutionsService mockService,
           [Frozen] IIntegrationsService integrationsService,
           InteroperabilityController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = integrations;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);
            integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.NhsApp)
                .Returns(integrationTypes);

            var actual = (await controller.AddNhsAppIntegration(catalogueItem.Id)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItem.Id);
            actual.ViewName.Should().NotBeNull();
            actual.ViewName.Should().Be("AddEditNhsAppIntegration");
            actual.Model.Should().BeEquivalentTo(new AddEditNhsAppIntegrationModel(catalogueItem, integrationTypes), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddNhsAppIntegration_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            InteroperabilityController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.AddNhsAppIntegration(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddNhsAppIntegration_WhenModelStateIsNotValid_ShouldReturnView(
        CatalogueItemId solutionId,
        AddEditNhsAppIntegrationModel model,
        InteroperabilityController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddNhsAppIntegration(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("AddEditNhsAppIntegration");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddNhsAppIntegration_Valid_Redirects(
        CatalogueItemId solutionId,
        AddEditNhsAppIntegrationModel model,
        InteroperabilityController controller)
        {
            var result = (await controller.AddNhsAppIntegration(solutionId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
        }
    }
}
