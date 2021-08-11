using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("solutions")]
    public sealed class SolutionDetailsController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public SolutionDetailsController(IMapper mapper, ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("futures/{solutionId}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithAllAssociatedServices(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AssociatedServicesModel>(solution));
        }

        [HttpGet("futures/{solutionId}/additional-services")]
        public async Task<IActionResult> AdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithAllAdditionalServices(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AdditionalServicesModel>(solution));
        }

        [HttpGet("futures/{solutionId}/capabilities")]
        public async Task<IActionResult> Capabilities(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, CapabilitiesViewModel>(solution));
        }

        [HttpGet("futures/{solutionId}/additional-services/{additionalServiceId}/capabilities")]
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

        [HttpGet("futures/{solutionId}/capability/{capabilityId}")]
        public async Task<IActionResult> CheckEpics(CatalogueItemId solutionId, int capabilityId)
        {
            var solution = await solutionsService.GetSolutionCapability(solutionId, capabilityId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId} with Capability Id: {capabilityId}");

            var model = mapper.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(
                solution.CatalogueItemCapability(capabilityId));

            return View(model.WithSolutionName(solution.Name));
        }

        [HttpGet("futures/{solutionId}/additional-services/{additionalServiceId}/capability/{capabilityId}")]
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

        [HttpGet("futures/{solutionId}/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution);

            return View(model);
        }

        [HttpGet("futures/{solutionId}")]
        public async Task<IActionResult> Description(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution);

            return View(model);
        }

        [HttpGet("futures/{solutionId}/features")]
        public async Task<IActionResult> Features(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, SolutionFeaturesModel>(solution));
        }

        [HttpGet("futures/{solutionId}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, HostingTypesModel>(solution));
        }

        [HttpGet("futures/{solutionId}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [HttpGet("futures/{solutionId}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(new InteroperabilityModel(solution));
        }

        [HttpGet("futures/{solutionId}/list-price")]
        public async Task<IActionResult> ListPrice(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, ListPriceModel>(solution));
        }

        [HttpGet("futures/{solutionId}/supplier-details")]
        public async Task<IActionResult> SupplierDetails(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");
            return View(mapper.Map<CatalogueItem, SolutionSupplierDetailsModel>(solution));
        }

        [HttpGet("futures/foundation/{solutionId}")]
        public async Task<IActionResult> FoundationSolutionDetail(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [HttpGet("dfocvc/{solutionId}")]
        public async Task<IActionResult> DfocvcSolutionDetail(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [HttpGet("vaccinations/{solutionId}")]
        public async Task<IActionResult> VaccinationsSolutionDetail(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [HttpGet("preview/{solutionId}")]
        public async Task<IActionResult> PreviewSolutionDetail(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionOverview(solutionId);

            var model = new SolutionDetailModel(solution) { BackLink = $"/marketing/supplier/solution/{solutionId}", };

            return View("SolutionDetail", model);
        }
    }
}
