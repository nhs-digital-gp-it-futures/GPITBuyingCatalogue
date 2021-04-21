using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class NativeMobileController : Controller
    {
        private readonly ILogger<NativeMobileController> _logger;
        private readonly ISolutionsService _solutionsService;

        public NativeMobileController(ILogger<NativeMobileController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
               
        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/operating-systems")]
        public async Task<IActionResult> NativeMobileOperatingSystems(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/mobile-first-approach")]
        public async Task<IActionResult> NativeMobileMobileFirstApproach(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/connectivity")]
        public async Task<IActionResult> NativeMobileConnectivity(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/memory-and-storage")]
        public async Task<IActionResult> NativeMobileMemoryAndStorage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/third-party")]
        public async Task<IActionResult> NativeMobileThirdParty(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/hardware-requirements")]
        public async Task<IActionResult> NativeMobileHardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/additional-information")]
        public async Task<IActionResult> NativeMobileAdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new DeviceModel(solution);

            return View(model);
        }                      
    }
}