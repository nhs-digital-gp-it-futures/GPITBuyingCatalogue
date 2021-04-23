using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class BrowserBasedController : Controller
    {
        private readonly ILogWrapper<BrowserBasedController> _logger;
        private readonly ISolutionsService _solutionsService;

        public BrowserBasedController(ILogWrapper<BrowserBasedController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/supported-browsers")]
        public async Task<IActionResult> SupportedBrowsers(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new SupportedBrowsersModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/browser-based/supported-browsers")]
        public async Task<IActionResult> SupportedBrowsers(SupportedBrowsersModel model)
        {
            if(model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.BrowsersSupported.Clear();

            foreach (var browser in model.Browsers.Where(x => x.Checked))
                clientApplication.BrowsersSupported.Add(browser.BrowserName);

            if (string.IsNullOrWhiteSpace(model.MobileResponsive))
                clientApplication.MobileResponsive = null;
            else
                clientApplication.MobileResponsive = model.MobileResponsive.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ? true : false;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new MobileFirstApproachModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/browser-based/mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(MobileFirstApproachModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (string.IsNullOrWhiteSpace(model.MobileFirstApproach))
                clientApplication.MobileFirstDesign = null;
            else
                clientApplication.MobileFirstDesign = model.MobileFirstApproach.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ? true : false;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }


        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new PlugInsOrExtensionsModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(PlugInsOrExtensionsModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication.Plugins == null)
                clientApplication.Plugins = new Plugins();

            if (string.IsNullOrWhiteSpace(model.PlugInsRequired))
                clientApplication.Plugins.Required = null;
            else
                clientApplication.Plugins.Required = model.PlugInsRequired.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ? true : false;

            clientApplication.Plugins.AdditionalInformation = model.AdditionalInformation;
            
            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new ConnectivityAndResolutionModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(ConnectivityAndResolutionModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MinimumDesktopResolution = model.SelectedScreenResolution;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new HardwareRequirementsModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/browser-based/hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(HardwareRequirementsModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.HardwareRequirements = model.Description;
            
            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/additional-information")]
        public async Task<IActionResult> AdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new AdditionalInformationModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/browser-based/additional-information")]
        public async Task<IActionResult> AdditionalInformation(AdditionalInformationModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.AdditionalInformation = model.AdditionalInformation;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        private RedirectResult RedirectBack(string solutionId)
        {
            return Redirect($"/marketing/supplier/solution/{solutionId}/section/browser-based");
        }
    }
}
