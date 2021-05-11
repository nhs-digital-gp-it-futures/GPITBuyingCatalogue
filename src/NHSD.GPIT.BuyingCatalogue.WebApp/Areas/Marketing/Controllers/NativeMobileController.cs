using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section/native-mobile")]
    public class NativeMobileController : Controller
    {
        private readonly ILogWrapper<NativeMobileController> _logger;
        private readonly IMapper _mapper;
        private readonly ISolutionsService _solutionsService;

        public NativeMobileController(ILogWrapper<NativeMobileController> logger, IMapper mapper, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
               
        [HttpGet("operating-systems")]
        public async Task<IActionResult> OperatingSystems(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new OperatingSystemsModel(solution);

            return View(model);
        }

        [HttpPost("operating-systems")]
        public async Task<IActionResult> OperatingSystems(OperatingSystemsModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication.MobileOperatingSystems == null)
                clientApplication.MobileOperatingSystems = new MobileOperatingSystems();

            if (clientApplication.MobileOperatingSystems.OperatingSystems == null)
                clientApplication.MobileOperatingSystems.OperatingSystems = new System.Collections.Generic.HashSet<string>();

            clientApplication.MobileOperatingSystems.OperatingSystems.Clear();

            foreach (var operatingSystem in model.OperatingSystems.Where(x => x.Checked))
                clientApplication.MobileOperatingSystems.OperatingSystems.Add(operatingSystem.OperatingSystemName);

            clientApplication.MobileOperatingSystems.OperatingSystemsDescription = model.Description;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new MobileFirstApproachModel(solution);

            return View(model);
        }

        [HttpPost("mobile-first-approach")]
        public async Task<IActionResult> MobileFirstApproach(MobileFirstApproachModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (string.IsNullOrWhiteSpace(model.MobileFirstApproach))
                clientApplication.NativeMobileFirstDesign = null;
            else
                clientApplication.NativeMobileFirstDesign = model.MobileFirstApproach.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ? true : false;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("connectivity")]
        public async Task<IActionResult> Connectivity(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = _mapper.Map<CatalogueItem, ConnectivityModel>(solution);

            return View(model);
        }

        [HttpPost("connectivity")]
        public async Task<IActionResult> Connectivity(ConnectivityModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);
            
            if (clientApplication.MobileConnectionDetails == null)
                clientApplication.MobileConnectionDetails = new MobileConnectionDetails();

            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MobileConnectionDetails.Description = model.Description;

            if (clientApplication.MobileConnectionDetails.ConnectionType == null)
                clientApplication.MobileConnectionDetails.ConnectionType = new System.Collections.Generic.HashSet<string>();

            clientApplication.MobileConnectionDetails.ConnectionType.Clear();
            
            foreach (var connectionType in model.ConnectionTypes.Where(x => x.Checked))
                clientApplication.MobileConnectionDetails.ConnectionType.Add(connectionType.ConnectionType);

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = _mapper.Map<CatalogueItem, MemoryAndStorageModel>(solution);

            return View(model);
        }

        [HttpPost("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(MemoryAndStorageModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication.MobileMemoryAndStorage == null)
                clientApplication.MobileMemoryAndStorage = new MobileMemoryAndStorage();

            clientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.MobileMemoryAndStorage.Description = model.Description;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("third-party")]
        public async Task<IActionResult> ThirdParty(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = _mapper.Map<CatalogueItem, ThirdPartyModel>(solution);

            return View(model);
        }

        [HttpPost("third-party")]
        public async Task<IActionResult> ThirdParty(ThirdPartyModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication.MobileThirdParty == null)
                clientApplication.MobileThirdParty = new MobileThirdParty();

            clientApplication.MobileThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.MobileThirdParty.DeviceCapabilities = model.DeviceCapabilities;            

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = _mapper.Map<CatalogueItem, HardwareRequirementsModel>(solution);

            return View(model);
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(HardwareRequirementsModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.NativeMobileHardwareRequirements = model.Description;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new AdditionalInformationModel(solution);

            return View(model);
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(AdditionalInformationModel model)
        {
            if (model == null)
                throw new ArgumentException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await _solutionsService.GetClientApplication(model.SolutionId);

            clientApplication.NativeMobileAdditionalInformation = model.AdditionalInformation;

            await _solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        private RedirectResult RedirectBack(string solutionId)
        {
            return Redirect($"/marketing/supplier/solution/{solutionId}/section/native-mobile");
        }
    }
}