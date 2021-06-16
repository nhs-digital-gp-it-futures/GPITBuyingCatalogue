using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class SolutionDetailsController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public SolutionDetailsController(IMapper mapper, ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [Route("solutions/futures/{id}/associated-services")]
        public async Task<IActionResult> AssociatedServices(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(AssociatedServices)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, AssociatedServicesModel>(solution));
        }

        [Route("solutions/futures/{id}/capabilities")]
        public async Task<IActionResult> Capabilities(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(Capabilities)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, CapabilitiesViewModel>(solution));
        }

        [Route("solutions/futures/{id}/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(ClientApplicationTypes)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution);

            return View(model);
        }

        [Route("solutions/futures/{id}")]
        public async Task<IActionResult> Description(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(Description)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = mapper.Map<CatalogueItem, SolutionDescriptionModel>(solution);

            return View(model);
        }

        [Route("solutions/futures/{id}/features")]
        public async Task<IActionResult> Features(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(Features)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, SolutionFeaturesModel>(solution));
        }

        [Route("solutions/futures/{id}/hosting-type")]
        public async Task<IActionResult> HostingType(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(HostingType)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, HostingTypesModel>(solution));
        }

        [Route("solutions/futures/{id}/implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(ImplementationTimescales)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ImplementationTimescalesModel>(solution));
        }

        [Route("solutions/futures/{id}/interoperability")]
        public async Task<IActionResult> Interoperability(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(Interoperability)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var viewModel = mapper.Map<CatalogueItem, InteroperabilityModel>(solution);
            return View(viewModel);
        }

        [Route("solutions/futures/{id}/list-price")]
        public async Task<IActionResult> ListPrice(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"{nameof(ListPrice)}-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ListPriceModel>(solution));
        }

        [Route("solutions/futures/foundation/{id}")]
        public async Task<IActionResult> FoundationSolutionDetail(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [Route("solutions/dfocvc/{id}")]
        public async Task<IActionResult> DVOCVCSolutionDetail(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [Route("solutions/vaccinations/{id}")]
        public async Task<IActionResult> VaccinationsSolutionDetail(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            return View("SolutionDetail", new SolutionDetailModel(solution));
        }

        [Route("solutions/preview/{id}")]
        public async Task<IActionResult> PreviewSolutionDetail(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution) { BackLink = $"/marketing/supplier/solution/{id}", };

            return View("SolutionDetail", model);
        }
    }
}
