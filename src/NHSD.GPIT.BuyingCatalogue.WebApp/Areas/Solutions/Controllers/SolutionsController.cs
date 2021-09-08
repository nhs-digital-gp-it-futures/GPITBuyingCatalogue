using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("catalogue-solutions")]
    public sealed class SolutionsController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;
        private readonly ISolutionsFilterService solutionsFilterService;
        private readonly IMemoryCache memoryCache;
        private readonly FilterCacheKeySettings filterCacheKey;
        private readonly MemoryCacheEntryOptions memoryCacheOptions;

        public SolutionsController(
            IMapper mapper,
            ISolutionsService solutionsService,
            IMemoryCache memoryCache,
            ISolutionsFilterService solutionsFilterService,
            FilterCacheKeySettings filterCacheKey)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
            this.filterCacheKey = filterCacheKey ?? throw new ArgumentNullException(nameof(filterCacheKey));
            memoryCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(60));
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

            var categories = await solutionsFilterService.GetAllCategoriesAndCountForFilter(selectedFramework);

            return View(new SolutionsModel(frameworks)
            {
                CatalogueItems = solutions.Items,
                Options = solutions.Options,
                SelectedFramework = selectedFramework ?? "All",
                CategoryFilters = categories.CategoryFilters,
                FoundationCapabilities = categories.FoundationCapabilities,
                CountOfSolutionsWithFoundationCapability = categories.CountOfCatalogueItemsWithFoundationCapabilities,
                SelectedCapabilities = capabilities,
            });
        }

        [HttpGet("filter")]
        public async Task<IActionResult> LoadCatalogueSolutionsFilter([FromQuery] string selectedFramework)
        {
            var cacheKey = $"{filterCacheKey.FilterCacheKey}{selectedFramework ?? "All"}";

            if (memoryCache.TryGetValue(cacheKey, out string html))
                return Content(html);

            var frameworks = await solutionsFilterService.GetAllFrameworksAndCountForFilter();

            var categories = await solutionsFilterService.GetAllCategoriesAndCountForFilter(selectedFramework);

            var result = await this.RenderViewAsync(
                "_FilterJavascript",
                new SolutionsModel(frameworks)
                {
                    SelectedFramework = selectedFramework ?? "All",
                    CategoryFilters = categories.CategoryFilters,
                    FoundationCapabilities = categories.FoundationCapabilities,
                    CountOfSolutionsWithFoundationCapability = categories.CountOfCatalogueItemsWithFoundationCapabilities,
                },
                true);

            memoryCache.Set(cacheKey, result, memoryCacheOptions);

            return Content(result);
        }

        [HttpGet("{solutionId}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithAllAssociatedServices(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AssociatedServicesModel>(solution));
        }

        [HttpGet("{solutionId}/additional-services")]
        public async Task<IActionResult> AdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithAllAdditionalServices(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AdditionalServicesModel>(solution));
        }

        [HttpGet("{solutionId}/capabilities")]
        public async Task<IActionResult> Capabilities(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, CapabilitiesViewModel>(solution));
        }

        [HttpGet("{solutionId}/additional-services/{additionalServiceId}/capabilities")]
        public async Task<IActionResult> CapabilitiesAdditionalServices(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolutionAdditionalServiceCapabilities(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var viewModel = mapper.Map<CatalogueItem, CapabilitiesViewModel>(solution);

            viewModel.Name = solution.CatalogueItemName(additionalServiceId);

            viewModel.Description = solution.AdditionalServiceDescription(additionalServiceId);

            return View(viewModel);
        }

        [HttpGet("{solutionId}/capability/{capabilityId}")]
        public async Task<IActionResult> CheckEpics(CatalogueItemId solutionId, int capabilityId)
        {
            var solution = await solutionsService.GetSolutionCapability(solutionId, capabilityId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId} with Capability Id: {capabilityId}");

            var model = mapper.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(
                solution.CatalogueItemCapability(capabilityId));

            return View(model.WithSolutionName(solution.Name));
        }

        [HttpGet("{solutionId}/additional-services/{additionalServiceId}/capability/{capabilityId}")]
        public async Task<IActionResult> CheckEpicsAdditionalServices(
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId,
            int capabilityId)
        {
            var solution = await solutionsService.GetAdditionalServiceCapability(
                additionalServiceId,
                capabilityId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId} with Capability Id: {capabilityId}");

            var model = mapper.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(
                solution.CatalogueItemCapability(capabilityId));

            return View(
                "CheckEpics",
                model.WithItems(solutionId, additionalServiceId, solution.Name));
        }

        [HttpGet("{solutionId}/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution);

            return View(model);
        }

        [HttpGet("{solutionId}")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution);

            return View(model);
        }

        [HttpGet("{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, SolutionFeaturesModel>(solution));
        }

        [HttpGet("{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, HostingTypesModel>(solution));
        }

        [HttpGet("{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [HttpGet("{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(new InteroperabilityModel(solution));
        }

        [HttpGet("{solutionId}/list-price")]
        public async Task<IActionResult> ListPrice(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, ListPriceModel>(solution));
        }

        [HttpGet("{solutionId}/supplier-details")]
        public async Task<IActionResult> SupplierDetails(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, SolutionSupplierDetailsModel>(solution));
        }
    }
}
