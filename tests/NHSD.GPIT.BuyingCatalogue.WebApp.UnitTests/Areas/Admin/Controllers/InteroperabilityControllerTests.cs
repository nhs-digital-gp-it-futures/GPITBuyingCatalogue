﻿using System;
using System.Collections.Generic;
using System.Text.Json;
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
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

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
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            var catalogueItem = solution.CatalogueItem;

            mockService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AddIm1Integration(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItem.Id));
            actual.ViewName.Should().Be("AddEditIm1Integration");
            var expectedModel = new AddEditIm1IntegrationModel(catalogueItem);
            actual.Model.Should().BeEquivalentTo(expectedModel, opt =>
                opt.Excluding(m => m.BackLink));
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
            AddEditIm1IntegrationModel model,
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
                .Callback<CatalogueItemId, Integration>((a1, a2) => { savedIntegration = a2; });

            var actual = (await controller.AddIm1Integration(catalogueItemId, model)).As<RedirectToActionResult>();

            mockInteroperabilityService.Verify(s => s.AddIntegration(catalogueItemId, It.IsAny<Integration>()));
            savedIntegration.Should().BeEquivalentTo(expectedIntegration);
            actual.ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditIm1Integration_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            List<Integration> integrations)
        {
            integrations.ForEach(i => i.IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType);
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrations[0].Id);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewName.Should().Be("AddEditIm1Integration");
            actual.As<ViewResult>().Model.Should().BeOfType<AddEditIm1IntegrationModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditIm1Integration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync((CatalogueItem)default);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditIm1Integration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditIm1Integration_Saves_And_RedirectsToManageIntegrations(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller,
            List<Integration> integrations,
            AddEditIm1IntegrationModel model)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

            mockInteroperabilityService.Setup(s => s.EditIntegration(solution.CatalogueItemId, integrations[0].Id, integrations[0]));

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditIm1Integration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId,
            AddEditIm1IntegrationModel model)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync((CatalogueItem)default);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditIm1Integration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId,
            AddEditIm1IntegrationModel model)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditIm1Integration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteIm1Integration_ValidId_ReturnsView(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

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
        [CommonAutoData]
        public static async Task Get_DeleteIm1Integration_NullSolutionId_ReturnsBadRequestObjectResult(
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            mockService.Setup(s => s.GetSolution(itemId)).ReturnsAsync((CatalogueItem)default);

            var actual = await controller.DeleteIm1Integration(itemId, Guid.NewGuid());

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteIm1Integration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteIm1Integration_ValidId_ReturnsRedirectToView(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller,
            List<Integration> integrations)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);

            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = Guid.NewGuid(),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);
            mockInteroperabilityService.Setup(s => s.DeleteIntegration(solution.CatalogueItemId, integrations[0].Id));

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Interoperability));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteIm1Integration_NullSolutionId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = Guid.NewGuid(),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.Setup(s => s.GetSolution(itemId)).ReturnsAsync((CatalogueItem)default);

            var actual = await controller.DeleteIm1Integration(itemId, Guid.NewGuid(), model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteIm1Integration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = Guid.NewGuid(),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.DeleteIm1Integration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddGpConnectIntegration_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           InteroperabilityController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AddGpConnectIntegration(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItem.Id));
            actual.ViewName.Should().NotBeNull();
            actual.ViewName.Should().Be("AddEditGpConnectIntegration");
            actual.Model.Should().BeEquivalentTo(new AddEditGpConnectIntegrationModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddGpConnectIntegration_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddGpConnectIntegration(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddGpConnectIntegration_Saves_And_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            AddEditGpConnectIntegrationModel model,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller)
        {
            var expectedIntegration = new Integration
            {
                IntegrationType = "GP Connect",
                AdditionalInformation = model.AdditionalInformation,
                Qualifier = model.SelectedIntegrationType,
                IsConsumer = model.SelectedProviderOrConsumer == "Consumer",
            };

            Integration savedIntegration = null;
            mockInteroperabilityService.Setup(s => s.AddIntegration(It.IsAny<CatalogueItemId>(), It.IsAny<Integration>()))
                .Callback<CatalogueItemId, Integration>((a1, a2) => { savedIntegration = a2; });

            var actual = (await controller.AddGpConnectIntegration(catalogueItemId, model)).As<RedirectToActionResult>();

            mockInteroperabilityService.Verify(s => s.AddIntegration(catalogueItemId, It.IsAny<Integration>()));
            savedIntegration.Should().BeEquivalentTo(expectedIntegration);
            actual.ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditGpConnectIntegration_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            List<Integration> integrations)
        {
            integrations.ForEach(i => i.IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType);
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewName.Should().Be("AddEditGpConnectIntegration");
            actual.As<ViewResult>().Model.Should().BeOfType<AddEditGpConnectIntegrationModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditGpConnectIntegration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync((CatalogueItem)default);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditGpConnectIntegration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditGpConnectIntegration_Saves_And_RedirectsToManageIntegrations(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller,
            List<Integration> integrations,
            AddEditGpConnectIntegrationModel model)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

            mockInteroperabilityService.Setup(s => s.EditIntegration(solution.CatalogueItemId, integrations[0].Id, integrations[0]));

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(InteroperabilityController.Interoperability));
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditGpConnectIntegration_NullSolution_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId,
            AddEditGpConnectIntegrationModel model)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync((CatalogueItem)default);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No Solution found for Id: {solution.CatalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditGpConnectIntegration_NullIntegration_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId,
            AddEditGpConnectIntegrationModel model)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.EditGpConnectIntegration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().BeEquivalentTo($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteGpConnectIntegration_ValidId_ReturnsView(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller)
        {
            integrations.ForEach(i => i.IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType);
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

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
        [CommonAutoData]
        public static async Task Get_DeleteGpConnectIntegration_NullSolutionId_ReturnsBadRequestObjectResult(
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            mockService.Setup(s => s.GetSolution(itemId)).ReturnsAsync((CatalogueItem)default);

            var actual = await controller.DeleteGpConnectIntegration(itemId, Guid.NewGuid());

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteGpConnectIntegration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrationId);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteGpConnectIntegration_ValidId_ReturnsRedirectToView(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IInteroperabilityService> mockInteroperabilityService,
            InteroperabilityController controller,
            List<Integration> integrations)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);

            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = Guid.NewGuid(),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);
            mockInteroperabilityService.Setup(s => s.DeleteIntegration(solution.CatalogueItemId, integrations[0].Id));

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrations[0].Id, model);

            actual.Should().NotBeNull()
                .And.BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Interoperability));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteGpConnectIntegration_NullSolutionId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            CatalogueItemId itemId)
        {
            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = Guid.NewGuid(),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.Setup(s => s.GetSolution(itemId)).ReturnsAsync((CatalogueItem)default);

            var actual = await controller.DeleteGpConnectIntegration(itemId, Guid.NewGuid(), model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {itemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteGpConnectIntegration_InvalidIntegrationId_ReturnsBadRequestObjectResult(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            InteroperabilityController controller,
            Guid integrationId)
        {
            var model = new DeleteIntegrationModel(solution.CatalogueItem)
            {
                IntegrationId = Guid.NewGuid(),
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            mockService.Setup(s => s.GetSolution(solution.CatalogueItemId)).ReturnsAsync(solution.CatalogueItem);

            var actual = await controller.DeleteGpConnectIntegration(solution.CatalogueItemId, integrationId, model);

            actual.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No integration found for Id: {integrationId}");
        }
    }
}
