using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class DesktopBasedController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public DesktopBasedController(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop")]
        public async Task<IActionResult> Desktop(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new DesktopBasedModel(solution));
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new OperatingSystemsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/desktop/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId, OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopOperatingSystemsDescription = model.Description;

            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.Desktop);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ConnectivityModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/desktop/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId, ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.Desktop);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new MemoryAndStorageModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/desktop/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId, MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopMemoryAndStorage ??= new NativeDesktopMemoryAndStorage();

            clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription = model.StorageSpace;
            clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu = model.ProcessingPower;
            clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution = model.SelectedResolution;

            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.Desktop);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new ThirdPartyComponentsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/desktop/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId, ThirdPartyComponentsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopThirdParty ??= new NativeDesktopThirdParty();

            clientApplication.NativeDesktopThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.NativeDesktopThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.Desktop);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new HardwareRequirementsModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/desktop/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopHardwareRequirements = model.Description;

            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.Desktop);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/desktop/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            return View(new AdditionalInformationModel(solution));
        }

        [HttpPost("manage/{solutionId}/client-application-type/desktop/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopAdditionalInformation = model.AdditionalInformation;

            clientApplication.EnsureClientApplicationTypePresent(ClientApplicationType.Desktop);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }
    }
}
