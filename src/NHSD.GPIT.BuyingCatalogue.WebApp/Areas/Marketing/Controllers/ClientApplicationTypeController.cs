using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class ClientApplicationTypeController : Controller
    {
        private readonly ILogWrapper<ClientApplicationTypeController> logger;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public ClientApplicationTypeController(
            ILogWrapper<ClientApplicationTypeController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(string id)
        {
            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution));
        }

        [HttpPost("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(ClientApplicationTypesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(model.SolutionId);

            if (clientApplication == null)
                return BadRequest($"No Client Application found for Solution Id: {model.SolutionId}");

            mapper.Map(model, clientApplication);

            var checkboxProperties = model.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(CheckboxAttribute)));

            foreach (var prop in checkboxProperties)
            {
                if ((bool)prop.GetValue(model)) clientApplication.ClientApplicationTypes.Add(prop.GetCustomAttribute<CheckboxAttribute>().FieldText);
            }

            await solutionsService.SaveClientApplication(model.SolutionId, clientApplication);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("browser-based")]
        public async Task<IActionResult> BrowserBased(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, BrowserBasedModel>(solution));
        }

        [HttpGet("native-mobile")]
        public async Task<IActionResult> NativeMobile(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, NativeMobileModel>(solution));
        }

        [HttpGet("native-desktop")]
        public async Task<IActionResult> NativeDesktop(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, NativeDesktopModel>(solution));
        }
    }
}
