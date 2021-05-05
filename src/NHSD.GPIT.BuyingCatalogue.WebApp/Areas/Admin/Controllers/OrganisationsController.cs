using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
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
        private readonly ICreateBuyerService _createBuyerService;
        private readonly IUsersService _userService;

        public OrganisationsController(ILogWrapper<OrganisationsController> logger,
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateBuyerService createBuyerService,
            IUsersService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organisationService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            _odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            _createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> Index()
        {
            var organisations = await _organisationService.GetAllOrganisations();
            return View(new ListOrganisationsModel(organisations));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var organisation = await _organisationService.GetOrganisation(id);
            var users = await _userService.GetAllUsersForOrganisation(id);
            return View(new DetailsModel(organisation, users));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> EditOrganisation(Guid id)
        {
            var organisation = await _organisationService.GetOrganisation(id);
            return View(new EditOrganisationModel(organisation));
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> EditOrganisation(string id, EditOrganisationModel model)
        {
            await _organisationService.UpdateCatalogueAgreementSigned(model.Organisation.OrganisationId, model.CatalogueAgreementSigned);
            var routeValues = new RouteValueDictionary {{ "id", id }};
            return RedirectToAction("EditConfirmation", "Organisations", routeValues);             
        }

        [HttpGet("{id}/edit/confirmation")]
        public async Task<IActionResult> EditConfirmation(Guid id)
        {
            var organisation = await _organisationService.GetOrganisation(id);
            return View(new EditConfirmationModel(organisation.Name, id));
        }

        [HttpGet("find")]
        public IActionResult Find(string ods)
        {           
            return View(new FindOrganisationModel(ods));
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
            var organisation = await _odsService.GetOrganisationByOdsCode(ods);
            return View(new SelectOrganisationModel(organisation));
        }

        [HttpPost("find/select")]
        public async Task<IActionResult> Select(SelectOrganisationModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction("Create", "Organisations", new { ods = model.OdsOrganisation.OdsCode });

        }

        [HttpGet("find/select/create")]
        public async Task<IActionResult> Create(string ods)
        {
            var organisation = await _odsService.GetOrganisationByOdsCode(ods);
            return View(new CreateOrganisationModel(organisation));
        }

        [HttpPost("find/select/create")]
        public async Task<IActionResult> Create(CreateOrganisationModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var organisation = await _odsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode);
            var orgId = await _organisationService.AddOdsOrganisation(organisation, model.CatalogueAgreementSigned);
            return RedirectToAction("Confirmation", "Organisations", new { id = orgId.ToString() });            
        }

        [HttpGet("find/select/create/confirmation")]
        public async Task<IActionResult> Confirmation(string id)
        {
            var organisation = await _organisationService.GetOrganisation(new Guid(id));
            return View(new ConfirmationModel(organisation.Name));
        }

        [HttpGet("{id}/adduser")]
        public async Task<IActionResult> AddUser(Guid id)
        {
            var organisation = await _organisationService.GetOrganisation(id);
            return View(new AddUserModel(organisation));
        }

        [HttpPost("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(Guid organisationId, AddUserModel model)
        {
            var result = await _createBuyerService.Create(organisationId, model.FirstName, model.LastName, model.TelephoneNumber, model.EmailAddress);

            // TODO - Check result

            // TODO - better way of routing
            return Redirect($"/admin/organisations/{organisationId}/adduser/confirmation?id={result.Value}");
        }

        [HttpGet("{organisationId}/adduser/confirmation")]
        public async Task<IActionResult> AddUserConfirmation(Guid organisationId, string id)
        {            
            var user = await _userService.GetUser(id);
            return View(new AddUserConfirmationModel(user.GetDisplayName(), organisationId));
        }

        [HttpGet("{organisationId}/{userId}")]
        public async Task<IActionResult> UserDetails(Guid organisationId, string userId)
        {
            var user = await _userService.GetUser(userId);
            var organisation = await _organisationService.GetOrganisation(organisationId);
            return View(new UserDetailsModel(organisation, user));
        }

        [HttpGet("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(Guid organisationId, string userId)
        {
            var organisation = await _organisationService.GetOrganisation(organisationId);
            var user = await _userService.GetUser(userId);
            return View(new UserEnablingModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(UserDetailsModel model)
        {
            await _userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);
            return Redirect($"/admin/organisations/{model.Organisation.OrganisationId}/{model.User.Id}/disable");
        }


        [HttpGet("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(Guid organisationId, string userId)
        {
            var organisation = await _organisationService.GetOrganisation(organisationId);
            var user = await _userService.GetUser(userId);
            return View(new UserEnablingModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(UserDetailsModel model)
        {
            await _userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);
            return Redirect($"/admin/organisations/{model.Organisation.OrganisationId}/{model.User.Id}/enable");
        }
    }
}
