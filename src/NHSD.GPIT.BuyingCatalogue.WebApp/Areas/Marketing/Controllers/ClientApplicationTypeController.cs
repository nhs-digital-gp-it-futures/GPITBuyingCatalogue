using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section")]
    public sealed class ClientApplicationTypeController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public ClientApplicationTypeController(
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, ClientApplicationTypesModel>(solution));
        }

        [HttpPost("client-application-types")]
        public async Task<IActionResult> ClientApplicationTypes(CatalogueItemId solutionId, ClientApplicationTypesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var clientApplication = await solutionsService.GetClientApplication(solutionId);

            if (clientApplication is null)
                return BadRequest($"No Client Application found for Solution Id: {solutionId}");

            mapper.Map(model, clientApplication);

            var checkboxProperties = model.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(CheckboxAttribute)));

            foreach (var prop in checkboxProperties)
            {
                if ((bool)prop.GetValue(model)) clientApplication.ClientApplicationTypes.Add(prop.GetCustomAttribute<CheckboxAttribute>().FieldText);
            }

            await solutionsService.SaveClientApplication(solutionId, clientApplication);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("browser-based")]
        public async Task<IActionResult> BrowserBased(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, BrowserBasedModel>(solution));
        }

        [HttpGet("native-mobile")]
        public async Task<IActionResult> NativeMobile(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, NativeMobileModel>(solution));
        }

        [HttpGet("native-desktop")]
        public async Task<IActionResult> NativeDesktop(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, NativeDesktopModel>(solution));
        }
    }
}
