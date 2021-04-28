using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
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
        private readonly ILogWrapper<AboutOrganisationController> _logger;
        private readonly ISolutionsService _solutionsService;
        private readonly IMapper _mapper;

        public AboutOrganisationController(ILogWrapper<AboutOrganisationController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
   
        [HttpGet("about-supplier")]
        public async Task<IActionResult> AboutSupplier(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(_mapper.Map<CatalogueItem, AboutSupplierModel>(solution));
        }

        [HttpPost("about-supplier")]
        public async Task<IActionResult> AboutSupplier(AboutSupplierModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            await _solutionsService.SaveSupplierDescriptionAndLink(model.SupplierId, model.Description, model.Link);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }

        [HttpGet("contact-details")]
        public async Task<IActionResult> ContactDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            
            return View(_mapper.Map<CatalogueItem, ContactDetailsModel>(solution));
        }

        [HttpPost("contact-details")]
        public async Task<IActionResult> ContactDetails(ContactDetailsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var supplierContactsModel = _mapper.Map<ContactDetailsModel, SupplierContactsModel>(model);
            
            await _solutionsService.SaveSupplierContacts(supplierContactsModel);

            return RedirectToAction(nameof(SolutionController.Index), "Solution", new { id = model.SolutionId });
        }
    }
}