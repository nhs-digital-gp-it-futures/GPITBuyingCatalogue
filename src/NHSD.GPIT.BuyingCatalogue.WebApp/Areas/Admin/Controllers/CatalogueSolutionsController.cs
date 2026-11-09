using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "ManageCatalogueSolutions")]
[Area("Admin")]
[Route("admin/catalogue-solutions")]
public sealed class CatalogueSolutionsController(
    ISolutionsService solutionsService,
    ISuppliersService suppliersService,
    ICapabilitiesService capabilitiesService,
    IAssociatedServicesService associatedServicesService,
    ISolutionPublicationStatusService publicationStatusService,
    ISolutionSectionsService solutionSectionsService)
    : Controller
{
    private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
    private readonly ISuppliersService suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
    private readonly ICapabilitiesService capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
    private readonly IAssociatedServicesService associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
    private readonly ISolutionPublicationStatusService publicationStatusService = publicationStatusService ?? throw new ArgumentNullException(nameof(publicationStatusService));
    private readonly ISolutionSectionsService solutionSectionsService = solutionSectionsService ?? throw new ArgumentNullException(nameof(solutionSectionsService));

    [HttpGet("{solutionType}")]
    public async Task<IActionResult> Index([FromRoute] SolutionType solutionType, [FromQuery] string search = null)
   {
        var solutions = await GetFilteredItems(search, solutionType);

        var solutionModel = new CatalogueSolutionsModel(solutions)
        {
            SearchTerm = search,
            SolutionType = solutionType,
        };

        return View(solutionModel);
    }

    [HttpPost("{solutionType}")]
    public async Task<IActionResult> Index([FromRoute] SolutionType solutionType, CatalogueSolutionsModel model)
    {
        var status = string.IsNullOrWhiteSpace(model.SelectedPublicationStatus)
            ? (PublicationStatus?)null
            : Enum.Parse<PublicationStatus>(model.SelectedPublicationStatus, ignoreCase: true);

        var solutions = await solutionsService.GetAllSolutions(solutionType, status);
        var filteredSolutions = await GetFilteredItems(model.SearchTerm, solutionType);

        model.SetSolutions(solutions.Intersect(filteredSolutions));

        return View(model);
    }

    [HttpGet("{solutionType}/search-results")]
    public async Task<IActionResult> SearchResults([FromRoute] SolutionType solutionType, [FromQuery] string search)
    {
        var results = await GetFilteredItems(search, solutionType);

        return Json(results.Take(15).Select(x => new HtmlEncodedSuggestionSearchResult(
            x.Name,
            x.Supplier.Name,
            Url.Action(nameof(ManageCatalogueSolution), new { solutionType, solutionId = $"{x.Id}" }))));
    }

    [HttpGet("manage/{solutionType}/{solutionId}/features")]
    public async Task<IActionResult> Features(SolutionType solutionType, CatalogueItemId solutionId)
    {
        var solution = await solutionsService.GetSolutionThin(solutionId);
        if (solution is null)
            return BadRequest($"No Solution found for Id: {solutionId}");

        var model = new FeaturesModel(solution)
        {
            BackLink = Url.Action(
                nameof(ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionType, solutionId }),
        };

        return View(model);
    }

    [HttpPost("manage/{solutionType}/{solutionId}/features")]
    public async Task<IActionResult> Features(SolutionType solutionType, CatalogueItemId solutionId, FeaturesModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await solutionsService.SaveSolutionFeatures(solutionId, model.AllFeatures);

        return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionType, solutionId });
    }

    [HttpGet("manage/{solutionType}/{solutionId}")]
    public async Task<IActionResult> ManageCatalogueSolution(SolutionType solutionType, CatalogueItemId solutionId)
    {
        var solution = await solutionsService.GetSolutionThin(solutionId);
        if (solution is null)
            return BadRequest($"No Solution found for Id: {solutionId}");

        var solutionSections = await solutionSectionsService.GetSolutionSectionsAsync(solutionType);
        if (solutionSections is null)
            return BadRequest($"No Solution Sections found for Id: {solutionId}, with Solution Type: {solutionType}");

        var solutionStatuses = await solutionsService.GetSolutionLoadingStatuses(solutionId, solutionSections);

        var model = new ManageCatalogueSolutionModel(solutionStatuses, solution, solutionSections)
        {
            SolutionType = solutionType,
        };

        return View(model);
    }

    [HttpPost("manage/{solutionType}/{solutionId}")]
    public async Task<IActionResult> SetPublicationStatus(SolutionType solutionType, CatalogueItemId solutionId, ManageCatalogueSolutionModel model)
    {
        if (!ModelState.IsValid)
            return View("ManageCatalogueSolution", model);

        var solution = await solutionsService.GetSolutionThin(solutionId);

        if (solution.PublishedStatus == model.SelectedPublicationStatus) return RedirectToAction(nameof(Index), new { solutionType });

        await publicationStatusService.SetPublicationStatus(solutionId, model.SelectedPublicationStatus);

        return RedirectToAction(nameof(Index), new { solutionType });
    }

    [HttpGet("manage/{solutionType}/{solutionId}/details")]
    public async Task<IActionResult> Details(SolutionType solutionType, CatalogueItemId solutionId)
    {
        var solution = await solutionsService.GetSolutionWithBasicInformation(solutionId);

        if (solution is null)
            return BadRequest($"No Solution found for Id: {solutionId}");

        var suppliers = await suppliersService.GetAllSuppliers();

        var model = new SolutionModel(solution)
        {
            SolutionType = solutionType,
        }.WithSelectListItems(suppliers);

        model.Frameworks = (await solutionsService.GetAllFrameworks())
            .Select(
                f =>
                {
                    FrameworkSolution sol =
                        solution.Solution.FrameworkSolutions.FirstOrDefault(fs => fs.FrameworkId == f.Id);
                    return new FrameworkModel
                    {
                        Name = $"{f.ShortName} Framework", FrameworkId = f.Id, Selected = sol is not null,
                    };
                })
            .ToList();

        return View(model);
    }

    [HttpPost("manage/{solutionType}/{solutionId}/details")]
    public async Task<IActionResult> Details(SolutionType solutionType, CatalogueItemId solutionId, SolutionModel model)
    {
        if (!ModelState.IsValid)
        {
            var suppliers = await suppliersService.GetAllSuppliers();

            model.Frameworks = (await solutionsService.GetAllFrameworks())
                .Select(f => new FrameworkModel { Name = $"{f.ShortName} Framework", FrameworkId = f.Id })
                .ToList();

            return View(model.WithSelectListItems(suppliers));
        }

        await solutionsService.SaveSolutionDetails(
            solutionId,
            model.SolutionName,
            model.SupplierId.GetValueOrDefault(),
            model.IsPilotSolution,
            model.SelectedCategory,
            model.Frameworks);

        return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionType, solutionId });
    }

    [HttpGet("manage/{solutionType}/{solutionId}/description")]
    public async Task<IActionResult> Description(SolutionType solutionType, CatalogueItemId solutionId)
    {
        var solution = await solutionsService.GetSolutionThin(solutionId);

        if (solution is null)
            return BadRequest($"No Solution found for Id: {solutionId}, solution type: {solutionType}");

        return View(new DescriptionModel(solution));
    }

    [HttpPost("manage/{solutionType}/{solutionId}/description")]
    public async Task<IActionResult> Description(SolutionType solutionType, CatalogueItemId solutionId, DescriptionModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await solutionsService.SaveSolutionDescription(
            solutionId,
            model.Summary,
            model.Description,
            model.Link);

        return RedirectToAction(nameof(ManageCatalogueSolution), new { solutionType, solutionId });
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

        var model = new ApplicationTypeSelectionModel(solution)
        {
            BackLink = Url.Action(nameof(ApplicationType), new { solutionId }),
        };

        return View(model);
    }

    [HttpPost("manage/{solutionId}/application-type/add-application-type")]
    public async Task<IActionResult> AddApplicationType(CatalogueItemId solutionId, ApplicationTypeSelectionModel model)
    {
        if (!ModelState.IsValid)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            var erroredModel = new ApplicationTypeSelectionModel(solution)
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
        {
            var capabilities = await capabilitiesService.GetCapabilitiesByCategory();
            EditCapabilitiesModel.RepopulateCapabilityCategories(model, capabilities);
            return View(model);
        }

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

    [HttpGet("manage/{solutionId}/associated-services")]
    public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
    {
        var catalogueItem = await solutionsService.GetCatalogueItemWithSupplierServiceAssociations(solutionId);
        if (catalogueItem is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSupplier(catalogueItem.Supplier.Id);

        var model = new CatalogueItemAssociatedServicesModel(catalogueItem, associatedServices)
        {
            BackLink = Url.Action(
                nameof(ManageCatalogueSolution),
                new { solutionId }),
        };

        return View(model);
    }

    [HttpPost("manage/{solutionId}/associated-services")]
    public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId, CatalogueItemAssociatedServicesModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.SelectableAssociatedServices is null)
        {
            return RedirectToAction(
                nameof(ManageCatalogueSolution),
                new { solutionId });
        }

        var associatedServices = model.SelectableAssociatedServices.Where(a => a.Selected).Select(a => a.CatalogueItemId);
        await associatedServicesService.RelateAssociatedServicesToCatalogueItem(solutionId, associatedServices);

        return RedirectToAction(
            nameof(ManageCatalogueSolution),
            new { solutionId });
    }

    private async Task<IEnumerable<CatalogueItem>> GetFilteredItems(string search, SolutionType solutionType)
    {
        return string.IsNullOrWhiteSpace(search)
            ? await solutionsService.GetAllSolutions(solutionType)
            : await solutionsService.GetAllSolutionsForSearchTerm(solutionType, search);
    }
}
