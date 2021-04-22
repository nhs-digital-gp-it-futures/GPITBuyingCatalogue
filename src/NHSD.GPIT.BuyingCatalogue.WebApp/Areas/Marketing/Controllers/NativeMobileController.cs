using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class NativeMobileController : Controller
    {
        private readonly ILogWrapper<NativeMobileController> _logger;
        private readonly ISolutionsService _solutionsService;

        public NativeMobileController(ILogWrapper<NativeMobileController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
               
        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/operating-systems")]
        public async Task<IActionResult> OperatingSystems(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new OperatingSystemsModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new MobileFirstApproachModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/connectivity")]
        public async Task<IActionResult> Connectivity(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new ConnectivityModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new MemoryAndStorageModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/third-party")]
        public async Task<IActionResult> ThirdParty(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new ThirdPartyModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new HardwareRequirementsModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/additional-information")]
        public async Task<IActionResult> AdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new AdditionalInformationModel(solution);

            return View(model);
        }                      
    }
}