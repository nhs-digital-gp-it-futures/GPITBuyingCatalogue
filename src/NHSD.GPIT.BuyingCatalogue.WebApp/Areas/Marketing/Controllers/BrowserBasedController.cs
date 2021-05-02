﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section/browser-based")]
    public class BrowserBasedController : Controller
    {
        private readonly ILogWrapper<BrowserBasedController> _logger;
        private readonly IMapper _mapper;
        private readonly ISolutionsService _solutionsService;

        public BrowserBasedController(ILogWrapper<BrowserBasedController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("supported-browsers")]
        public async Task<IActionResult> SupportedBrowsers(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, SupportedBrowsersModel>(solution));
        }

        [HttpPost("supported-browsers")]
        public async Task<IActionResult> SupportedBrowsers(SupportedBrowsersModel model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            _mapper.Map(model, clientApplication);
            
            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, MobileFirstApproachModel>(solution));
        }

        [HttpPost("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(MobileFirstApproachModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");
            
            clientApplication.MobileFirstDesign = _mapper.Map<string, bool?>(model.MobileFirstApproach);

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, PlugInsOrExtensionsModel>(solution));
        }

        [HttpPost("plug-ins-or-extensions")]
        public async Task<IActionResult> PlugInsOrExtensions(PlugInsOrExtensionsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.Plugins = _mapper.Map<PlugInsOrExtensionsModel, Plugins>(model);
            
            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }
        
        [HttpGet("connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(solution));
        }

        [HttpPost("connectivity-and-resolution")]
        public async Task<IActionResult> ConnectivityAndResolution(ConnectivityAndResolutionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            _mapper.Map(model, clientApplication);

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, HardwareRequirementsModel>(solution));
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(HardwareRequirementsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.HardwareRequirements = model.Description;
            
            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }
        
        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");
            
            return View(_mapper.Map<CatalogueItem, AdditionalInformationModel>(solution));
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(AdditionalInformationModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");
            
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
