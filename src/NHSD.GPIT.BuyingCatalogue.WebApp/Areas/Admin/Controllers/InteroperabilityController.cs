using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/interoperability")]
    public sealed class InteroperabilityController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IInteroperabilityService interoperabilityService;

        public InteroperabilityController(
            ISolutionsService solutionsService,
            IInteroperabilityService interoperabilityService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.interoperabilityService = interoperabilityService ?? throw new ArgumentNullException(nameof(interoperabilityService));
        }

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

            var model = new AddEditIm1IntegrationModel(solution)
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

            var integration = new Integration
            {
                IntegrationType = Framework.Constants.Interoperability.IM1IntegrationType,
                IntegratesWith = model.IntegratesWith,
                Description = model.Description,
                Qualifier = model.SelectedIntegrationType,
                IsConsumer = model.SelectedProviderOrConsumer == Framework.Constants.Interoperability.Consumer,
            };

            await interoperabilityService.AddIntegration(solutionId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("edit-im1-integration/{integrationId}")]
        public async Task<IActionResult> EditIm1Integration(CatalogueItemId solutionId, Guid integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var im1Integration = solution
                .Solution
                .GetIntegrations()
                .FirstOrDefault(i => i.Id == integrationId);

            if (im1Integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            var model = new AddEditIm1IntegrationModel(solution)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
                IntegrationId = integrationId,
                Description = im1Integration.Description,
                SelectedIntegrationType = im1Integration.Qualifier,
                IntegratesWith = im1Integration.IntegratesWith,
                SelectedProviderOrConsumer = im1Integration.IsConsumer ?
                    Framework.Constants.Interoperability.Consumer :
                    Framework.Constants.Interoperability.Provider,
            };

            return View("AddEditIm1Integration", model);
        }

        [HttpPost("edit-im1-integration/{integrationId}")]
        public async Task<IActionResult> EditIm1Integration(CatalogueItemId solutionId, Guid integrationId, AddEditIm1IntegrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditIm1Integration", model);
            }

            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var im1Integration = solution
                .Solution
                .GetIntegrations()
                .FirstOrDefault(i => i.Id == integrationId);

            if (im1Integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            im1Integration.Qualifier = model.SelectedIntegrationType;
            im1Integration.IntegratesWith = model.IntegratesWith;
            im1Integration.Description = model.Description;
            im1Integration.Qualifier = model.SelectedIntegrationType;
            im1Integration.IsConsumer = model.SelectedProviderOrConsumer == Framework.Constants.Interoperability.Consumer;

            await interoperabilityService.EditIntegration(solutionId, integrationId, im1Integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("delete-im1-integration/{integrationId}")]
        public async Task<IActionResult> DeleteIm1Integration(CatalogueItemId solutionId, Guid integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var im1Integration = solution
                .Solution
                .GetIntegrations()
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
        public async Task<IActionResult> DeleteIm1Integration(CatalogueItemId solutionId, Guid integrationId, DeleteIntegrationModel model)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var im1Integration = solution
                .Solution
                .GetIntegrations()
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

            var model = new AddEditGpConnectIntegrationModel(solution)
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

            var integration = new Integration
            {
                IntegrationType = Framework.Constants.Interoperability.GpConnectIntegrationType,
                AdditionalInformation = model.AdditionalInformation,
                Qualifier = model.SelectedIntegrationType,
                IsConsumer = model.SelectedProviderOrConsumer == Framework.Constants.Interoperability.Consumer,
            };

            await interoperabilityService.AddIntegration(solutionId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("edit-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> EditGpConnectIntegration(CatalogueItemId solutionId, Guid integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .GetIntegrations()
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            var model = new AddEditGpConnectIntegrationModel(solution)
            {
                BackLink = Url.Action(nameof(Interoperability), new { solutionId }),
                AdditionalInformation = integration.AdditionalInformation,
                SelectedIntegrationType = integration.Qualifier,
                SelectedProviderOrConsumer = integration.IsConsumer ?
                    Framework.Constants.Interoperability.Consumer :
                    Framework.Constants.Interoperability.Provider,
                IntegrationId = integration.Id,
            };

            return View("AddEditGpConnectIntegration", model);
        }

        [HttpPost("edit-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> EditGpConnectIntegration(CatalogueItemId solutionId, Guid integrationId, AddEditGpConnectIntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditGpConnectIntegration", model);

            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .GetIntegrations()
                .FirstOrDefault(i => i.Id == integrationId);

            if (integration is null)
                return BadRequest($"No integration found for Id: {integrationId}");

            integration.Qualifier = model.SelectedIntegrationType;
            integration.AdditionalInformation = model.AdditionalInformation;
            integration.Qualifier = model.SelectedIntegrationType;
            integration.IsConsumer = model.SelectedProviderOrConsumer == Framework.Constants.Interoperability.Consumer;

            await interoperabilityService.EditIntegration(solutionId, integrationId, integration);

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }

        [HttpGet("delete-gp-connect-integration/{integrationId}")]
        public async Task<IActionResult> DeleteGpConnectIntegration(CatalogueItemId solutionId, Guid integrationId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .GetIntegrations()
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
        public async Task<IActionResult> DeleteGpConnectIntegration(CatalogueItemId solutionId, Guid integrationId, DeleteIntegrationModel model)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var integration = solution
                .Solution
                .GetIntegrations()
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

            var model = new AddEditNhsAppIntegrationModel(solution)
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

            var integration = new Integration
            {
                IntegrationType = Framework.Constants.Interoperability.NhsAppIntegrationType,
                NHSAppIntegrationTypes = model.NhsAppIntegrationTypes
                                        .Where(o => o.Checked)
                                        .Select(o => o.IntegrationType)
                                        .ToHashSet(),
            };

            var integrationExists = await interoperabilityService.GetIntegrationById(solutionId, model.IntegrationId);

            if (string.IsNullOrEmpty(integrationExists?.IntegrationType))
            {
                await interoperabilityService.AddIntegration(solutionId, integration);
            }
            else
            {
                await interoperabilityService.EditIntegration(solutionId, model.IntegrationId, integration);
            }

            return RedirectToAction(nameof(Interoperability), new { solutionId });
        }
    }
}
