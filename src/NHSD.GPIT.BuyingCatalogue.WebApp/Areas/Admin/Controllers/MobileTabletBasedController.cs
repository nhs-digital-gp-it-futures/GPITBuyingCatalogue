using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class MobileTabletBasedController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public MobileTabletBasedController(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("manage/{solutionId}/application-type/mobiletablet")]
        public async Task<IActionResult> MobileTablet(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var applicationType = solution.Solution.EnsureAndGetApplicationType();
            var model = new MobileTabletBasedModel(solution)
            {
                BackLink = applicationType?.HasApplicationType(ApplicationType.MobileTablet) ?? false
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

        [HttpGet("manage/{solutionId}/application-type/mobiletablet/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new OperatingSystemsModel(solution)
            {
                BackLink = Url.Action(nameof(MobileTablet), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/mobiletablet/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId, OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.MobileOperatingSystems ??= new MobileOperatingSystems();
            applicationType.MobileOperatingSystems.OperatingSystems ??= new System.Collections.Generic.HashSet<string>();

            applicationType.MobileOperatingSystems.OperatingSystems.Clear();
            applicationType.MobileOperatingSystems.OperatingSystems = model.OperatingSystems
                .Where(o => o.Checked)
                .Select(o => o.OperatingSystemName)
                .ToHashSet();

            applicationType.MobileOperatingSystems.OperatingSystemsDescription = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/mobiletablet/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ConnectivityModel(solution)
            {
                BackLink = Url.Action(nameof(MobileTablet), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/mobiletablet/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId, ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.MobileConnectionDetails ??= new MobileConnectionDetails();

            applicationType.MobileConnectionDetails.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            applicationType.MobileConnectionDetails.Description = model.Description;

            applicationType.MobileConnectionDetails.ConnectionType ??= new System.Collections.Generic.HashSet<string>();
            applicationType.MobileConnectionDetails.ConnectionType.Clear();

            applicationType.MobileConnectionDetails.ConnectionType = model.ConnectionTypes
                .Where(o => o.Checked)
                .Select(o => o.ConnectionType)
                .ToHashSet();

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/mobiletablet/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new MemoryAndStorageModel(solution)
            {
                BackLink = Url.Action(nameof(MobileTablet), new { solutionId }),
            };
            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/mobiletablet/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId, MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.MobileMemoryAndStorage ??= new MobileMemoryAndStorage();

            applicationType.MobileMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            applicationType.MobileMemoryAndStorage.Description = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/mobiletablet/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new ThirdPartyComponentsModel(solution)
            {
                BackLink = Url.Action(nameof(MobileTablet), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/mobiletablet/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId, ThirdPartyComponentsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.MobileThirdParty ??= new MobileThirdParty();

            applicationType.MobileThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            applicationType.MobileThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/mobiletablet/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new HardwareRequirementsModel(solution)
            {
                BackLink = Url.Action(nameof(MobileTablet), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/mobiletablet/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeMobileHardwareRequirements = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/application-type/mobiletablet/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AdditionalInformationModel(solution)
            {
                BackLink = Url.Action(nameof(MobileTablet), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/application-type/mobiletablet/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var applicationType = await solutionsService.GetApplicationType(solutionId);

            if (applicationType is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            applicationType.NativeMobileAdditionalInformation = model.AdditionalInformation;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }
    }
}
