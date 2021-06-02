using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section/native-desktop")]
    public class NativeDesktopController : Controller
    {
        private readonly ILogWrapper<NativeDesktopController> logger;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public NativeDesktopController(
            ILogWrapper<NativeDesktopController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("operating-systems")]
        public async Task<IActionResult> OperatingSystems(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"operating-systems-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, OperatingSystemsModel>(solution));
        }

        [HttpPost("operating-systems")]
        public async Task<IActionResult> OperatingSystems(OperatingSystemsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.NativeDesktopOperatingSystemsDescription = model.OperatingSystemsDescription;

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("connectivity")]
        public async Task<IActionResult> Connectivity(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"connectivity-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ConnectivityModel>(solution));
        }

        [HttpPost("connectivity")]
        public async Task<IActionResult> Connectivity(ConnectivityModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"memory-and-storage-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, MemoryAndStorageModel>(solution));
        }

        [HttpPost("memory-and-storage")]
        public async Task<IActionResult> MemoryAndStorage(MemoryAndStorageModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.NativeDesktopMemoryAndStorage =
                mapper.Map<MemoryAndStorageModel, NativeDesktopMemoryAndStorage>(model);

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("third-party")]
        public async Task<IActionResult> ThirdParty(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"third-party-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ThirdPartyModel>(solution));
        }

        [HttpPost("third-party")]
        public async Task<IActionResult> ThirdParty(ThirdPartyModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.NativeDesktopThirdParty = mapper.Map<ThirdPartyModel, NativeDesktopThirdParty>(model);

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"hardware-requirments-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, HardwareRequirementsModel>(solution));
        }

        [HttpPost("hardware-requirements")]
        public async Task<IActionResult> HardwareRequirements(HardwareRequirementsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.NativeDesktopHardwareRequirements = model.Description;

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        [HttpGet("additional-information")]
        public async Task<IActionResult> AdditionalInformation(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"additional-information-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, AdditionalInformationModel>(solution));
        }

        [HttpPost("additional-information")]
        public async Task<IActionResult> AdditionalInformation(AdditionalInformationModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);
            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            clientApplication.NativeDesktopAdditionalInformation = model.AdditionalInformation;

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectBack(model.SolutionId);
        }

        private RedirectResult RedirectBack(string solutionId) =>
            Redirect($"/marketing/supplier/solution/{solutionId}/section/native-desktop");
    }
}
