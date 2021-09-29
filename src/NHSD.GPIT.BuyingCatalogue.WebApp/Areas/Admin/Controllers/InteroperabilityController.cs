using System;
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
    [Route("admin/catalogue-solutions")]
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

        [HttpGet("manage/{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new InteroperabilityModel(solution));
        }

        [HttpPost("manage/{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId, InteroperabilityModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolution(solutionId);
                model.SetSolution(solution);
                return View(model);
            }

            await interoperabilityService.SaveIntegrationLink(solutionId, model.Link);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("manage/{solutionId}/interoperability/add-im1-integration")]
        public async Task<IActionResult> AddIm1Integration(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new AddIm1IntegrationModel(solution));
        }

        [HttpPost("manage/{solutionId}/interoperability/add-im1-integration")]
        public async Task<IActionResult> AddIm1Integration(CatalogueItemId solutionId, AddIm1IntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

        [HttpGet("manage/{solutionId}/interoperability/add-gp-connect-integration")]
        public async Task<IActionResult> AddGpConnectIntegration(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new AddGpConnectIntegrationModel(solution));
        }

        [HttpPost("manage/{solutionId}/interoperability/add-gp-connect-integration")]
        public async Task<IActionResult> AddGpConnectIntegration(CatalogueItemId solutionId, AddGpConnectIntegrationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
    }
}
