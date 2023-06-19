using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.MobileTabletBasedModels;

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

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet")]
        public async Task<IActionResult> MobileTablet(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var clientApplication = solution.Solution.EnsureAndGetApplicationType();
            var model = new MobileTabletBasedModel(solution)
            {
                BackLink = clientApplication?.HasApplicationType(ApplicationType.MobileTablet) ?? false
                           ? Url.Action(
                               nameof(CatalogueSolutionsController.ClientApplicationType),
                               typeof(CatalogueSolutionsController).ControllerName(),
                               new { solutionId })
                           : Url.Action(
                               nameof(CatalogueSolutionsController.AddApplicationType),
                               typeof(CatalogueSolutionsController).ControllerName(),
                               new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet/operating-systems")]
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

        [HttpPost("manage/{solutionId}/client-application-type/mobiletablet/operating-systems")]
        public async Task<IActionResult> OperatingSystems(CatalogueItemId solutionId, OperatingSystemsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            if (clientApplication.MobileOperatingSystems is null)
                clientApplication.MobileOperatingSystems = new MobileOperatingSystems();

            if (clientApplication.MobileOperatingSystems.OperatingSystems is null)
                clientApplication.MobileOperatingSystems.OperatingSystems = new System.Collections.Generic.HashSet<string>();

            clientApplication.MobileOperatingSystems.OperatingSystems.Clear();

            clientApplication.MobileOperatingSystems.OperatingSystems = model.OperatingSystems
                .Where(o => o.Checked)
                .Select(o => o.OperatingSystemName)
                .ToHashSet();

            clientApplication.MobileOperatingSystems.OperatingSystemsDescription = model.Description;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet/connectivity")]
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

        [HttpPost("manage/{solutionId}/client-application-type/mobiletablet/connectivity")]
        public async Task<IActionResult> Connectivity(CatalogueItemId solutionId, ConnectivityModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            if (clientApplication.MobileConnectionDetails is null)
                clientApplication.MobileConnectionDetails = new MobileConnectionDetails();

            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MobileConnectionDetails.Description = model.Description;

            if (clientApplication.MobileConnectionDetails.ConnectionType is null)
                clientApplication.MobileConnectionDetails.ConnectionType = new System.Collections.Generic.HashSet<string>();

            clientApplication.MobileConnectionDetails.ConnectionType.Clear();

            clientApplication.MobileConnectionDetails.ConnectionType = model.ConnectionTypes
                .Where(o => o.Checked)
                .Select(o => o.ConnectionType)
                .ToHashSet();

            clientApplication.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet/memory-and-storage")]
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

        [HttpPost("manage/{solutionId}/client-application-type/mobiletablet/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(CatalogueItemId solutionId, MemoryAndStorageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.MobileMemoryAndStorage ??= new MobileMemoryAndStorage();

            clientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.MobileMemoryAndStorage.Description = model.Description;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet/third-party-components")]
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

        [HttpPost("manage/{solutionId}/client-application-type/mobiletablet/third-party-components")]
        public async Task<IActionResult> ThirdPartyComponents(CatalogueItemId solutionId, ThirdPartyComponentsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.MobileThirdParty ??= new MobileThirdParty();

            clientApplication.MobileThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.MobileThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet/hardware-requirements")]
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

        [HttpPost("manage/{solutionId}/client-application-type/mobiletablet/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(CatalogueItemId solutionId, HardwareRequirementsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeMobileHardwareRequirements = model.Description;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }

        [HttpGet("manage/{solutionId}/client-application-type/mobiletablet/additional-information")]
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

        [HttpPost("manage/{solutionId}/client-application-type/mobiletablet/additional-information")]
        public async Task<IActionResult> AdditionalInformation(CatalogueItemId solutionId, AdditionalInformationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Application Type found for Solution Id: {solutionId}");

            clientApplication.NativeMobileAdditionalInformation = model.AdditionalInformation;

            clientApplication.EnsureApplicationTypePresent(ApplicationType.MobileTablet);

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(nameof(MobileTablet), new { solutionId });
        }
    }
}
