using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section/native-desktop")]
    public class NativeDesktopController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public NativeDesktopController(
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, OperatingSystemsModel>(solution));
        }

        [HttpPost("operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId, OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopOperatingSystemsDescription = model.OperatingSystemsDescription;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, ConnectivityModel>(solution));
        }

        [HttpPost("connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId, ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, MemoryAndStorageModel>(solution));
        }

        [HttpPost("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId, MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopMemoryAndStorage =
                mapper.Map<MemoryAndStorageModel, NativeDesktopMemoryAndStorage>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("third-party")]
        public async Task<IActionResult> ThirdParty(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, ThirdPartyModel>(solution));
        }

        [HttpPost("third-party")]
        public async Task<IActionResult> ThirdParty(CatalogueItemId solutionId, ThirdPartyModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopThirdParty = mapper.Map<ThirdPartyModel, NativeDesktopThirdParty>(model);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, HardwareRequirementsModel>(solution));
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopHardwareRequirements = model.Description;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AdditionalInformationModel>(solution));
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopAdditionalInformation = model.AdditionalInformation;

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectBack(solutionId);
        }

        private RedirectToActionResult RedirectBack(CatalogueItemId solutionId)
        {
            return RedirectToAction(
                nameof(ClientApplicationTypeController.NativeDesktop),
                typeof(ClientApplicationTypeController).ControllerName(),
                new { solutionId });
        }
    }
}
