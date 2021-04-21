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
    public class SolutionController : Controller
    {
        private readonly ILogger<SolutionController> _logger;
        private readonly ISolutionsService _solutionsService;

        public SolutionController(ILogger<SolutionController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        [Route("marketing/supplier/solution/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/preview")]
        public IActionResult Preview(string id)
        {
            return RedirectToAction("preview", "solutions", new { id = id });
        }

        #region About Catalogue Solution DONE

        [HttpGet("marketing/supplier/solution/{id}/section/solution-description")]
        public async Task<IActionResult> SolutionDescription(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDescriptionModel(solution);
            
            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/solution-description")]
        public async Task<IActionResult> SolutionDescription(SolutionDescriptionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveSolutionDescription(model.SolutionId, model.Summary, model.Description, model.Link);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/features")]
        public async Task<IActionResult> Features(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/features")]
        public async Task<IActionResult> Features(FeaturesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var features = new List<string>();

            if (!string.IsNullOrEmpty(model.Listing1)) features.Add(model.Listing1);
            if (!string.IsNullOrEmpty(model.Listing2)) features.Add(model.Listing2);
            if (!string.IsNullOrEmpty(model.Listing3)) features.Add(model.Listing3);
            if (!string.IsNullOrEmpty(model.Listing4)) features.Add(model.Listing4);
            if (!string.IsNullOrEmpty(model.Listing5)) features.Add(model.Listing5);
            if (!string.IsNullOrEmpty(model.Listing6)) features.Add(model.Listing6);
            if (!string.IsNullOrEmpty(model.Listing7)) features.Add(model.Listing7);
            if (!string.IsNullOrEmpty(model.Listing8)) features.Add(model.Listing8);
            if (!string.IsNullOrEmpty(model.Listing9)) features.Add(model.Listing9);
            if (!string.IsNullOrEmpty(model.Listing10)) features.Add(model.Listing10);

            var jsonFeatures = JsonConvert.SerializeObject(features);

            await _solutionsService.SaveSolutionFeatures(model.SolutionId, jsonFeatures);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/integrations")]
        public async Task<IActionResult> Integrations(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new IntegrationsModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/integrations")]
        public async Task<IActionResult> Integrations(IntegrationsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveIntegrationLink(model.SolutionId, model.Link);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new ImplementationTimescalesModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/implementation-timescales")]
        public async Task<IActionResult> ImplementationTimescales(ImplementationTimescalesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveImplementationDetail(model.SolutionId, model.Description);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/roadmap")]
        public async Task<IActionResult> Roadmap(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new RoadmapModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/roadmap")]
        public async Task<IActionResult> Roadmap(RoadmapModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveRoadmap(model.SolutionId, model.Summary);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        #endregion

        #region Client Application Type CONTROLLER AND PAGES DONE - MODELS NEED WORK

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
            
            return RedirectToAction("Index", new { id = model.SolutionId });
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

        #endregion

        #region Browser Based Sections

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/supported-browsers")]
        public async Task<IActionResult> BrowserBasedSupportedBrowsers(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/mobile-first-approach")]
        public async Task<IActionResult> BrowserBasedMobileFirstApproach(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/plug-ins-or-extensions")]
        public async Task<IActionResult> BrowserBasedPlugInsOrExtensions(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/connectivity-and-resolution")]
        public async Task<IActionResult> BrowserBasedConnectivityAndResolution(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/hardware-requirements")]
        public async Task<IActionResult> BrowserBasedHardwareRequirements(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/browser-based/additional-information")]
        public async Task<IActionResult> BrowserBasedAdditionalInformation(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        #endregion

        #region Native Mobile or Tablet Sections

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/operating-systems")]
        public async Task<IActionResult> NativeMobileOperatingSystems(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/mobile-first-approach")]
        public async Task<IActionResult> NativeMobileMobileFirstApproach(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/connectivity")]
        public async Task<IActionResult> NativeMobileConnectivity(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/memory-and-storage")]
        public async Task<IActionResult> NativeMobileMemoryAndStorage(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/third-party")]
        public async Task<IActionResult> NativeMobileThirdParty(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/hardware-requirements")]
        public async Task<IActionResult> NativeMobileHardwareRequirements(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/section/native-mobile/additional-information")]
        public async Task<IActionResult> NativeMobileAdditionalInformation(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new FeaturesModel(solution);

            return View(model);
        }


        #endregion

        #region Native Desktop

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

        #endregion

        #region Hosting Type DONE

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-public-cloud")]
        public async Task<IActionResult> HostingTypePublicCloud(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new HostingTypePublicCloudModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-public-cloud")]
        public async Task<IActionResult> HostingTypePublicCloud(HostingTypePublicCloudModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.PublicCloud = model.PublicCloud;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-private-cloud")]
        public async Task<IActionResult> HostingTypePrivateCloud(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new HostingTypePrivateCloudModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-private-cloud")]
        public async Task<IActionResult> HostingTypePrivateCloud(HostingTypePrivateCloudModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.PrivateCloud = model.PrivateCloud;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-hybrid")]
        public async Task<IActionResult> HostingTypeHybrid(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new HostingTypeHybridModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-hybrid")]
        public async Task<IActionResult> HostingTypeHybrid(HostingTypeHybridModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.HybridHostingType = model.HybridHostingType;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }


        [HttpGet("marketing/supplier/solution/{id}/section/hosting-type-on-premise")]
        public async Task<IActionResult> HostingTypeOnPremise(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new HostingTypeOnPremiseModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/hosting-type-on-premise")]
        public async Task<IActionResult> HostingTypeOnPremise(HostingTypeOnPremiseModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var hosting = await _solutionsService.GetHosting(model.SolutionId);

            hosting.OnPremise = model.OnPremise;

            await _solutionsService.SaveHosting(model.SolutionId, hosting);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }


        #endregion

        #region About Organisation

        [HttpGet("marketing/supplier/solution/{id}/section/about-supplier")]
        public async Task<IActionResult> AboutSupplier(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new AboutSupplierModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/about-supplier")]
        public async Task<IActionResult> AboutSupplier(AboutSupplierModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveSupplierDescriptionAndLink(model.SupplierId, model.Description, model.Link);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/contact-details")]
        public async Task<IActionResult> ContactDetails(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new ContactDetailsModel(solution);

            return View(model);
        }

        [HttpPost("marketing/supplier/solution/{id}/section/contact-details")]
        public async Task<IActionResult> ContactDetails(ContactDetailsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            //var solution = await _solutionsService.GetSolution(id);

            //            var model = new ContactDetailsModel(solution);

            return RedirectToAction("Index", new { id = model.SolutionId });
        }

        #endregion
    }
}
