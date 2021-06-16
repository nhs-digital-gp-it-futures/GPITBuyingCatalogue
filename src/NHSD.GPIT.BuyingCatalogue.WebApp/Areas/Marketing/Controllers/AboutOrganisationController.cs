using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}/section")]
    public class AboutOrganisationController : Controller
    {
        private readonly ILogWrapper<AboutOrganisationController> logger;
        private readonly ISolutionsService solutionsService;
        private readonly IMapper mapper;

        public AboutOrganisationController(
            ILogWrapper<AboutOrganisationController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("about-supplier")]
        public async Task<IActionResult> AboutSupplier(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"about-supplier-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, AboutSupplierModel>(solution));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("about-supplier")]
        public async Task<IActionResult> AboutSupplier(AboutSupplierModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.SaveSupplierDescriptionAndLink(model.SupplierId, model.Description, model.Link);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }

        [HttpGet("contact-details")]
        public async Task<IActionResult> ContactDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"contact-details-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            return View(mapper.Map<CatalogueItem, ContactDetailsModel>(solution));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("contact-details")]
        public async Task<IActionResult> ContactDetails(ContactDetailsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var supplierContactsModel = mapper.Map<ContactDetailsModel, SupplierContactsModel>(model);

            await solutionsService.SaveSupplierContacts(supplierContactsModel);

            return RedirectToAction(
                nameof(SolutionController.Index),
                typeof(SolutionController).ControllerName(),
                new { id = model.SolutionId });
        }
    }
}
