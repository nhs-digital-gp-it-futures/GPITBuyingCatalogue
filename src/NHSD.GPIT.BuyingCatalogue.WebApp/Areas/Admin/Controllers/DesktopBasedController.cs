using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionType}/{solutionId}/application-type/desktop")]
    public sealed class DesktopBasedController(
        ISolutionsService solutionsService) : Controller
    {
        private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));

        [HttpGet]
        public async Task<IActionResult> Desktop(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var applicationType = solution.Solution.EnsureAndGetApplicationType();
            var model = new DesktopBasedModel(solution)
            {
                SolutionType = solutionType,
                BackLink = applicationType?.HasApplicationType(ApplicationType.Desktop) ?? false
                           ? Url.Action(
                               nameof(CatalogueSolutionsController.ApplicationType),
                               typeof(CatalogueSolutionsController).ControllerName(),
                               new { solutionType, solutionId })
                           : Url.Action(
                               nameof(CatalogueSolutionsController.AddApplicationType),
                               typeof(CatalogueSolutionsController).ControllerName(),
                               new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpGet("operating-systems")]
        public async Task<IActionResult> OperatingSystems(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new OperatingSystemsModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(Desktop), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("operating-systems")]
        public async Task<IActionResult> OperatingSystems(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeDesktopOperatingSystemsDescription = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(Desktop), new { solutionType, solutionId });
        }

        [HttpGet("connectivity")]
        public async Task<IActionResult> Connectivity(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ConnectivityModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(Desktop), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("connectivity")]
        public async Task<IActionResult> Connectivity(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            applicationType.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(Desktop), new { solutionType, solutionId });
        }

        [HttpGet("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new MemoryAndStorageModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(Desktop), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeDesktopMemoryAndStorage ??= new NativeDesktopMemoryAndStorage();

            applicationType.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            applicationType.NativeDesktopMemoryAndStorage.StorageRequirementsDescription = model.StorageSpace;
            applicationType.NativeDesktopMemoryAndStorage.MinimumCpu = model.ProcessingPower;
            applicationType.NativeDesktopMemoryAndStorage.RecommendedResolution = model.SelectedResolution;

            applicationType.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(Desktop), new { solutionType, solutionId });
        }

        [HttpGet("third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ThirdPartyComponentsModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(Desktop), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            ThirdPartyComponentsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeDesktopThirdParty ??= new NativeDesktopThirdParty();

            applicationType.NativeDesktopThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            applicationType.NativeDesktopThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            applicationType.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(Desktop), new { solutionType, solutionId });
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HardwareRequirementsModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(Desktop), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeDesktopHardwareRequirements = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(Desktop), new { solutionType, solutionId });
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AdditionalInformationModel(solution)
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(Desktop), new { solutionType, solutionId }),
            };

            return View(model);
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(
            SolutionType solutionType,
            CatalogueItemId solutionId,
            AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeDesktopAdditionalInformation = model.AdditionalInformation;

            applicationType.EnsureApplicationTypePresent(ApplicationType.Desktop);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(Desktop), new { solutionType, solutionId });
        }
    }
}
