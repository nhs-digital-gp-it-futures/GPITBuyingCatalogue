using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section/native-mobile")]
    public sealed class NativeMobileController : Controller
    {
        private readonly ILogWrapper<NativeMobileController> logger;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public NativeMobileController(
            ILogWrapper<NativeMobileController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, OperatingSystemsModel>(solution);

            return View(model);
        }

        [HttpPost("operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId, OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MobileOperatingSystems =
                mapper.Map<OperatingSystemsModel, MobileOperatingSystems>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, MobileFirstApproachModel>(solution);

            return View(model);
        }

        [HttpPost("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(CatalogueItemId solutionId, MobileFirstApproachModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeMobileFirstDesign = model.MobileFirstDesign();

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, ConnectivityModel>(solution);

            return View(model);
        }

        [HttpPost("connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId, ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MobileConnectionDetails = mapper.Map<ConnectivityModel, MobileConnectionDetails>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, MemoryAndStorageModel>(solution);

            return View(model);
        }

        [HttpPost("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId, MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MobileMemoryAndStorage =
                mapper.Map<MemoryAndStorageModel, MobileMemoryAndStorage>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("third-party")]
        public async Task<IActionResult> ThirdParty(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, ThirdPartyModel>(solution);

            return View(model);
        }

        [HttpPost("third-party")]
        public async Task<IActionResult> ThirdParty(CatalogueItemId solutionId, ThirdPartyModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.MobileThirdParty = mapper.Map<ThirdPartyModel, MobileThirdParty>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, HardwareRequirementsModel>(solution);

            return View(model);
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeMobileHardwareRequirements = model.Description;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, AdditionalInformationModel>(solution);

            return View(model);
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeMobileAdditionalInformation = model.AdditionalInformation;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        private RedirectToActionResult RedirectBack(CatalogueItemId solutionId)
        {
            return RedirectToAction(
                nameof(ClientApplicationTypeController.NativeMobile),
                typeof(ClientApplicationTypeController).ControllerName(),
                new { solutionId });
        }
    }
}
