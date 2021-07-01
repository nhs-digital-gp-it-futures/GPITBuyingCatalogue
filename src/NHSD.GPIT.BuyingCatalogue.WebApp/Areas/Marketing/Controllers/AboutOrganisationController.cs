using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}/section")]
    public sealed class AboutOrganisationController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IMapper mapper;

        public AboutOrganisationController(
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("about-supplier")]
        public async Task<IActionResult> AboutSupplier(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, AboutSupplierModel>(solution));
        }

        [HttpPost("about-supplier")]
        public async Task<IActionResult> AboutSupplier(CatalogueItemId solutionId, AboutSupplierModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveSupplierDescriptionAndLink(model.SupplierId, model.Description, model.Link);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }

        [HttpGet("contact-details")]
        public async Task<IActionResult> ContactDetails(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            return View(mapper.Map<CatalogueItem, ContactDetailsModel>(solution));
        }

        [HttpPost("contact-details")]
        public async Task<IActionResult> ContactDetails(CatalogueItemId solutionId, ContactDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var supplierContactsModel = mapper.Map<ContactDetailsModel, SupplierContactsModel>(model);

            await solutionsService.SaveSupplierContacts(supplierContactsModel);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { solutionId });
        }
    }
}
