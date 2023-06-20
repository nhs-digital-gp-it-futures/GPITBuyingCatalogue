using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels;

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

        [HttpGet("manage/{solutionId}/application-type/desktop")]
        public async Task<IActionResult> Desktop(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = solution.Solution.EnsureAndGetApplicationType();
            var model = new DesktopBasedModel(solution)
            {
                BackLink = clientApplication?.HasApplicationType(ApplicationType.Desktop) ?? false
                           ? Url.Action(
                               nameof(CatalogueSolutionsController.ApplicationType),
                               typeof(CatalogueSolutionsController).ControllerName(),
                               new { solutionId })
                           : Url.Action(
                               nameof(CatalogueSolutionsController.AddApplicationType),
                               typeof(CatalogueSolutionsController).ControllerName(),
                               new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("manage/{solutionId}/application-type/desktop/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new OperatingSystemsModel(solution)
            {
                BackLink = Url.Action(nameof(Desktop), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/desktop/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId, OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationType(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopOperatingSystemsDescription = model.Description;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/desktop/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ConnectivityModel(solution)
            {
                BackLink = Url.Action(nameof(Desktop), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/desktop/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId, ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationType(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/desktop/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new MemoryAndStorageModel(solution)
            {
                BackLink = Url.Action(nameof(Desktop), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/desktop/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId, MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationType(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopMemoryAndStorage ??= new NativeDesktopMemoryAndStorage();

            clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription = model.StorageSpace;
            clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu = model.ProcessingPower;
            clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution = model.SelectedResolution;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/desktop/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ThirdPartyComponentsModel(solution)
            {
                BackLink = Url.Action(nameof(Desktop), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/desktop/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId, ThirdPartyComponentsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationType(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopThirdParty ??= new NativeDesktopThirdParty();

            clientApplication.NativeDesktopThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.NativeDesktopThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/desktop/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HardwareRequirementsModel(solution)
            {
                BackLink = Url.Action(nameof(Desktop), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/desktop/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationType(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopHardwareRequirements = model.Description;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/desktop/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AdditionalInformationModel(solution)
            {
                BackLink = Url.Action(nameof(Desktop), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/desktop/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetApplicationType(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeDesktopAdditionalInformation = model.AdditionalInformation;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, clientApplication);

            return RedirectToAction(nameof(Desktop), new { solutionId });
        }
    }
}
