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
        private readonly ILogWrapper<OrganisationsController> logger;
        private readonly IOrganisationsService organisationService;
        private readonly IOdsService odsService;
        private readonly ICreateBuyerService createBuyerService;
        private readonly IUsersService userService;

        public OrganisationsController(
            ILogWrapper<OrganisationsController> logger,
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateBuyerService createBuyerService,
            IUsersService userService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            organisationService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> Index()
        {
            var organisations = await organisationService.GetAllOrganisations();
            return View(new ListOrganisationsModel(organisations));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var organisation = await organisationService.GetOrganisation(id);
            var users = await userService.GetAllUsersForOrganisation(id);
            var relatedOrganisations = await organisationService.GetRelatedOrganisations(id);
            return View(new DetailsModel(organisation, users, relatedOrganisations));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> EditOrganisation(Guid id)
        {
            var organisation = await organisationService.GetOrganisation(id);
            return View(new EditOrganisationModel(organisation));
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> EditOrganisation(string id, EditOrganisationModel model)
        {
            await organisationService.UpdateCatalogueAgreementSigned(model.Organisation.OrganisationId, model.CatalogueAgreementSigned);
            return RedirectToAction("EditConfirmation", "Organisations", new { id });
        }

        [HttpGet("{id}/edit/confirmation")]
        public async Task<IActionResult> EditConfirmation(Guid id)
        {
            var organisation = await organisationService.GetOrganisation(id);
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
            var organisation = await odsService.GetOrganisationByOdsCode(ods);
            return View(new SelectOrganisationModel(organisation));
        }

        [HttpPost("find/select")]
        public IActionResult Select(SelectOrganisationModel model)
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
            var organisation = await odsService.GetOrganisationByOdsCode(ods);
            return View(new CreateOrganisationModel(organisation));
        }

        [HttpPost("find/select/create")]
        public async Task<IActionResult> Create(CreateOrganisationModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!ModelState.IsValid)
                return View(model);

            var organisation = await odsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode);
            var orgId = await organisationService.AddOdsOrganisation(organisation, model.CatalogueAgreementSigned);
            return RedirectToAction("Confirmation", "Organisations", new { id = orgId.ToString() });
        }

        [HttpGet("find/select/create/confirmation")]
        public async Task<IActionResult> Confirmation(string id)
        {
            var organisation = await organisationService.GetOrganisation(new Guid(id));
            return View(new ConfirmationModel(organisation.Name));
        }

        [HttpGet("{id}/adduser")]
        public async Task<IActionResult> AddUser(Guid id)
        {
            var organisation = await organisationService.GetOrganisation(id);
            return View(new AddUserModel(organisation));
        }

        [HttpPost("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(Guid organisationId, AddUserModel model)
        {
            var result = await createBuyerService.Create(organisationId, model.FirstName, model.LastName, model.TelephoneNumber, model.EmailAddress);

            // TODO - Check result

            // TODO - better way of routing
            return Redirect($"/admin/organisations/{organisationId}/adduser/confirmation?id={result.Value}");
        }

        [HttpGet("{organisationId}/adduser/confirmation")]
        public async Task<IActionResult> AddUserConfirmation(Guid organisationId, string id)
        {
            var user = await userService.GetUser(id);
            return View(new AddUserConfirmationModel(user.GetDisplayName(), organisationId));
        }

        [HttpGet("{organisationId}/{userId}")]
        public async Task<IActionResult> UserDetails(Guid organisationId, string userId)
        {
            var user = await userService.GetUser(userId);
            var organisation = await organisationService.GetOrganisation(organisationId);
            return View(new UserDetailsModel(organisation, user));
        }

        [HttpGet("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(Guid organisationId, string userId)
        {
            var organisation = await organisationService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);
            return View(new UserEnablingModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(UserDetailsModel model)
        {
            await userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);
            return Redirect($"/admin/organisations/{model.Organisation.OrganisationId}/{model.User.Id}/disable");
        }

        [HttpGet("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(Guid organisationId, string userId)
        {
            var organisation = await organisationService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);
            return View(new UserEnablingModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(UserDetailsModel model)
        {
            await userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);
            return Redirect($"/admin/organisations/{model.Organisation.OrganisationId}/{model.User.Id}/enable");
        }

        [HttpGet("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(Guid organisationId)
        {
            var organisation = await organisationService.GetOrganisation(organisationId);
            var availableOrganisations = await organisationService.GetUnrelatedOrganisations(organisationId);
            return View(new AddAnOrganisationModel(organisation, availableOrganisations));
        }

        [HttpPost("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(AddAnOrganisationModel model)
        {
            await organisationService.AddRelatedOrganisations(model.Organisation.OrganisationId, model.SelectedOrganisation);
            return RedirectToAction("Details", "Organisations", new { id = model.Organisation.OrganisationId });
        }

        [HttpGet("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(Guid organisationId, Guid relatedOrganisationId)
        {
            var relatedOrganisation = await organisationService.GetOrganisation(relatedOrganisationId);
            return View(new RemoveAnOrganisationModel(organisationId, relatedOrganisation));
        }

        [HttpPost("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(Guid organisationId, Guid relatedOrganisationId, RemoveAnOrganisationModel model)
        {
            await organisationService.RemoveRelatedOrganisations(organisationId, relatedOrganisationId);
            return RedirectToAction("Details", "Organisations", new { id = organisationId });
        }
    }
}
