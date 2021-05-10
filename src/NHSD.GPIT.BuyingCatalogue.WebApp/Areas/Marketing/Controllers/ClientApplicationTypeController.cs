using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class ClientApplicationTypeController : Controller
    {
        private readonly ILogWrapper<ClientApplicationTypeController> _logger;
        private readonly IMapper _mapper;
        private readonly ISolutionsService _solutionsService;

        public ClientApplicationTypeController(ILogWrapper<ClientApplicationTypeController> logger, IMapper mapper,
            ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution));
        }

        [HttpPost("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(ClientApplicationTypesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            _mapper.Map(model, clientApplication);

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }

        [HttpGet("browser-based")]
        public async Task<IActionResult> BrowserBased(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, BrowserBasedModel>(solution));
        }

        [HttpGet("native-mobile")]
        public async Task<IActionResult> NativeMobile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, NativeMobileModel>(solution));
        }

        [HttpGet("native-desktop")]
        public async Task<IActionResult> NativeDesktop(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, NativeDesktopModel>(solution));
        }
    }
}
