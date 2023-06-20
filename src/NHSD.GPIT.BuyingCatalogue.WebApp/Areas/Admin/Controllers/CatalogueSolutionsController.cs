using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class CatalogueSolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly ISuppliersService suppliersService;
        private readonly ICapabilitiesService capabilitiesService;
        private readonly ISolutionPublicationStatusService publicationStatusService;

        public CatalogueSolutionsController(
            ISolutionsService solutionsService,
            ISuppliersService suppliersService,
            ICapabilitiesService capabilitiesService,
            ISolutionPublicationStatusService publicationStatusService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
            this.publicationStatusService = publicationStatusService ?? throw new ArgumentNullException(nameof(publicationStatusService));
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string search = null)
        {
            var solutions = await GetFilteredItems(search);

            var solutionModel = new CatalogueSolutionsModel(solutions)
            {
                SearchTerm = search,
            };

            return View(solutionModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CatalogueSolutionsModel model)
        {
            var status = string.IsNullOrWhiteSpace(model.SelectedPublicationStatus)
                ? (PublicationStatus?)null
                : Enum.Parse<PublicationStatus>(model.SelectedPublicationStatus, ignoreCase: true);

            var solutions = await solutionsService.GetAllSolutions(status);
            var filteredSolutions = await GetFilteredItems(model.SearchTerm);

            model.SetSolutions(solutions.Intersect(filteredSolutions));

            return View(model);
        }

        [HttpGet("search-results")]
        public async Task<IActionResult> SearchResults([FromQuery] string search)
        {
            var results = await GetFilteredItems(search);

            return Json(results.Take(15).Select(x => new SuggestionSearchResult
            {
                Title = x.Name,
                Category = x.Supplier.Name,
                Url = Url.Action(nameof(ManageCatalogueSolution), new { solutionId = $"{x.Id}" }),
            }));
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

            if (solution.PublishedStatus == model.SelectedPublicationStatus) return RedirectToAction(nameof(Index));

            await publicationStatusService.SetPublicationStatus(solutionId, model.SelectedPublicationStatus);

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
                model.IsPilotSolution,
                model.Frameworks);

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

        [HttpGet("manage/{solutionId}/application-type")]
        public async Task<IActionResult> ApplicationType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ApplicationTypeSectionModel(solution)
            {
                BackLink = Url.Action(nameof(ManageCatalogueSolution), new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("manage/{solutionId}/application-type/add-application-type")]
        public async Task<IActionResult> AddApplicationType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ClientApplicationTypeSelectionModel(solution)
            {
                BackLink = Url.Action(nameof(ApplicationType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/add-application-type")]
        public async Task<IActionResult> AddApplicationType(CatalogueItemId solutionId, ClientApplicationTypeSelectionModel model)
        {
            if (!ModelState.IsValid)
            {
                var solution = await solutionsService.GetSolutionThin(solutionId);
                var erroredModel = new ClientApplicationTypeSelectionModel(solution)
                {
                    BackLink = Url.Action(nameof(ApplicationType), new { solutionId }),
                };
                return View(erroredModel);
            }

            return model.SelectedApplicationType switch
            {
                EntityFramework.Catalogue.Models.ApplicationType.BrowserBased => RedirectToAction(
                    nameof(BrowserBasedController.BrowserBased),
                    typeof(BrowserBasedController).ControllerName(),
                    new { solutionId }),
                EntityFramework.Catalogue.Models.ApplicationType.MobileTablet => RedirectToAction(
                    nameof(MobileTabletBasedController.MobileTablet),
                    typeof(MobileTabletBasedController).ControllerName(),
                    new { solutionId }),
                EntityFramework.Catalogue.Models.ApplicationType.Desktop => RedirectToAction(
                    nameof(DesktopBasedController.Desktop),
                    typeof(DesktopBasedController).ControllerName(),
                    new { solutionId }),
                _ => RedirectToAction(nameof(ApplicationType), new { solutionId }),
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

            var capabilities = await capabilitiesService.GetCapabilitiesByCategory();

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

            return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionId });
        }

        private async Task<IEnumerable<CatalogueItem>> GetFilteredItems(string search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? await solutionsService.GetAllSolutions()
                : await solutionsService.GetAllSolutionsForSearchTerm(search);
        }
    }
}
