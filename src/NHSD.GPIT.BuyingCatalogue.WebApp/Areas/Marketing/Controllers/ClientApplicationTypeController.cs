using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class ClientApplicationTypeController : Controller
    {
        private readonly ILogger<ClientApplicationTypeController> _logger;
        private readonly ISolutionsService _solutionsService;

        public ClientApplicationTypeController(ILogger<ClientApplicationTypeController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
                      
        [HttpGet("marketing/supplier/solution/{id}/section/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new ClientApplicationTypesModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(ClientApplicationTypesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.ClientApplicationTypes.Clear();

            if (model.BrowserBased) clientApplication.ClientApplicationTypes.Add("browser-based");
            if (model.NativeMobile) clientApplication.ClientApplicationTypes.Add("native-mobile");
            if (model.NativeDesktop) clientApplication.ClientApplicationTypes.Add("native-desktop");

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based")]
        public async Task<IActionResult> BrowserBased(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new BrowserBasedModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile")]
        public async Task<IActionResult> NativeMobile(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new NativeMobileModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-desktop")]
        public async Task<IActionResult> NativeDesktop(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new NativeDesktopModel(solution);

            return View(model);
        }
    }
}
