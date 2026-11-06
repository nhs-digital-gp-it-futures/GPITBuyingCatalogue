using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Controllers;

[Area("CommunityPharmacy")]
[Route("community-pharmacy")]
public class CommunityPharmacyController(ISolutionsFilterService solutionsFilterService, ISolutionsService solutionsService, ISolutionStandardsService solutionStandardsService) : Controller
{
    private readonly ISolutionsFilterService solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
    private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
    private readonly ISolutionStandardsService solutionStandardsService = solutionStandardsService ?? throw new ArgumentNullException(nameof(solutionStandardsService));

    [HttpGet("solutions")]
    public async Task<IActionResult> Solutions(
        [FromQuery] string page,
        [FromQuery] string sortBy,
        [FromQuery] string search)
    {
        var selectedSort = string.IsNullOrWhiteSpace(sortBy)
            ? PageOptions.SortOptions.LastPublished
            : Enum.Parse<PageOptions.SortOptions>(sortBy);

        var inputOptions = new PageOptions(page, selectedSort.ToString())
        {
            PageSize = CommunityPharmacySolutionsModel.SearchResultsPerPage,
        };

        var filters = new SolutionsFilters() { SolutionType = SolutionType.CommunityPharmacy, Search = search, };

        (IList<CatalogueItem> catalogueItems, PageOptions options, _) =
            await solutionsFilterService.GetAllSolutionsFiltered(
                filters,
                inputOptions);

        var model = new CommunityPharmacySolutionsModel
        {
            ResultsModel = new SolutionsResultsModel { CatalogueItems = catalogueItems, PageOptions = options },
        };

        return View(model);
    }

    [HttpGet("search-suggestions")]
    public async Task<IActionResult> FilterSearchSuggestions([FromQuery] string search)
    {
        var currentPageUrl = new UriBuilder(HttpContext.Request.Headers.Referer.ToString());

        var results = await solutionsFilterService.GetSolutionsBySearchTerm(search, isCommunityPharmacy: true);

        return Json(
            results.Select(
                r =>
                    new HtmlEncodedSuggestionSearchResult(
                        r.Title,
                        r.Category,
                        currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).Uri.PathAndQuery)));
    }

    [HttpGet("{solutionId}")]
    public async Task<IActionResult> Summary(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View(new SummaryModel(item, contentStatus));
    }

    [HttpGet("{solutionId}/list-price")]
    public async Task<IActionResult> ListPrice(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithCataloguePrice(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/ListPrice.cshtml", new CommunityPharmacyListPriceModel(item, contentStatus));
    }

    [HttpGet("{solutionId}/capabilities")]
    public async Task<IActionResult> Capabilities(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithCapabilities(solutionId);

        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/Capabilities.cshtml", new CommunityPharmacyCapabilitiesModel(item, contentStatus));
    }

    [HttpGet("{solutionId}/capability/{capabilityId}")]
    public async Task<IActionResult> CheckEpics(CatalogueItemId solutionId, int capabilityId)
    {
        var item = await solutionsService.GetSolutionCapability(solutionId, capabilityId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId} with Capability Id: {capabilityId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var solutionCapability = item.CatalogueItemCapabilities
            .First(cic => cic.CapabilityId == capabilityId);

        var model = new SolutionCheckEpicsModel(solutionCapability, item)
        {
            BackLink = Url.Action(
                nameof(Capabilities),
                typeof(CommunityPharmacyController).ControllerName(),
                new { solutionId }),
        };

        return View("~/Views/Solutions/CheckEpics.cshtml", model);
    }

    [HttpGet("{solutionId}/standards")]
    public async Task<IActionResult> Standards(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var standards = await solutionStandardsService.GetSolutionStandards(solutionId);

        var standardsWithWorkOffPlans =
            (await solutionsService.GetWorkOffPlans(solutionId)).Select(wp => wp.StandardId);

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/Standards.cshtml", new CommunityPharmacyStandardsModel(item, standards, standardsWithWorkOffPlans, contentStatus));
    }

    [HttpGet("{solutionId}/integrations")]
    public async Task<IActionResult> Integrations(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/Integrations.cshtml", new CommunityPharmacyIntegrationModel(item, contentStatus));
    }

    [HttpGet("{solutionId}/application-types")]
    public async Task<IActionResult> ApplicationTypes(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);

        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/ApplicationTypes.cshtml", new CommunityPharmacyApplicationTypesModel(item, contentStatus));
    }

    [HttpGet("{solutionId}/hosting-type")]
    public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/HostingType.cshtml", new CommunityPharmacyHostingTypeModel(item, contentStatus));
    }

    [HttpGet("{solutionId}/development-plans")]
    public async Task<IActionResult> DevelopmentPlans(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        var workOffPlans = await solutionsService.GetWorkOffPlans(solutionId);

        return View("~/Views/Solutions/DevelopmentPlans.cshtml", new CommunityPharmacyDevelopmentPlansModel(item, workOffPlans, contentStatus));
    }

    [HttpGet("{solutionId}/supplier-details")]
    public async Task<IActionResult> SupplierDetails(CatalogueItemId solutionId)
    {
        var item = await solutionsService.GetSolutionWithSupplierDetails(solutionId);
        if (item is null)
            return BadRequest($"No Catalogue Item found for Id: {solutionId}");

        if (item.PublishedStatus == PublicationStatus.Suspended)
            return RedirectToAction(nameof(Summary), new { solutionId });

        var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

        return View("~/Views/Solutions/SupplierDetails.cshtml", new CommunityPharmacySupplierDetails(item, contentStatus));
    }
}
