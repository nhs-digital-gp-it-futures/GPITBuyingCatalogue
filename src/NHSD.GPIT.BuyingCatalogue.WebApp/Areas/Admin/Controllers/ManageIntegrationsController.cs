using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/manage-integrations")]
public class ManageIntegrationsController(IIntegrationsService integrationsService) : Controller
{
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
    public async Task<IActionResult> ViewIntegration(SupportedIntegrations integrationId)
    {
        var integration = await integrationsService.GetIntegrationWithTypes(integrationId);

        var model = new ViewIntegrationModel(integration) { BackLink = Url.Action(nameof(Integrations)), };

        return View(model);
    }
}
