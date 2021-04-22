using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class AboutOrganisationController : Controller
    {
        private readonly ILogWrapper<AboutOrganisationController> _logger;
        private readonly ISolutionsService _solutionsService;

        public AboutOrganisationController(ILogWrapper<AboutOrganisationController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
   
        [HttpGet("marketing/supplier/solution/{id}/section/about-supplier")]
        public async Task<IActionResult> AboutSupplier(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

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

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }

        [HttpGet("marketing/supplier/solution/{id}/section/contact-details")]
        public async Task<IActionResult> ContactDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

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

            await _solutionsService.SaveSupplierContacts(model.SolutionId, model.Contact1, model.Contact2);

            return RedirectToAction("Index", "Solution", new { id = model.SolutionId });
        }
    }
}
