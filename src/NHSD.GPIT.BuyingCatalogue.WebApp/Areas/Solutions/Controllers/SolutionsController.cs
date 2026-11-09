using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("catalogue-solutions")]
    public sealed class SolutionsController(
        ISolutionsService solutionsService,
        IListPriceService listPriceService,
        IAdditionalServicesService additionalServicesService,
        ISolutionsFilterService solutionsFilterService,
        IFrameworkService frameworkService,
        IIntegrationsService integrationsService,
        ISolutionStandardsService solutionStandardsService,
        IAssociatedServicesService associatedServicesService,
        ISolutionSectionsService solutionSectionsService)
        : Controller
    {
        private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        private readonly IListPriceService listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
        private readonly IAdditionalServicesService additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
        private readonly ISolutionsFilterService solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
        private readonly IFrameworkService frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        private readonly IIntegrationsService integrationsService = integrationsService ?? throw new ArgumentNullException(nameof(integrationsService));
        private readonly ISolutionStandardsService solutionStandardsService = solutionStandardsService ?? throw new ArgumentNullException(nameof(solutionStandardsService));
        private readonly IAssociatedServicesService associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
        private readonly ISolutionSectionsService solutionSectionsService = solutionSectionsService ?? throw new ArgumentNullException(nameof(solutionSectionsService));

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string solutionType,
            [FromQuery] string page,
            [FromQuery] string sortBy,
            [FromQuery] string selected,
            [FromQuery] string search,
            [FromQuery] string selectedFrameworkId,
            [FromQuery] string selectedApplicationTypeIds,
            [FromQuery] string selectedHostingTypeIds,
            [FromQuery] string selectedIntegrations)
        {
            var filters = new RequestedFilters(
                selected,
                search,
                selectedFrameworkId,
                selectedApplicationTypeIds,
                selectedHostingTypeIds,
                selectedIntegrations,
                sortBy);

            var inputOptions = new PageOptions(page, sortBy)
            {
                PageSize = 10,
            };

            var solutionTypeEnum = Enum.Parse<SolutionType>(solutionType, ignoreCase: true);

            var solutionsFilters = new SolutionsFilters
            {
                CapabilitiesAndEpics = filters.GetCapabilityAndEpicIds(),
                Search = search,
                SelectedFrameworkId = selectedFrameworkId,
                SelectedApplicationTypeIds = selectedApplicationTypeIds,
                SelectedHostingTypeIds = selectedHostingTypeIds,
                SelectedIntegrationsAndTypes = filters.GetIntegrationsAndTypes(),
                SolutionType = solutionTypeEnum,
            };

            (IList<CatalogueItem> catalogueItems, PageOptions options, _) =
                await solutionsFilterService.GetAllSolutionsFiltered(
                    solutionsFilters,
                    inputOptions);

            var frameworks = await frameworkService.GetFrameworksWithPublishedCatalogueItems();
            var integrations = await integrationsService.GetIntegrationsWithTypes();

            var additionalFilters = new AdditionalFiltersModel(frameworks, filters, integrations);

            var viewName = solutionTypeEnum switch
            {
                SolutionType.CommunityPharmacy => "Solutions",
                _ => "Index",
            };

            return View(
                viewName,
                new SolutionsModel
                {
                    AdditionalFilters = additionalFilters,
                    ResultsModel = new SolutionsResultsModel
                    {
                        PageOptions = options,
                        CatalogueItems = catalogueItems,
                        Filters = filters,
                    },
                });
        }

        [HttpPost]
        public IActionResult Index(AdditionalFiltersModel model)
        {
            return RedirectToAction(
                nameof(Index),
                typeof(SolutionsController).ControllerName(),
                model.ToRequestedFilters().ToRouteValues());
        }

        [HttpPost("select-capabilities")]
        public IActionResult SelectCapabilities(
            AdditionalFiltersModel model)
        {
            return RedirectToAction(
                nameof(FilterController.FilterCapabilities),
                typeof(FilterController).ControllerName(),
                model.ToRequestedFilters().ToRouteValues());
        }

        [HttpPost("select-epics")]
        public IActionResult SelectEpics(
            AdditionalFiltersModel model)
        {
            return RedirectToAction(
                nameof(FilterController.FilterEpics),
                typeof(FilterController).ControllerName(),
                model.ToRequestedFilters().ToRouteValues());
        }

        [HttpGet("search-suggestions")]
        public async Task<IActionResult> FilterSearchSuggestions([FromQuery] string search)
        {
            var currentPageUrl = new UriBuilder(HttpContext.Request.Headers.Referer.ToString());
            var query = QueryHelpers.ParseQuery(currentPageUrl.Query);

            var solutionType = query.TryGetValue("solutionType", out var value)
                ? Enum.Parse<SolutionType>(value.ToString(), ignoreCase: true)
                : SolutionType.GPIT;

            var results = await solutionsFilterService.GetSolutionsBySearchTerm(search, solutionType: solutionType);

            return Json(
                results.Select(
                    r =>
                        new HtmlEncodedSuggestionSearchResult(
                            r.Title,
                            r.Category,
                            currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).Uri.PathAndQuery)));
        }

        [HttpGet("search-results")]
        public async Task<IActionResult> SearchResults(
            RequestedFilters filters = null)
        {
            var inputOptions = new PageOptions(null, filters.SortBy)
            {
                PageSize = 10,
            };

            var solutionsFilters = new SolutionsFilters
            {
                CapabilitiesAndEpics = filters.GetCapabilityAndEpicIds(),
                Search = filters.Search,
                SelectedFrameworkId = filters.SelectedFrameworkId,
                SelectedApplicationTypeIds = filters.SelectedApplicationTypeIds,
                SelectedHostingTypeIds = filters.SelectedHostingTypeIds,
                SelectedIntegrationsAndTypes = filters.GetIntegrationsAndTypes(),
                SolutionType = SolutionType.GPIT,
            };

            (IList<CatalogueItem> catalogueItems, PageOptions options, _) =
                await solutionsFilterService.GetAllSolutionsFiltered(
                    solutionsFilters,
                    inputOptions);

            var model = new SolutionsResultsModel()
            {
                PageOptions = options,
                CatalogueItems = catalogueItems,
                Filters = filters,
            };

            return PartialView(model);
        }

        [HttpGet("solution-sort")]
        public IActionResult SolutionSort()
        {
            var model = new SolutionSortModel();

            return PartialView(model);
        }

        [HttpGet("{solutionId}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithBasicInformation(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);

            var associatedServices = await solutionsService.GetPublishedAssociatedServicesForSolution(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(solution.Solution.SolutionType);

            return View(new AssociatedServicesModel(solution, associatedServices, contentStatus, sections));
        }

        [HttpGet("{solutionId}/associated-services/{serviceId}/price")]
        public async Task<IActionResult> AssociatedServicePrice(CatalogueItemId solutionId, CatalogueItemId serviceId)
        {
            var item = await solutionsService.GetSolutionWithCataloguePrice(solutionId);
            var associatedService = await listPriceService.GetCatalogueItemWithListPrices(serviceId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {serviceId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { serviceId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View("ListPrice", new ListPriceModel(item, associatedService, contentStatus, sections)
            {
                BackLink = Url.Action(
                    nameof(AssociatedServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId }),
                IndexValue = 5,
                Caption = associatedService.Name,
            });
        }

        [HttpGet("{solutionId}/additional-services")]
        public async Task<IActionResult> AdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithBasicInformation(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(solution.Solution.SolutionType);

            var additionalServices =
                await solutionsService.GetPublishedAdditionalServicesForSolution(solutionId);

            return View(new AdditionalServicesModel(solution, additionalServices, contentStatus, sections));
        }

        [HttpGet("{solutionId}/additional-services/{serviceId}/price")]
        public async Task<IActionResult> AdditionalServicePrice(CatalogueItemId solutionId, CatalogueItemId serviceId)
        {
            var item = await solutionsService.GetSolutionWithCataloguePrice(solutionId);
            var additionalService = await listPriceService.GetCatalogueItemWithListPrices(serviceId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {serviceId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { serviceId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View("ListPrice", new ListPriceModel(item, additionalService, contentStatus, sections)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId }),
                IndexValue = 4,
                Caption = additionalService.Name,
            });
        }

        [HttpGet("{solutionId}/additional-services/{additionalServiceId}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No additional service found for Id: {additionalServiceId}");

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            var associatedServices =
                await associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(additionalServiceId);

            var model =
                new AdditionalAssociatedServicesModel(item, additionalService, associatedServices, contentStatus, sections)
                {
                    BackLink = Url.Action(nameof(AdditionalServices), new { solutionId, additionalServiceId }),
                };

            return View(model);
        }

        [HttpGet(
            "{solutionId}/additional-services/{additionalServiceId}/associated-services/{associatedServiceId}/price")]
        public async Task<IActionResult> AssociatedServicePrice(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            CatalogueItemId associatedServiceId)
        {
            var item = await solutionsService.GetSolutionThin(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No additional service found for Id: {additionalServiceId}");
            if (additionalService.SupplierServiceAssociations.All(x => x.AssociatedServiceId != associatedServiceId))
                return BadRequest($"No associated service found for Id: {associatedServiceId}");

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);
            var associatedService = await listPriceService.GetCatalogueItemWithListPrices(associatedServiceId);

            return View("ListPrice", new ListPriceModel(item, associatedService, contentStatus, sections)
            {
                BackLink = Url.Action(
                    nameof(AssociatedServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId, additionalServiceId }),
                IndexValue = 4,
                Caption = associatedService.Name,
            });
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
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            var model = new CapabilitiesViewModel(item, contentStatus, sections)
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
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(
                new CapabilitiesViewModel(solution, item, contentStatus, sections)
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
            {
                return BadRequest(
                    $"No Catalogue Item found for Id: {additionalServiceId} with Capability Id: {capabilityId}");
            }

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

        [HttpGet("{solutionId}/application-types")]
        public async Task<IActionResult> ApplicationTypes(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);

            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            var model = new ApplicationTypesModel(item, contentStatus, sections);

            return View(model);
        }

        [HttpGet("{solutionId}")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            var viewName = item.Solution.SolutionType switch
            {
                SolutionType.CommunityPharmacy => "Summary",
                _ => "Description",
            };

            return View(viewName, new SolutionDescriptionModel(item, contentStatus, sections));
        }

        [HttpGet("{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new SolutionFeaturesModel(item, contentStatus, sections));
        }

        [HttpGet("{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new HostingTypesModel(item, contentStatus, sections));
        }

        [HttpGet("{solutionId}/data-processing-information")]
        public async Task<IActionResult> DataProcessingInformation(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithDataProcessingInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new DataProcessingInformationModel(item, contentStatus, sections));
        }

        [HttpGet("{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new ImplementationTimescalesModel(item, contentStatus, sections));
        }

        [HttpGet("{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new InteroperabilityModel(item, contentStatus, sections));
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
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new ListPriceModel(item, contentStatus, sections));
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
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            var model = new ServiceLevelAgreementDetailsModel(item, contentStatus, sections);

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
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new SolutionSupplierDetailsModel(item, contentStatus, sections));
        }

        [HttpGet("{solutionId}/standards")]
        public async Task<IActionResult> Standards(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var standards = await solutionStandardsService.GetSolutionStandards(solutionId);

            var standardsWithWorkOffPlans =
                (await solutionsService.GetWorkOffPlans(solutionId)).Select(wp => wp.StandardId);

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            return View(new SolutionStandardsModel(item, standards, standardsWithWorkOffPlans, contentStatus, sections));
        }

        [HttpGet("{solutionId}/development-plans")]
        public async Task<IActionResult> DevelopmentPlans(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithBasicInformation(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var contentStatus = await solutionsService.GetContentStatusForCatalogueItem(solutionId);
            var sections = await solutionSectionsService.GetSolutionSectionsAsync(item.Solution.SolutionType);

            var workOffPlans = await solutionsService.GetWorkOffPlans(solutionId);

            return View(new DevelopmentPlansModel(item, workOffPlans, contentStatus, sections));
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
