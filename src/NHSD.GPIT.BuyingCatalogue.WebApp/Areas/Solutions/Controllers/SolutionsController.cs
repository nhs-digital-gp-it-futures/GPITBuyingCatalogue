using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("catalogue-solutions")]
    public sealed class SolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly ISolutionsFilterService solutionsFilterService;
        private readonly IFrameworkService frameworkService;

        public SolutionsController(
            ISolutionsService solutionsService,
            IAdditionalServicesService additionalServicesService,
            ISolutionsFilterService solutionsFilterService,
            IFrameworkService frameworkService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page,
            [FromQuery] string sortBy,
            [FromQuery] string selectedCapabilityIds,
            [FromQuery] string selectedEpicIds,
            [FromQuery] string search,
            string selectedFrameworkId)
        {
            var inputOptions = new PageOptions(page, sortBy);

            var (catalogueItems, options, capabilitiesAndCount) =
                await solutionsFilterService.GetAllSolutionsFiltered(
                    inputOptions,
                    selectedCapabilityIds,
                    selectedEpicIds,
                    search,
                    selectedFrameworkId);
            var (catalogueItemsWithoutFrameworkFilter, capabilitiesAndCountWithoutFrameworkFilter) = await solutionsFilterService.GetFilteredAndNonFilteredQueryResults(selectedCapabilityIds, selectedEpicIds);
            var frameworks = await frameworkService.GetFrameworksByCatalogueItems(catalogueItemsWithoutFrameworkFilter.Select(x => x.Id).ToList());
            var additionalFilters = new Models.Filters.AdditionalFiltersModel(frameworks, selectedCapabilityIds, selectedEpicIds);

            return View(new SolutionsModel()
            {
                CatalogueItems = catalogueItems,
                SelectedSortOption = options.Sort,
                PageOptions = options,
                SearchSummary = new CatalogueFilterSearchSummary(
                    capabilitiesAndCount,
                    search,
                    selectedCapabilityIds,
                    selectedEpicIds),
                AdditionalFilters = additionalFilters,
            });
        }

        [HttpPost]
        public IActionResult Index(
            SolutionsModel model,
            [FromQuery] string page,
            [FromQuery] string sortBy,
            [FromQuery] string selectedCapabilityIds,
            [FromQuery] string selectedEpicIds,
            [FromQuery] string search,
            string selectedFrameworkId) =>
            RedirectToAction(
                    nameof(Index),
                    typeof(SolutionsController).ControllerName(),
                    new
                    {
                        page,
                        selectedCapabilityIds,
                        selectedEpicIds,
                        search,
                        sortBy = model.SelectedSortOption.ToString(),
                        selectedFrameworkId,
                    });

        [HttpGet("search-suggestions")]
        public async Task<IActionResult> FilterSearchSuggestions([FromQuery] string search)
        {
            var currentPageUrl = new UriBuilder(HttpContext.Request.Headers.Referer.ToString());

            var results = await solutionsFilterService.GetSolutionsBySearchTerm(search);

            return Json(results.Select(r =>
                new SuggestionSearchResult
                {
                    Title = r.Title,
                    Category = r.Category,
                    Url = currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).ToString(),
                }));
        }

        [HttpGet("{solutionId}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var associatedServices = await solutionsService.GetPublishedAssociatedServicesForSolution(solutionId);

            return View(new AssociatedServicesModel(solution, associatedServices, contentStatus));
        }

        [HttpGet("{solutionId}/additional-services")]
        public async Task<IActionResult> AdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var additionalServices =
                await solutionsService.GetPublishedAdditionalServicesForSolution(solutionId);

            return View(new AdditionalServicesModel(solution, additionalServices, contentStatus));
        }

        [HttpGet("{solutionId}/capabilities")]
        public async Task<IActionResult> Capabilities(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithCapabilities(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var model = new CapabilitiesViewModel(item, contentStatus)
            {
                BackLinkText = NavBaseModel.BackLinkTextDefault,
                BackLink = Url.Action(
                    nameof(AdditionalServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("{solutionId}/additional-services/{additionalServiceId}/capabilities")]
        public async Task<IActionResult> CapabilitiesAdditionalServices(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);
            if (solution?.Solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var item = await additionalServicesService.GetAdditionalServiceWithCapabilities(additionalServiceId);
            if (item?.AdditionalService is null)
                return BadRequest($"No Catalogue Item found for Id: {additionalServiceId}");

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new CapabilitiesViewModel(solution, item, contentStatus)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId }),
                BackLinkText = NavBaseModel.BackLinkTextDefault,
                Name = item.Name,
                Description = item.AdditionalService.FullDescription,
            });
        }

        [HttpGet("{solutionId}/capability/{capabilityId}")]
        public async Task<IActionResult> CheckEpics(CatalogueItemId solutionId, int capabilityId)
        {
            var item = await solutionsService.GetSolutionCapability(solutionId, capabilityId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId} with Capability Id: {capabilityId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var solutionCapability = item.CatalogueItemCapabilities
                .First(cic => cic.CapabilityId == capabilityId);

            var model = new SolutionCheckEpicsModel(solutionCapability, item)
            {
                BackLink = Url.Action(
                    nameof(Capabilities),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("{solutionId}/additional-services/{additionalServiceId}/capability/{capabilityId}")]
        public async Task<IActionResult> CheckEpicsAdditionalServices(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int capabilityId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var item = await solutionsService.GetSolutionCapability(
                additionalServiceId,
                capabilityId);

            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {additionalServiceId} with Capability Id: {capabilityId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var solutionCapability = item.CatalogueItemCapabilities
                .First(cic => cic.CapabilityId == capabilityId);

            var model = new SolutionCheckEpicsModel(solutionCapability, item, additionalServiceId)
            {
                BackLink = Url.Action(
                    nameof(CapabilitiesAdditionalServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId, additionalServiceId }),
            };

            return View("CheckEpics", model);
        }

        [HttpGet("{solutionId}/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);

            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var model = new ClientApplicationTypesModel(item, contentStatus);

            return View(model);
        }

        [HttpGet("{solutionId}")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new SolutionDescriptionModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new SolutionFeaturesModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new HostingTypesModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new ImplementationTimescalesModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new InteroperabilityModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/list-price")]
        public async Task<IActionResult> ListPrice(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithCataloguePrice(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new ListPriceModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/service-level-agreements")]
        public async Task<IActionResult> ServiceLevelAgreement(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var model = new ServiceLevelAgreementDetailsModel(item, contentStatus);

            return View(model);
        }

        [HttpGet("{solutionId}/supplier-details")]
        public async Task<IActionResult> SupplierDetails(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithSupplierDetails(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new SolutionSupplierDetailsModel(item, contentStatus));
        }

        [HttpGet("{solutionId}/standards")]
        public async Task<IActionResult> Standards(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var standards = await solutionsService.GetSolutionStandardsForMarketing(solutionId);

            var standardsWithWorkOffPlans = (await solutionsService.GetWorkOffPlans(solutionId)).Select(wp => wp.StandardId);

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            return View(new SolutionStandardsModel(item, standards, standardsWithWorkOffPlans, contentStatus));
        }

        [HttpGet("{solutionId}/development-plans")]
        public async Task<IActionResult> DevelopmentPlans(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var workOffPlans = await solutionsService.GetWorkOffPlans(solutionId);

            return View(new DevelopmentPlansModel(item, workOffPlans, contentStatus));
        }

        [HttpGet("about-pilot-solutions")]
        public IActionResult AboutPilotSolutions()
        {
            var backlink = Request.Headers.Referer.ToString();
            var model = new NavBaseModel();

            if (!string.IsNullOrWhiteSpace(backlink))
                model.BackLink = backlink;

            return View(model);
        }
    }
}
