using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class CatalogueSolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IUsersService usersService;

        public CatalogueSolutionsController(
            ISolutionsService solutionsService,
            IUsersService usersService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        [HttpGet("manage/{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new FeaturesModel().FromCatalogueItem(solution));
        }

        [HttpPost("manage/{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId, FeaturesModel model)
        {
            await solutionsService.SaveSolutionFeatures(solutionId, model.AllFeatures);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetAllSolutions();

            var solutionModel = new CatalogueSolutionsModel(solutions);

            return View(solutionModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CatalogueSolutionsModel model)
        {
            model.SetSolutions(
                 await solutionsService.GetAllSolutions(string.IsNullOrWhiteSpace(model.SelectedPublicationStatus)
                     ? null
                     : Enum.Parse<PublicationStatus>(model.SelectedPublicationStatus, true)));

            return View(model);
        }

        [HttpGet("manage/{solutionId}")]
        public async Task<IActionResult> ManageCatalogueSolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            var model = new ManageCatalogueSolutionModel { Solution = solution };

            if (solution.Solution.LastUpdatedBy != Guid.Empty)
            {
                var lastUpdatedBy = await usersService.GetUser(solution.Solution.LastUpdatedBy.ToString());

                if (lastUpdatedBy is not null)
                    model.LastUpdatedByName = $"{lastUpdatedBy.FirstName} {lastUpdatedBy.LastName}";
            }

            return View(model);
        }

        [HttpGet("manage/{solutionId}/description")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new DescriptionModel(solution));
        }

        [HttpPost("manage/{solutionId}/description")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId, DescriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await solutionsService.SaveSolutionDescription(
                solutionId,
                model.Summary,
                model.Description,
                model.Link);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ImplementationTimescaleModel(solution));
        }

        [HttpPost("manage/{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId, ImplementationTimescaleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await solutionsService.SaveImplementationDetail(
                solutionId,
                model.Description);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/development-plans")]
        public async Task<IActionResult> Roadmap(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new RoadmapModel().FromCatalogueItem(solution));
        }

        [HttpPost("manage/{solutionId}/development-plans")]
        public async Task<IActionResult> Roadmap(CatalogueItemId solutionId, RoadmapModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolution(solutionId);
                return View(model.FromCatalogueItem(solution));
            }

            await solutionsService.SaveRoadMap(solutionId, model.Link);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new HostingTypeSectionModel(solution));
        }

        [HttpPost("manage/{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId, HostingTypeSectionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var catalogueItem = await solutionsService.GetSolution(solutionId);

            Hosting hosting = new Hosting
            {
                PublicCloud = catalogueItem.Solution?.GetHosting()?.PublicCloud,
                PrivateCloud = catalogueItem.Solution?.GetHosting()?.PrivateCloud,
                HybridHostingType = catalogueItem.Solution?.GetHosting()?.HybridHostingType,
                OnPremise = catalogueItem.Solution?.GetHosting()?.OnPremise,
            };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }
    }
}
