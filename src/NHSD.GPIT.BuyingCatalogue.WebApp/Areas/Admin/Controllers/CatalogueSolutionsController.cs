using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;
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
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IFilterCache filterCache;
        private readonly ISuppliersService suppliersService;
        private readonly ICapabilitiesService capabilitiesService;

        public CatalogueSolutionsController(
            ISolutionsService solutionsService,
            IAssociatedServicesService associatedServicesService,
            IAdditionalServicesService additionalServicesService,
            IFilterCache filterCache,
            ISuppliersService suppliersService,
            ICapabilitiesService capabilitiesService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.filterCache = filterCache ?? throw new ArgumentNullException(nameof(filterCache));
            this.suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
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

        [HttpGet("manage/{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new FeaturesModel(solution)
            {
                BackLink = Url.Action(
                    nameof(ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId, FeaturesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveSolutionFeatures(solutionId, model.AllFeatures);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}")]
        public async Task<IActionResult> ManageCatalogueSolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var solutionStatuses = await solutionsService.GetSolutionLoadingStatuses(solutionId);

            var model = new ManageCatalogueSolutionModel(solutionStatuses, solution);

            return View(model);
        }

        [HttpPost("manage/{solutionId}")]
        public async Task<IActionResult> SetPublicationStatus(CatalogueItemId solutionId, ManageCatalogueSolutionModel model)
        {
            if (!ModelState.IsValid)
                return View("ManageCatalogueSolution", model);

            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (model.SelectedPublicationStatus == solution.PublishedStatus)
                return RedirectToAction(nameof(Index));

            await solutionsService.SavePublicationStatus(solutionId, model.SelectedPublicationStatus);

            filterCache.RemoveAll();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("manage/{solutionId}/details")]
        public async Task<IActionResult> Details(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithBasicInformation(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var suppliers = await suppliersService.GetAllActiveSuppliers();

            var model = new SolutionModel(solution).WithSelectListItems(suppliers).WithEditSolution();

            model.Frameworks = (await solutionsService.GetAllFrameworks())
                .Select(f =>
                {
                    FrameworkSolution sol = solution.Solution.FrameworkSolutions.FirstOrDefault(fs => fs.FrameworkId == f.Id);
                    return new FrameworkModel
                    {
                        Name = $"{f.ShortName} Framework",
                        FrameworkId = f.Id,
                        Selected = sol is not null,
                        IsFoundation = sol?.IsFoundation ?? false,
                    };
                }).ToList();

            return View(model);
        }

        [HttpPost("manage/{solutionId}/details")]
        public async Task<IActionResult> Details(CatalogueItemId solutionId, SolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await suppliersService.GetAllActiveSuppliers();

                return View(model.WithSelectListItems(suppliers).WithEditSolution());
            }

            await solutionsService.SaveSolutionDetails(
                solutionId,
                model.SolutionName,
                model.SupplierId ?? default,
                model.Frameworks);

            filterCache.RemoveAll();

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/description")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new DescriptionModel(solution));
        }

        [HttpPost("manage/{solutionId}/description")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId, DescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ImplementationTimescaleModel(solution));
        }

        [HttpPost("manage/{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId, ImplementationTimescaleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveImplementationDetail(
                solutionId,
                model.Description);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HostingTypeSectionModel(solution)
            {
                BackLink = Url.Action(nameof(ManageCatalogueSolution), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId, HostingTypeSectionModel model)
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

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/add-hosting-type")]
        public async Task<IActionResult> AddHostingType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HostingTypeSelectionModel(solution)
            {
                BackLink = Url.Action(nameof(HostingType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/add-hosting-type")]
        public async Task<IActionResult> AddHostingType(CatalogueItemId solutionId, HostingTypeSelectionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                return View(new HostingTypeSelectionModel(solution));
            }

            return model.SelectedHostingType is null
                ? RedirectToAction(nameof(HostingType), new { solutionId })
                : RedirectToAction(model.SelectedHostingType.ToString(), new { solutionId, isNewHostingType = true });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new PublicCloudModel(catalogueItem)
            {
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-public-cloud")]
        public async Task<IActionResult> PublicCloud(CatalogueItemId solutionId, PublicCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.PublicCloud = new PublicCloud { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new PrivateCloudModel(catalogueItem)
            {
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-private-cloud")]
        public async Task<IActionResult> PrivateCloud(CatalogueItemId solutionId, PrivateCloudModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.PrivateCloud = new PrivateCloud { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HybridModel(catalogueItem)
            {
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-hybrid")]
        public async Task<IActionResult> Hybrid(CatalogueItemId solutionId, HybridModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.HybridHostingType = new HybridHostingType { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(CatalogueItemId solutionId, [FromQuery] bool? isNewHostingType = false)
        {
            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new OnPremiseModel(catalogueItem)
            {
                IsNewHostingType = isNewHostingType.GetValueOrDefault(),
                BackLink = Url.Action(nameof(HostingType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/hosting-type-on-premise")]
        public async Task<IActionResult> OnPremise(CatalogueItemId solutionId, OnPremiseModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var hosting = await solutionsService.GetHosting(solutionId);
            hosting.OnPremise = new OnPremise { Summary = model.Summary, HostingModel = model.HostingModel, Link = model.Link, RequiresHscn = model.RequiresHscn };

            await solutionsService.SaveHosting(solutionId, hosting);

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/hosting-type/delete-hosting-type/{hostingType}")]
        public async Task<IActionResult> DeleteHostingType(CatalogueItemId solutionId, HostingType hostingType)
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
                BackLink = Url.Action(backlinkActionName, new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/hosting-type/delete-hosting-type/{hostingType}")]
        public async Task<IActionResult> DeleteHostingType(CatalogueItemId solutionId, HostingType hostingType, DeleteHostingTypeConfirmationModel model)
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

            return RedirectToAction(nameof(HostingType), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based")]
        public async Task<IActionResult> BrowserBased(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = solution.Solution.GetClientApplication();
            var model = new BrowserBasedModel(solution)
            {
                BackLink = clientApplication?.HasClientApplicationType(ServiceContracts.Solutions.ClientApplicationType.BrowserBased) ?? false
                           ? Url.Action(nameof(ClientApplicationType), new { solutionId })
                           : Url.Action(nameof(AddApplicationType), new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/supported-browser")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new SupportedBrowsersModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/supported-browser")]
        public async Task<IActionResult> SupportedBrowsers(CatalogueItemId solutionId, SupportedBrowsersModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.BrowsersSupported.Clear();
            clientApplication.BrowsersSupported = model.Browsers is null
                ? new HashSet<SupportedBrowser>()
                : model.Browsers.Where(b => b.Checked).Select(b =>
                new SupportedBrowser
                {
                    BrowserName = b.BrowserName,
                    MinimumBrowserVersion = b.MinimumBrowserVersion,
                }).ToHashSet();

            clientApplication.MobileResponsive = string.IsNullOrWhiteSpace(model.MobileResponsive)
                ? null
                : model.MobileResponsive.EqualsIgnoreCase("Yes");

            clientApplication.EnsureClientApplicationTypePresent(ServiceContracts.Solutions.ClientApplicationType.BrowserBased);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new PlugInsOrExtensionsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(CatalogueItemId solutionId, PlugInsOrExtensionsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.Plugins = new Plugins
            {
                AdditionalInformation = model.AdditionalInformation,
                Required = string.IsNullOrWhiteSpace(model.PlugInsRequired)
                    ? null
                    : model.PlugInsRequired.EqualsIgnoreCase("Yes"),
            };

            clientApplication.EnsureClientApplicationTypePresent(ServiceContracts.Solutions.ClientApplicationType.BrowserBased);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ConnectivityAndResolutionModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(CatalogueItemId solutionId, ConnectivityAndResolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                return View(new ConnectivityAndResolutionModel(solution));
            }

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MinimumDesktopResolution = model.SelectedScreenResolution;
            clientApplication.EnsureClientApplicationTypePresent(ServiceContracts.Solutions.ClientApplicationType.BrowserBased);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new HardwareRequirementsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/browser-based/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionThin(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = await solutionsService.GetClientApplication(solutionId);
            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.HardwareRequirements = model.Description;
            clientApplication.EnsureClientApplicationTypePresent(ServiceContracts.Solutions.ClientApplicationType.BrowserBased);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/browser-based/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
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
            clientApplication.EnsureClientApplicationTypePresent(ServiceContracts.Solutions.ClientApplicationType.BrowserBased);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(BrowserBased), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type")]
        public async Task<IActionResult> ClientApplicationType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ClientApplicationTypeSectionModel(solution)
            {
                BackLink = Url.Action(nameof(ManageCatalogueSolution), new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("manage/{solutionId}/client-application-type/add-application-type")]
        public async Task<IActionResult> AddApplicationType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ClientApplicationTypeSelectionModel(solution)
            {
                BackLink = Url.Action(nameof(ClientApplicationType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/client-application-type/add-application-type")]
        public async Task<IActionResult> AddApplicationType(CatalogueItemId solutionId, ClientApplicationTypeSelectionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                var erroredModel = new ClientApplicationTypeSelectionModel(solution)
                {
                    BackLink = Url.Action(nameof(ClientApplicationType), new { solutionId }),
                };
                return View(erroredModel);
            }

            return model.SelectedApplicationType switch
            {
                ServiceContracts.Solutions.ClientApplicationType.BrowserBased => RedirectToAction(
                    nameof(CatalogueSolutionsController.BrowserBased),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
                ServiceContracts.Solutions.ClientApplicationType.MobileTablet => RedirectToAction(
                    nameof(MobileTabletBasedController.MobileTablet),
                    typeof(MobileTabletBasedController).ControllerName(),
                    new { solutionId }),
                ServiceContracts.Solutions.ClientApplicationType.Desktop => RedirectToAction(
                    nameof(DesktopBasedController.Desktop),
                    typeof(DesktopBasedController).ControllerName(),
                    new { solutionId }),
                _ => RedirectToAction(nameof(ClientApplicationType), new { solutionId }),
            };
        }

        [HttpGet("manage/{solutionId}/supplier-details")]
        public async Task<IActionResult> EditSupplierDetails(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithSupplierDetails(solutionId);

            var model = new EditSolutionContactsModel(catalogueItem)
            {
                BackLink = Url.Action(nameof(ManageCatalogueSolution), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/supplier-details")]
        public async Task<IActionResult> EditSupplierDetails(CatalogueItemId solutionId, EditSolutionContactsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var solution = await solutionsService.GetSolutionWithSupplierDetails(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var filteredSelectedContacts = model.AvailableSupplierContacts.Where(sc => sc.Selected).ToList();
            var selectedContacts = solution.Supplier.SupplierContacts.Join(
                filteredSelectedContacts,
                outer => outer.Id,
                inner => inner.Id,
                (supplierContact, _) => supplierContact).ToList();

            await solutionsService.SaveContacts(solutionId, selectedContacts);

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/edit-capabilities")]
        public async Task<IActionResult> EditCapabilities(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithCapabilities(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var capabilities = await capabilitiesService.GetCapabilitiesByCategory(solution.SupplierId);

            var model = new EditCapabilitiesModel(solution, capabilities)
            {
                BackLink = Url.Action(nameof(ManageCatalogueSolution), new { solutionId }),
                SolutionName = solution.Name,
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/edit-capabilities")]
        public async Task<IActionResult> EditCapabilities(CatalogueItemId solutionId, EditCapabilitiesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var saveRequestModel = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = User.UserId(),
                Capabilities = model.CapabilityCategories
                    .SelectMany(cc => cc.Capabilities.Where(c => c.Selected))
                    .ToDictionary(c => c.Id, c => c.Epics.Where(e => e.Selected).Select(e => e.Id).ToArray()),
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(solutionId, saveRequestModel);

            filterCache.RemoveAll();

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }
    }
}
