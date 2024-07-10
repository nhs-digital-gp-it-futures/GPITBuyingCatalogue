using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class ManageIntegrationsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(ManageIntegrationsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Integrations_ReturnsViewWithModel(
        List<Integration> integrations,
        [Frozen] IIntegrationsService integrationsService,
        ManageIntegrationsController controller)
    {
        integrationsService.GetIntegrationsWithTypes().Returns(integrations);

        var expected = new ManageIntegrationsModel(integrations);

        var result = (await controller.Integrations()).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MockAutoData]
    public static async Task ViewIntegration_InvalidIntegrationId_Redirects(
        SupportedIntegrations integrationId,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        service.GetIntegrationWithTypes(integrationId).Returns((Integration)null);

        var result = (await controller.ViewIntegration(integrationId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Integrations));
    }

    [Theory]
    [MockAutoData]
    public static async Task ViewIntegration_ValidIntegrationId_ReturnsViewWithModel(
        Integration integration,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        service.GetIntegrationWithTypes(integration.Id).Returns(integration);

        var expected = new ViewIntegrationModel(integration);

        var result = (await controller.ViewIntegration(integration.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task AddIntegrationType_InvalidIntegrationId_Redirects(
        SupportedIntegrations integrationId,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        service.GetIntegrationWithTypes(integrationId).Returns((Integration)null);

        var result = (await controller.AddIntegrationType(integrationId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewIntegration));
    }

    [Theory]
    [MockAutoData]
    public static async Task AddIntegrationType_ValidIntegrationId_ReturnsViewWithModel(
        Integration integration,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        service.GetIntegrationWithTypes(integration.Id).Returns(integration);

        var expected = new AddEditIntegrationTypeModel(integration);

        var result = (await controller.AddIntegrationType(integration.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_AddIntegrationType_InvalidModel_ReturnsView(
        SupportedIntegrations integrationId,
        AddEditIntegrationTypeModel model,
        ManageIntegrationsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.AddIntegrationType(integrationId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_AddIntegrationType_Valid_Redirects(
        SupportedIntegrations integrationId,
        AddEditIntegrationTypeModel model,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        var result = (await controller.AddIntegrationType(integrationId, model)).As<RedirectToActionResult>();

        await service.Received().AddIntegrationType(integrationId, model.IntegrationTypeName, model.Description);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewIntegration));
    }

    [Theory]
    [MockAutoData]
    public static async Task EditIntegrationType_InvalidIntegrationId_Redirects(
        SupportedIntegrations integrationId,
        int integrationTypeId,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        service.GetIntegrationTypeById(integrationId, integrationTypeId).Returns((IntegrationType)null);

        var result =
            (await controller.EditIntegrationType(integrationId, integrationTypeId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewIntegration));
    }

    [Theory]
    [MockAutoData]
    public static async Task EditIntegrationType_ValidIntegrationId_ReturnsViewWithModel(
        Integration integration,
        IntegrationType integrationType,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        integrationType.Integration = integration;

        service.GetIntegrationTypeById(integrationType.IntegrationId, integrationType.Id).Returns(integrationType);

        var expected = new AddEditIntegrationTypeModel(integrationType.Integration, integrationType);

        var result = (await controller.EditIntegrationType(integrationType.IntegrationId, integrationType.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_EditIntegrationType_InvalidModel_ReturnsView(
        SupportedIntegrations integrationId,
        int integrationTypeId,
        AddEditIntegrationTypeModel model,
        ManageIntegrationsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.EditIntegrationType(integrationId, integrationTypeId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_EditIntegrationType_Valid_Redirects(
        SupportedIntegrations integrationId,
        int integrationTypeId,
        AddEditIntegrationTypeModel model,
        [Frozen] IIntegrationsService service,
        ManageIntegrationsController controller)
    {
        var result = (await controller.EditIntegrationType(integrationId, integrationTypeId, model))
            .As<RedirectToActionResult>();

        await service.Received()
            .EditIntegrationType(integrationId, integrationTypeId, model.IntegrationTypeName, model.Description);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewIntegration));
    }
}
