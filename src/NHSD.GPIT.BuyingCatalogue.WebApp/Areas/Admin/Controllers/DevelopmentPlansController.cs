using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/development-plans")]
    public sealed class DevelopmentPlansController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IDevelopmentPlansService developmentPlansService;

        public DevelopmentPlansController(
            ISolutionsService solutionsService,
            IDevelopmentPlansService developmentPlansService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.developmentPlansService = developmentPlansService ?? throw new ArgumentNullException(nameof(developmentPlansService));
        }

        [HttpGet]
        public async Task<IActionResult> DevelopmentPlans(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new DevelopmentPlanModel(solution)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DevelopmentPlans(CatalogueItemId solutionId, DevelopmentPlanModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolution(solutionId);
                return View(model);
            }

            await developmentPlansService.SaveDevelopmentPlans(solutionId, model.Link);

            return RedirectToAction(nameof(CatalogueSolutionsController.ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("add-work-off-plan")]
        public async Task<IActionResult> AddWorkOffPlan(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var standards = await solutionsService.GetSolutionStandardsForEditing(solution.Id);

            var model = new EditWorkOffPlanModel(solution, standards)
            {
                BackLink = Url.Action(
                    nameof(DevelopmentPlans),
                    typeof(DevelopmentPlansController).ControllerName(),
                    new { solutionId }),
            };

            return View("EditWorkOffPlan", model);
        }

        [HttpPost("add-work-off-plan")]
        public async Task<IActionResult> AddWorkOffPlan(CatalogueItemId solutionId, EditWorkOffPlanModel model)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (!ModelState.IsValid)
            {
                var standards = await solutionsService.GetSolutionStandardsForEditing(solutionId);
                model.Standards = standards;
                return View("EditWorkOffPlan", model);
            }

            var saveModel = new SaveWorkOffPlanModel()
            {
                SolutionId = solutionId,
                StandardId = model.SelectedStandard,
                Details = model.Details,
                CompletionDate = model.CompletionDate.Value,
            };

            await developmentPlansService.SaveWorkOffPlan(solutionId, saveModel);

            return RedirectToAction(nameof(DevelopmentPlans), new { solutionId });
        }

        [HttpGet("edit-work-off-plan/{workOffPlanId}")]
        public async Task<IActionResult> EditWorkOffPlan(CatalogueItemId solutionId, int workOffPlanId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var workOffPlan = solution.Solution.WorkOffPlans.FirstOrDefault(wp => wp.Id == workOffPlanId);

            if (workOffPlan is null)
                return BadRequest($"No Work-off Plan item found for Id: {workOffPlanId}");

            var standards = await solutionsService.GetSolutionStandardsForEditing(solutionId);

            var model = new EditWorkOffPlanModel(solution, standards, workOffPlan)
            {
                BackLink = Url.Action(
                    nameof(DevelopmentPlans),
                    typeof(DevelopmentPlansController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("edit-work-off-plan/{workOffPlanId}")]
        public async Task<IActionResult> EditWorkOffPlan(CatalogueItemId solutionId, int workOffPlanId, EditWorkOffPlanModel model)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var workOffPlan = solution.Solution.WorkOffPlans.FirstOrDefault(wp => wp.Id == workOffPlanId);

            if (workOffPlan is null)
                return BadRequest($"No Work-off Plan item found for Id: {workOffPlanId}");

            if (!ModelState.IsValid)
            {
                var standards = await solutionsService.GetSolutionStandardsForEditing(solutionId);
                model.Standards = standards;
                return View("EditWorkOffPlan", model);
            }

            var saveModel = new SaveWorkOffPlanModel()
            {
                Id = workOffPlanId,
                SolutionId = solutionId,
                StandardId = model.SelectedStandard,
                Details = model.Details,
                CompletionDate = model.CompletionDate.Value,
            };

            await developmentPlansService.UpdateWorkOffPlan(workOffPlanId, saveModel);

            return RedirectToAction(nameof(DevelopmentPlans), new { solutionId });
        }

        [HttpGet("delete-work-off-plan/{workOffPlanId}")]
        public async Task<IActionResult> DeleteWorkOffPlan(CatalogueItemId solutionId, int workOffPlanId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var workOffPlan = solution.Solution.WorkOffPlans.FirstOrDefault(wp => wp.Id == workOffPlanId);

            if (workOffPlan is null)
                return BadRequest($"No Work-off Plan item found for Id: {workOffPlanId}");

            var model = new EditWorkOffPlanModel(solution, workOffPlan)
            {
                BackLink = Url.Action(
                nameof(EditWorkOffPlan),
                typeof(DevelopmentPlansController).ControllerName(),
                new { solutionId, workOffPlanId = workOffPlan.Id }),
            };

            return View(model);
        }

        [HttpPost("delete-work-off-plan/{workOffPlanId}")]
        public async Task<IActionResult> DeleteWorkOffPlan(CatalogueItemId solutionId, int workOffPlanId, EditWorkOffPlanModel model)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var workOffPlan = solution.Solution.WorkOffPlans.FirstOrDefault(wp => wp.Id == workOffPlanId);

            if (workOffPlan is null)
                return BadRequest($"No Work-off Plan item found for Id: {workOffPlanId}");

            await developmentPlansService.DeleteWorkOffPlan(workOffPlanId);

            return RedirectToAction(nameof(DevelopmentPlans), new { solutionId });
        }
    }
}
