using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/interoperability")]
    public sealed class InteroperabilityController(
        ISolutionsService solutionsService,
        IInteroperabilityService interoperabilityService,
        IIntegrationsService integrationsService)
        : Controller
    {
        private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        private readonly IInteroperabilityService interoperabilityService = interoperabilityService ?? throw new ArgumentNullException(nameof(interoperabilityService));
        private readonly IIntegrationsService integrationsService = integrationsService ?? throw new ArgumentNullException(nameof(integrationsService));

        [HttpGet]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new InteroperabilityModel(solution)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId, InteroperabilityModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                model.SetSolution(solution);
                return View(model);
            }

            await interoperabilityService.SaveIntegrationLink(solutionId, model.Link);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("add-im1-integration")]
        public async Task<IActionResult> AddIm1Integration(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integrationTypes =
                await integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.Im1);

            var model = new AddEditIm1IntegrationModel(solution, integrationTypes)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
            };

            return View("AddEditIm1Integration", model);
        }

        [HttpPost("add-im1-integration")]
        public async Task<IActionResult> AddIm1Integration(CatalogueItemId solutionId, AddEditIm1IntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditIm1Integration", model);

            var integration = new SolutionIntegration
            {
                IntegrationTypeId = model.SelectedIntegrationType.GetValueOrDefault(),
                IntegratesWith = model.IntegratesWith,
                Description = model.Description,
                IsConsumer = model.IsConsumer,
            };

            await interoperabilityService.AddIntegration(solutionId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("edit-im1-integration/{integrationId}")]
        public async Task<IActionResult> EditIm1Integration(CatalogueItemId solutionId, int integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var solutionIntegration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (solutionIntegration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            var integrationTypes =
                await integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.Im1);

            var model = new AddEditIm1IntegrationModel(solution, integrationTypes, solutionIntegration)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
            };

            return View("AddEditIm1Integration", model);
        }

        [HttpPost("edit-im1-integration/{integrationId}")]
        public async Task<IActionResult> EditIm1Integration(CatalogueItemId solutionId, int integrationId, AddEditIm1IntegrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditIm1Integration", model);
            }

            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            integration.IntegrationTypeId = model.SelectedIntegrationType.GetValueOrDefault();
            integration.IntegratesWith = model.IntegratesWith;
            integration.Description = model.Description;
            integration.IsConsumer = model.IsConsumer.GetValueOrDefault();

            await interoperabilityService.EditIntegration(solutionId, integrationId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("delete-im1-integration/{integrationId}")]
        public async Task<IActionResult> DeleteIm1Integration(CatalogueItemId solutionId, int integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var im1Integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (im1Integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            var model = new DeleteIntegrationModel(solution)
            {
                BackLink = Url.Action(nameof(EditIm1Integration), new { solutionId, integrationId }),
                IntegrationId = integrationId,
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
            };

            return View("DeleteIntegration", model);
        }

        [HttpPost("delete-im1-integration/{integrationId}")]
        public async Task<IActionResult> DeleteIm1Integration(CatalogueItemId solutionId, int integrationId, DeleteIntegrationModel model)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var im1Integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (im1Integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            await interoperabilityService.DeleteIntegration(solutionId, model.IntegrationId);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("add-gp-connect-integration")]
        public async Task<IActionResult> AddGpConnectIntegration(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integrationTypes =
                await integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.GpConnect);

            var model = new AddEditGpConnectIntegrationModel(solution, integrationTypes)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
            };

            return View("AddEditGpConnectIntegration", model);
        }

        [HttpPost("add-gp-connect-integration")]
        public async Task<IActionResult> AddGpConnectIntegration(CatalogueItemId solutionId, AddEditGpConnectIntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditGpConnectIntegration", model);

            var integration = new SolutionIntegration
            {
                IntegrationTypeId = model.SelectedIntegrationType.GetValueOrDefault(),
                Description = model.AdditionalInformation,
                IsConsumer = model.IsConsumer,
            };

            await interoperabilityService.AddIntegration(solutionId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("edit-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> EditGpConnectIntegration(CatalogueItemId solutionId, int integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            var integrationTypes =
                await integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.GpConnect);

            var model = new AddEditGpConnectIntegrationModel(solution, integrationTypes, integration)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
            };

            return View("AddEditGpConnectIntegration", model);
        }

        [HttpPost("edit-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> EditGpConnectIntegration(CatalogueItemId solutionId, int integrationId, AddEditGpConnectIntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditGpConnectIntegration", model);

            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            integration.IntegrationTypeId = model.SelectedIntegrationType.GetValueOrDefault();
            integration.Description = model.AdditionalInformation;
            integration.IsConsumer = model.IsConsumer.GetValueOrDefault();

            await interoperabilityService.EditIntegration(solutionId, integrationId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("delete-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> DeleteGpConnectIntegration(CatalogueItemId solutionId, int integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            var model = new DeleteIntegrationModel(solution)
            {
                BackLink = Url.Action(nameof(EditIm1Integration), new { solutionId, integrationId }),
                IntegrationId = integrationId,
                IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType,
            };

            return View("DeleteIntegration", model);
        }

        [HttpPost("delete-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> DeleteGpConnectIntegration(CatalogueItemId solutionId, int integrationId, DeleteIntegrationModel model)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .Integrations
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            await interoperabilityService.DeleteIntegration(solutionId, model.IntegrationId);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("add-nhs-app-integration")]
        public async Task<IActionResult> AddNhsAppIntegration(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integrationTypes =
                await integrationsService.GetIntegrationTypesByIntegration(SupportedIntegrations.NhsApp);

            var model = new AddEditNhsAppIntegrationModel(solution, integrationTypes)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
            };

            return View("AddEditNhsAppIntegration", model);
        }

        [HttpPost("add-nhs-app-integration")]
        public async Task<IActionResult> AddNhsAppIntegration(CatalogueItemId solutionId, AddEditNhsAppIntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditNhsAppIntegration", model);

            var selectedAppIntegrations = model.NhsAppIntegrations.Where(x => x.Selected).Select(x => x.Value);

            await interoperabilityService.SetNhsAppIntegrations(solutionId, selectedAppIntegrations);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }
    }
}
