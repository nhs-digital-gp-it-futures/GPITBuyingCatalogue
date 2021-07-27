using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
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
            if (model.ExistingHostingTypesCount == 0)
            {
                ModelState.AddModelError(" ", "Add at least one hosting type");
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

        [HttpGet("manage/{solutionId}/hosting-type/add-hosting-type")]
        public async Task<IActionResult> AddHostingType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new HostingTypeSelectionModel(solution));
        }

        [HttpPost("manage/{solutionId}/hosting-type/add-hosting-type")]
        public async Task<IActionResult> AddHostingType(CatalogueItemId solutionId, HostingTypeSelectionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolution(solutionId);
                return View(new HostingTypeSelectionModel(solution));
            }

            return model.SelectedHostingType is null
                ? RedirectToAction(nameof(HostingType), new { solutionId })
                : RedirectToAction(model.SelectedHostingType.ToString(), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolution(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new PublicCloudModel(catalogueItem.Solution.GetHosting().PublicCloud);
            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(CatalogueItemId solutionId, PublicCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.PublicCloud = new PublicCloud { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolution(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new PrivateCloudModel(catalogueItem.Solution.GetHosting().PrivateCloud);
            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(CatalogueItemId solutionId, PrivateCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.PrivateCloud = new PrivateCloud { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolution(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HybridModel(catalogueItem.Solution.GetHosting().HybridHostingType);
            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(CatalogueItemId solutionId, HybridModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.HybridHostingType = new HybridHostingType { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolution(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new OnPremiseModel(catalogueItem.Solution.GetHosting().OnPremise);
            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(CatalogueItemId solutionId, OnPremiseModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.OnPremise = new OnPremise { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }
    }
}
