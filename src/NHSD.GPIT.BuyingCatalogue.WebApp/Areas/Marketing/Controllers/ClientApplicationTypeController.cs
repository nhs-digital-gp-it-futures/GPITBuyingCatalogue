﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class ClientApplicationTypeController : Controller
    {
        private readonly ILogWrapper<ClientApplicationTypeController> _logger;
        private readonly ISolutionsService _solutionsService;

        public ClientApplicationTypeController(ILogWrapper<ClientApplicationTypeController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
                      
        [HttpGet("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new ClientApplicationTypesModel(solution));
        }

        [HttpPost("client-application-types")]
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

        [HttpGet("browser-based")]
        public async Task<IActionResult> BrowserBased(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new BrowserBasedModel(solution));
        }

        [HttpGet("native-mobile")]
        public async Task<IActionResult> NativeMobile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new NativeMobileModel(solution));
        }

        [HttpGet("native-desktop")]
        public async Task<IActionResult> NativeDesktop(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(new NativeDesktopModel(solution));
        }
    }
}
