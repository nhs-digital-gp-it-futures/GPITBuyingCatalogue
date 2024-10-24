﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/manage-integrations")]
public class ManageIntegrationsController(IIntegrationsService integrationsService) : Controller
{
    private const string AddEditViewName = "AddEditIntegrationType";

    private readonly IIntegrationsService integrationsService =
        integrationsService ?? throw new ArgumentNullException(nameof(integrationsService));

    [HttpGet]
    public async Task<IActionResult> Integrations()
    {
        var integrations = await integrationsService.GetIntegrationsWithTypes();

        var model = new ManageIntegrationsModel(integrations);

        return View(model);
    }

    [HttpGet("{integrationId}")]
    public async Task<IActionResult> ViewIntegration(int integrationId)
    {
        var integration = await integrationsService.GetIntegrationWithTypes(integrationId.ToIntegrationId());
        if (integration is null)
            return RedirectToAction(nameof(Integrations));

        var model = new ViewIntegrationModel(integration) { BackLink = Url.Action(nameof(Integrations)), };

        return View(model);
    }

    [HttpGet("{integrationId}/add")]
    public async Task<IActionResult> AddIntegrationType(int integrationId)
    {
        var integration = await integrationsService.GetIntegrationWithTypes(integrationId.ToIntegrationId());
        if (integration is null)
            return RedirectToAction(nameof(ViewIntegration), new { integrationId });

        var model = new AddEditIntegrationTypeModel(integration)
        {
            BackLink = Url.Action(nameof(ViewIntegration), new { integrationId }),
        };

        return View(AddEditViewName, model);
    }

    [HttpPost("{integrationId}/add")]
    public async Task<IActionResult> AddIntegrationType(
        int integrationId,
        AddEditIntegrationTypeModel model)
    {
        if (!ModelState.IsValid)
            return View(AddEditViewName, model);

        await integrationsService.AddIntegrationType(integrationId.ToIntegrationId(), model.IntegrationTypeName, model.Description);

        return RedirectToAction(nameof(ViewIntegration), new { integrationId });
    }

    [HttpGet("{integrationId}/edit/{integrationTypeId}")]
    public async Task<IActionResult> EditIntegrationType(int integrationId, int integrationTypeId)
    {
        var integrationType = await integrationsService.GetIntegrationTypeById(integrationId.ToIntegrationId(), integrationTypeId);
        if (integrationType is null)
            return RedirectToAction(nameof(ViewIntegration), new { integrationId });

        var model = new AddEditIntegrationTypeModel(integrationType.Integration, integrationType)
        {
            BackLink = Url.Action(nameof(ViewIntegration), new { integrationId }),
        };

        return View(AddEditViewName, model);
    }

    [HttpPost("{integrationId}/edit/{integrationTypeId}")]
    public async Task<IActionResult> EditIntegrationType(
        int integrationId,
        int integrationTypeId,
        AddEditIntegrationTypeModel model)
    {
        if (!ModelState.IsValid)
            return View(AddEditViewName, model);

        await integrationsService.EditIntegrationType(
            integrationId.ToIntegrationId(),
            integrationTypeId,
            model.IntegrationTypeName,
            model.Description);

        return RedirectToAction(nameof(ViewIntegration), new { integrationId });
    }
}
