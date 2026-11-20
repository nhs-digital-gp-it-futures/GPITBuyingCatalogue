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
    [Route("admin/catalogue-solutions/manage/{solutionType}/{solutionId}/application-type/mobiletablet")]
    public sealed class MobileTabletBasedController(
        ISolutionsService solutionsService) : Controller
    {
        private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));

        [HttpGet]
        public async Task<IActionResult> MobileTablet(SolutionType solutionType, CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var applicationType = solution.Solution.EnsureAndGetApplicationType();
            var model = new MobileTabletBasedModel(solution)
            {
                SolutionType = solutionType,
                BackLink = applicationType?.HasApplicationType(ApplicationType.MobileTablet) ?? false
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
                BackLink = Url.Action(nameof(MobileTablet), new { solutionType, solutionId }),
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

            applicationType.MobileOperatingSystems ??= new MobileOperatingSystems();
            applicationType.MobileOperatingSystems.OperatingSystems ??= [];

            applicationType.MobileOperatingSystems.OperatingSystems.Clear();
            applicationType.MobileOperatingSystems.OperatingSystems = [.. model.OperatingSystems
                .Where(o => o.Checked)
                .Select(o => o.OperatingSystemName)];

            applicationType.MobileOperatingSystems.OperatingSystemsDescription = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionType, solutionId });
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
                BackLink = Url.Action(nameof(MobileTablet), new { solutionType, solutionId }),
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

            applicationType.MobileConnectionDetails ??= new MobileConnectionDetails();

            applicationType.MobileConnectionDetails.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            applicationType.MobileConnectionDetails.Description = model.Description;

            applicationType.MobileConnectionDetails.ConnectionType ??= [];
            applicationType.MobileConnectionDetails.ConnectionType.Clear();

            applicationType.MobileConnectionDetails.ConnectionType = [.. model.ConnectionTypes
                .Where(o => o.Checked)
                .Select(o => o.ConnectionType)];

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionType, solutionId });
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
                BackLink = Url.Action(nameof(MobileTablet), new { solutionType, solutionId }),
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

            applicationType.MobileMemoryAndStorage ??= new MobileMemoryAndStorage();

            applicationType.MobileMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            applicationType.MobileMemoryAndStorage.Description = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionType, solutionId });
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
                BackLink = Url.Action(nameof(MobileTablet), new { solutionType, solutionId }),
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

            applicationType.MobileThirdParty ??= new MobileThirdParty();

            applicationType.MobileThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            applicationType.MobileThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionType, solutionId });
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
                BackLink = Url.Action(nameof(MobileTablet), new { solutionType, solutionId }),
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

            applicationType.NativeMobileHardwareRequirements = model.Description;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionType, solutionId });
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
                BackLink = Url.Action(nameof(MobileTablet), new { solutionType, solutionId }),
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

            applicationType.NativeMobileAdditionalInformation = model.AdditionalInformation;

            applicationType.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveApplicationType(solutionId, applicationType);

            return RedirectToAction(nameof(MobileTablet), new { solutionType, solutionId });
        }
    }
}
