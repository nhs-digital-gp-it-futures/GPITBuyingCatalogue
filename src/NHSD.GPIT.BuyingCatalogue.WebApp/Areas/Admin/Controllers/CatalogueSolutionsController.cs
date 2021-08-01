using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;
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

        [HttpGet("manage/{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new InteroperabilityModel(solution));
        }

// TODO
//        [HttpPost("manage/{solutionId}/Interoperability")]
//        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId, DescriptionModel model)
//        {
//        }

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
                return View(model);

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

        [HttpGet("manage/{solutionId}/client-application-type/browser-based")]
        public async Task<IActionResult> BrowserBased(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new BrowserBasedModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based")]
        public async Task<IActionResult> BrowserBased(CatalogueItemId solutionId, BrowserBasedModel model)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (!ModelState.IsValid)
            {
                return View(new BrowserBasedModel(solution));
            }

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/supported-browser")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new SupportedBrowsersModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/supported-browser")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId, SupportedBrowsersModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.BrowsersSupported.Clear();
            clientApplication.BrowsersSupported = model.Browsers is null
                ? new HashSet<string>()
                : model.Browsers.Where(b => b.Checked).Select(b => b.BrowserName).ToHashSet();

            clientApplication.MobileResponsive = string.IsNullOrWhiteSpace(model.MobileResponsive)
                ? (bool?)null
                : model.MobileResponsive.EqualsIgnoreCase("Yes");

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new PlugInsOrExtensionsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId, PlugInsOrExtensionsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.Plugins = new Plugins
            {
                AdditionalInformation = model.AdditionalInformation,
                Required = string.IsNullOrWhiteSpace(model.PlugInsRequired)
                    ? (bool?)null
                    : model.PlugInsRequired.EqualsIgnoreCase("Yes"),
            };

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ConnectivityAndResolutionModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId, ConnectivityAndResolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolution(solutionId);
                return View(new ConnectivityAndResolutionModel(solution));
            }

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MinimumDesktopResolution = model.SelectedScreenResolution;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new HardwareRequirementsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.HardwareRequirements = model.Description;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new AdditionalInformationModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.AdditionalInformation = model.AdditionalInformation;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }
    }
}
