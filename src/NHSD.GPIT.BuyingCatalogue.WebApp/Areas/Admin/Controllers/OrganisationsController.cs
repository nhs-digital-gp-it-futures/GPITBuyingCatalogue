using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/organisations")]
    public class OrganisationsController : Controller
    {
        private readonly ILogWrapper<OrganisationsController> _logger;
        private readonly IOrganisationsService _organisationService;
        private readonly IOdsService _odsService;
        
        public OrganisationsController(ILogWrapper<OrganisationsController> logger,
            IOrganisationsService organisationsService,
            IOdsService odsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organisationService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            _odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
        }

        public async Task<IActionResult> Index()
        {
            var organisations = await _organisationService.GetAllOrganisations();

            return View(new ListOrganisationsModel(organisations));
        }

        [HttpGet("find")]
        public IActionResult Find()
        {           
            return View(new FindOrganisationModel());
        }

        [HttpPost("find")]
        public IActionResult Find(FindOrganisationModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            // TODO - Check ODS code has not already been added

            // TODO - Check the ODS code is valid

            return RedirectToAction("Select", "Organisations", new { ods = model.OdsCode });
        }

        [HttpGet("find/select")]
        public async Task<IActionResult> Select(string ods)
        {
            var organisation = await _odsService.GetOrganisationByOdsCodeAsync(ods);

            return View(new SelectOrganisationModel(organisation));
        }
    }
}
