using System;
using System.Linq;
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

        [Route("futures/{id}/associated-services")]
        public async Task<IActionResult> AssociatedServices(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionWithAllAssociatedServices(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, AssociatedServicesModel>(solution));
        }

        [Route("futures/{id}/additional-services")]
        public async Task<IActionResult> AdditionalServices(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionWithAllAdditionalServices(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, AdditionalServicesModel>(solution));
        }

        [Route("futures/{id}/capabilities")]
        public async Task<IActionResult> Capabilities(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, CapabilitiesViewModel>(solution));
        }

        [Route("futures/{catalogueItemId}/additional-services/{additionalServiceId}/capabilities")]
        public async Task<IActionResult> CapabilitiesAdditionalServices(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolutionAdditionalServiceCapabilities(catalogueItemId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {catalogueItemId}");

            var viewModel = mapper.Map<CatalogueItem, CapabilitiesViewModel>(solution);

            viewModel.Name = solution.CatalogueItemName(additionalServiceId);

            viewModel.Description = solution.AdditionalServiceDescription(additionalServiceId);

            return View(viewModel);
        }

        [Route("futures/{catalogueItemId}/capability/{capabilityId:guid}")]
        public async Task<IActionResult> CheckEpics(CatalogueItemId catalogueItemId, Guid capabilityId)
        {
            var solution = await solutionsService.GetSolutionCapability(catalogueItemId, capabilityId);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");

            var model = mapper.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(
                solution.CatalogueItemCapability(capabilityId));

            return View(model.WithSolutionName(solution.Name));
        }

        [Route("futures/{catalogueItemId}/additional-services/{catalogueItemIdAdditional}/capability/{capabilityId:guid}")]
        public async Task<IActionResult> CheckEpicsAdditionalServices(
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            Guid capabilityId)
        {
            var solution = await solutionsService.GetAdditionalServiceCapability(
                catalogueItemIdAdditional,
                capabilityId);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");

            var model = mapper.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(
                solution.CatalogueItemCapability(capabilityId));

            return View(
                "CheckEpics",
                model.WithItems(catalogueItemId, catalogueItemIdAdditional, solution.Name));
        }

        [Route("futures/{id}/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution);

            return View(model);
        }

        [Route("futures/{id}")]
        public async Task<IActionResult> Description(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution);

            return View(model);
        }

        [Route("futures/{id}/features")]
        public async Task<IActionResult> Features(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, SolutionFeaturesModel>(solution));
        }

        [Route("futures/{id}/hosting-type")]
        public async Task<IActionResult> HostingType(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, HostingTypesModel>(solution));
        }

        [Route("futures/{id}/implementation")]
        public async Task<IActionResult> Implementation(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [Route("futures/{id}/interoperability")]
        public async Task<IActionResult> Interoperability(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(new InteroperabilityModel(solution));
        }

        [Route("futures/{id}/list-price")]
        public async Task<IActionResult> ListPrice(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ListPriceModel>(solution));
        }

        [Route("futures/{id}/supplier-details")]
        public async Task<IActionResult> SupplierDetails(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, SolutionSupplierDetailsModel>(solution));
        }

        [Route("futures/foundation/{id}")]
        public async Task<IActionResult> FoundationSolutionDetail(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [Route("dfocvc/{id}")]
        public async Task<IActionResult> DVOCVCSolutionDetail(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [Route("vaccinations/{id}")]
        public async Task<IActionResult> VaccinationsSolutionDetail(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [Route("preview/{id}")]
        public async Task<IActionResult> PreviewSolutionDetail(CatalogueItemId id)
        {
            var solution = await solutionsService.GetSolutionOverview(id);

            var model = new SolutionDetailModel(solution) { BackLink = $"/marketing/supplier/solution/{id}", };

            return View("SolutionDetail", model);
        }
    }
}
