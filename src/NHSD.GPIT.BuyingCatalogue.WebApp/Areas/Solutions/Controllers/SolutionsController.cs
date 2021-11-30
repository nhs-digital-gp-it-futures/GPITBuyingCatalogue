using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("catalogue-solutions")]
    public sealed class SolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly ISolutionsFilterService solutionsFilterService;
        private readonly IFilterCache filterCache;

        public SolutionsController(
            ISolutionsService solutionsService,
            IFilterCache filterCache,
            ISolutionsFilterService solutionsFilterService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.filterCache = filterCache ?? throw new ArgumentNullException(nameof(filterCache));
            this.solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page,
            [FromQuery] string sortBy,
            [FromQuery] string selectedFramework,
            [FromQuery] string capabilities)
        {
            var options = new PageOptions(page, sortBy);

            var solutions = await solutionsFilterService.GetAllSolutionsFiltered(options, selectedFramework, capabilities);

            var frameworks = await solutionsFilterService.GetAllFrameworksAndCountForFilter();

            return View(new SolutionsModel(frameworks)
            {
                CatalogueItems = solutions.Items,
                Options = solutions.Options,
                SelectedFramework = selectedFramework ?? FrameworkFilterKeys.GenericCacheKey,
                CategoryFilters = default,
                FoundationCapabilities = default,
                CountOfSolutionsWithFoundationCapability = default,
                SelectedCapabilities = capabilities,
            });
        }

        [HttpGet("filter")]
        public async Task<IActionResult> LoadCatalogueSolutionsFilter([FromQuery] string selectedFramework)
        {
            var cacheKey = selectedFramework ?? FrameworkFilterKeys.GenericCacheKey;

            var html = filterCache.Get(cacheKey);

            if (html is not null)
                return Content(html);

            var frameworks = await solutionsFilterService.GetAllFrameworksAndCountForFilter();

            var categories = await solutionsFilterService.GetAllCategoriesAndCountForFilter(selectedFramework);

            var result = await this.RenderViewAsync(
                "_FilterJavascript",
                new SolutionsModel(frameworks)
                {
                    SelectedFramework = cacheKey,
                    CategoryFilters = categories.CategoryFilters,
                    FoundationCapabilities = categories.FoundationCapabilities,
                    CountOfSolutionsWithFoundationCapability = categories.CountOfCatalogueItemsWithFoundationCapabilities,
                },
                true);

            filterCache.Set(cacheKey, result);

            return Content(result);
        }

        [HttpGet("{solutionId}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithAllAssociatedServices(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new AssociatedServicesModel(item));
        }

        [HttpGet("{solutionId}/additional-services")]
        public async Task<IActionResult> AdditionalServices(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionWithAllAdditionalServices(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new AdditionalServicesModel(item));
        }

        [HttpGet("{solutionId}/capabilities")]
        public async Task<IActionResult> Capabilities(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var model = new CapabilitiesViewModel(item)
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
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var item = await solutionsService.GetSolutionAdditionalServiceCapabilities(additionalServiceId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {additionalServiceId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new CapabilitiesViewModel(solution, item)
            {
                BackLink = Url.Action(
                    nameof(AdditionalServices),
                    typeof(SolutionsController).ControllerName(),
                    new { solutionId }),
                BackLinkText = NavBaseModel.BackLinkTextDefault,
                Name = item.CatalogueItemName(additionalServiceId),
                Description = item.AdditionalServiceDescription(additionalServiceId),
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

            var solutionCapability = item.CatalogueItemCapability(capabilityId);

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
            var item = await solutionsService.GetAdditionalServiceCapability(
                additionalServiceId,
                capabilityId);

            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId} with Capability Id: {capabilityId}");

            var solution = await solutionsService.GetSolutionOverview(solutionId);

            if (solution.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var solutionCapability = item.CatalogueItemCapability(capabilityId);
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
            var item = await solutionsService.GetSolutionOverview(solutionId);

            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var model = new ClientApplicationTypesModel(item);

            return View(model);
        }

        [HttpGet("{solutionId}")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(new SolutionDescriptionModel(item));
        }

        [HttpGet("{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new SolutionFeaturesModel(item));
        }

        [HttpGet("{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new HostingTypesModel(item));
        }

        [HttpGet("{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new ImplementationTimescalesModel(item));
        }

        [HttpGet("{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new InteroperabilityModel(item));
        }

        [HttpGet("{solutionId}/list-price")]
        public async Task<IActionResult> ListPrice(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new ListPriceModel(item));
        }

        [HttpGet("{solutionId}/service-level-agreements")]
        public async Task<IActionResult> ServiceLevelAgreement(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var serviceLevelAgreement = item.Solution.ServiceLevelAgreement;
            var model = new ServiceLevelAgreementDetailsModel(
                item,
                serviceLevelAgreement.ServiceHours,
                serviceLevelAgreement.Contacts,
                serviceLevelAgreement.ServiceLevels);

            return View(model);
        }

        [HttpGet("{solutionId}/supplier-details")]
        public async Task<IActionResult> SupplierDetails(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            return View(new SolutionSupplierDetailsModel(item));
        }

        [HttpGet("{solutionId}/standards")]
        public async Task<IActionResult> Standards(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var standards = await solutionsService.GetSolutionStandardsForMarketing(solutionId);

            var standardsWithWorkOffPlans = (await solutionsService.GetWorkOffPlans(solutionId)).Select(wp => wp.StandardId);

            return View(new SolutionStandardsModel(item, standards, standardsWithWorkOffPlans));
        }

        [HttpGet("{solutionId}/development-plans")]
        public async Task<IActionResult> DevelopmentPlans(CatalogueItemId solutionId)
        {
            var item = await solutionsService.GetSolutionOverview(solutionId);
            if (item is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            if (item.PublishedStatus == PublicationStatus.Suspended)
                return RedirectToAction(nameof(Description), new { solutionId });

            var workOffPlans = await solutionsService.GetWorkOffPlans(solutionId);

            return View(new DevelopmentPlansModel(item, workOffPlans));
        }
    }
}
