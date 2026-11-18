using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionType}/{solutionId}/hosting-type")]
    public class HostingTypesController(ISolutionsService solutionsService) : Controller
    {
        private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));

        [HttpGet]
        public async Task<IActionResult> HostingType(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HostingTypeSectionModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HostingType(SolutionType solutionType, CatalogueItemId solutionId, HostingTypeSectionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);

            Hosting hosting = new Hosting
            {
                PublicCloud = catalogueItem.Solution?.Hosting?.PublicCloud,
                PrivateCloud = catalogueItem.Solution?.Hosting?.PrivateCloud,
                HybridHostingType = catalogueItem.Solution?.Hosting?.HybridHostingType,
                OnPremise = catalogueItem.Solution?.Hosting?.OnPremise,
            };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionType, solutionId });
        }

        [HttpGet("add-hosting-type")]
        public async Task<IActionResult> AddHostingType(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HostingTypeSelectionModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(HostingType), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("add-hosting-type")]
        public async Task<IActionResult> AddHostingType(SolutionType solutionType, CatalogueItemId solutionId, HostingTypeSelectionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                return View(new HostingTypeSelectionModel(solution) { SolutionType = solutionType });
            }

            return model.SelectedHostingType is null
                ? RedirectToAction(nameof(HostingType), new { solutionType, solutionId })
                : RedirectToAction(model.SelectedHostingType.ToString(), new { solutionType, solutionId, isNewHostingType = true });
        }

        [HttpGet("hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new PublicCloudModel(catalogueItem)
            {
                SolutionType = solutionType,
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(SolutionType solutionType, CatalogueItemId solutionId, PublicCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = catalogueItem.Solution.Hosting ?? new Hosting();
            hosting.PublicCloud = new PublicCloud { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionType, solutionId });
        }

        [HttpGet("hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(SolutionType solutionType, CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new PrivateCloudModel(catalogueItem)
            {
                SolutionType = solutionType,
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(SolutionType solutionType, CatalogueItemId solutionId, PrivateCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = catalogueItem.Solution.Hosting ?? new Hosting();
            hosting.PrivateCloud = new PrivateCloud { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionType, solutionId });
        }

        [HttpGet("hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(SolutionType solutionType, CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HybridModel(catalogueItem)
            {
                SolutionType = solutionType,
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(SolutionType solutionType, CatalogueItemId solutionId, HybridModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = catalogueItem.Solution.Hosting ?? new Hosting();
            hosting.HybridHostingType = new HybridHostingType { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionType, solutionId });
        }

        [HttpGet("hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(SolutionType solutionType, CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new OnPremiseModel(catalogueItem)
            {
                SolutionType = solutionType,
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(SolutionType solutionType, CatalogueItemId solutionId, OnPremiseModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = catalogueItem.Solution.Hosting ?? new Hosting();
            hosting.OnPremise = new OnPremise { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionType, solutionId });
        }

        [HttpGet("delete-hosting-type/{hostingType}")]
        public async Task<IActionResult> DeleteHostingType(SolutionType solutionType, CatalogueItemId solutionId, HostingType hostingType)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            var backlinkActionName = hostingType switch
            {
                ServiceContracts.Solutions.HostingType.Hybrid => nameof(Hybrid),
                ServiceContracts.Solutions.HostingType.OnPremise => nameof(OnPremise),
                ServiceContracts.Solutions.HostingType.PrivateCloud => nameof(PrivateCloud),
                ServiceContracts.Solutions.HostingType.PublicCloud => nameof(PublicCloud),
                _ => throw new ArgumentOutOfRangeException(nameof(hostingType)),
            };

            var model = new DeleteHostingTypeConfirmationModel(solution, hostingType)
            {
                BackLink = Url.Action(backlinkActionName, new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("delete-hosting-type/{hostingType}")]
        public async Task<IActionResult> DeleteHostingType(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            HostingType hostingType,
            DeleteHostingTypeConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var solution = await solutionsService.GetSolutionThin(solutionId);
            var hosting = solution.Solution.Hosting;
            switch (hostingType)
            {
                case ServiceContracts.Solutions.HostingType.Hybrid:
                    hosting.HybridHostingType = new HybridHostingType();
                    break;

                case ServiceContracts.Solutions.HostingType.OnPremise:
                    hosting.OnPremise = new OnPremise();
                    break;

                case ServiceContracts.Solutions.HostingType.PrivateCloud:
                    hosting.PrivateCloud = new PrivateCloud();
                    break;

                case ServiceContracts.Solutions.HostingType.PublicCloud:
                    hosting.PublicCloud = new PublicCloud();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(hostingType));
            }

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionType, solutionId });
        }
    }
}
