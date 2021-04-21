using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class NativeDesktopController : Controller
    {
        private readonly ILogger<NativeDesktopController> _logger;
        private readonly ISolutionsService _solutionsService;

        public NativeDesktopController(ILogger<NativeDesktopController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
                                  
        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop/operating-systems")]
        public async Task<IActionResult> NativeDesktopOperatingSystems(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop/connectivity")]
        public async Task<IActionResult> NativeDesktopConnectivity(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop/memory-and-storage")]
        public async Task<IActionResult> NativeDesktopMemoryAndStorage(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop/third-party")]
        public async Task<IActionResult> NativeDesktopThirdParty(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop/hardware-requirements")]
        public async Task<IActionResult> NativeDesktopHardwareRequirements(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop/additional-information")]
        public async Task<IActionResult> NativeDesktopAdditionalInformation(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }
    }
}
