using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section/native-desktop")]
    public class NativeDesktopController : Controller
    {
        private readonly ILogWrapper<NativeDesktopController> _logger;
        private readonly ISolutionsService _solutionsService;

        public NativeDesktopController(ILogWrapper<NativeDesktopController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
                                  
        [HttpGet("operating-systems")]
        public async Task<IActionResult> OperatingSystems(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new OperatingSystemsModel(solution));
        }

        [HttpPost("operating-systems")]
        public async Task<IActionResult> OperatingSystems(OperatingSystemsModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.NativeDesktopOperatingSystemsDescription = model.OperatingSystemsDescription;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("connectivity")]
        public async Task<IActionResult> Connectivity(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new ConnectivityModel(solution));
        }

        [HttpPost("connectivity")]
        public async Task<IActionResult> Connectivity(ConnectivityModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new MemoryAndStorageModel(solution));
        }

        [HttpPost("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(MemoryAndStorageModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication.NativeDesktopMemoryAndStorage == null)
                clientApplication.NativeDesktopMemoryAndStorage = new NativeDesktopMemoryAndStorage();

            clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription = model.StorageDescription;
            clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu = model.MinimumCpu;            
            clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution = model.SelectedScreenResolution;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("third-party")]
        public async Task<IActionResult> ThirdParty(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new ThirdPartyModel(solution));
        }

        [HttpPost("third-party")]
        public async Task<IActionResult> ThirdParty(ThirdPartyModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication.NativeDesktopThirdParty == null)
                clientApplication.NativeDesktopThirdParty = new NativeDesktopThirdParty();

            clientApplication.NativeDesktopThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.NativeDesktopThirdParty.DeviceCapabilities = model.DeviceCapabilities;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new HardwareRequirementsModel(solution));
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(HardwareRequirementsModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.NativeDesktopHardwareRequirements = model.Description;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new AdditionalInformationModel(solution));
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(AdditionalInformationModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.NativeDesktopAdditionalInformation = model.AdditionalInformation;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        private RedirectResult RedirectBack(string solutionId)
        {
            return Redirect($"/marketing/supplier/solution/{solutionId}/section/native-desktop");
        }
    }
}
