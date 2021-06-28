using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/organisations")]
    public sealed class OrganisationsController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IOdsService odsService;
        private readonly ICreateBuyerService createBuyerService;
        private readonly IUsersService userService;

        public OrganisationsController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateBuyerService createBuyerService,
            IUsersService userService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> Index()
        {
            var organisations = await organisationsService.GetAllOrganisations();

            return View(new ListOrganisationsModel(organisations));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var organisation = await organisationsService.GetOrganisation(id);
            var users = await userService.GetAllUsersForOrganisation(id);
            var relatedOrganisations = await organisationsService.GetRelatedOrganisations(id);

            return View(new DetailsModel(organisation, users, relatedOrganisations));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> EditOrganisation(Guid id)
        {
            var organisation = await organisationsService.GetOrganisation(id);

            return View(new EditOrganisationModel(organisation));
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> EditOrganisation(Guid id, EditOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.UpdateCatalogueAgreementSigned(id, model.CatalogueAgreementSigned);

            return RedirectToAction(
                nameof(EditConfirmation),
                typeof(OrganisationsController).ControllerName(),
                new { id });
        }

        [HttpGet("{id}/edit/confirmation")]
        public async Task<IActionResult> EditConfirmation(Guid id)
        {
            var organisation = await organisationsService.GetOrganisation(id);

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
            if (!ModelState.IsValid)
                return View(model);

            // TODO - Check ODS code has not already been added
            // TODO - Check the ODS code is valid
            return RedirectToAction(
                nameof(Select),
                typeof(OrganisationsController).ControllerName(),
                new { ods = model.OdsCode });
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
            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(Create),
                typeof(OrganisationsController).ControllerName(),
                new { ods = model.OdsOrganisation.OdsCode });
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
            if (!ModelState.IsValid)
                return View(model);

            var organisation = await odsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode);
            var orgId = await organisationsService.AddOdsOrganisation(organisation, model.CatalogueAgreementSigned);

            return RedirectToAction(
                nameof(Confirmation),
                typeof(OrganisationsController).ControllerName(),
                new { id = orgId.ToString() });
        }

        [HttpGet("find/select/create/confirmation")]
        public async Task<IActionResult> Confirmation(string id)
        {
            var organisation = await organisationsService.GetOrganisation(new Guid(id));

            return View(new ConfirmationModel(organisation.Name));
        }

        [HttpGet("{id}/adduser")]
        public async Task<IActionResult> AddUser(Guid id)
        {
            var organisation = await organisationsService.GetOrganisation(id);

            return View(new AddUserModel(organisation));
        }

        [HttpPost("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(Guid organisationId, AddUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
            var organisation = await organisationsService.GetOrganisation(organisationId);

            return View(new UserDetailsModel(organisation, user));
        }

        [HttpGet("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(Guid organisationId, string userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            return View(new UserEnablingModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(UserDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);
            return Redirect($"/admin/organisations/{model.Organisation.OrganisationId}/{model.User.Id}/disable");
        }

        [HttpGet("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(Guid organisationId, string userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            return View(new UserEnablingModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(UserDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);
            return Redirect($"/admin/organisations/{model.Organisation.OrganisationId}/{model.User.Id}/enable");
        }

        [HttpGet("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(Guid organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var availableOrganisations = await organisationsService.GetUnrelatedOrganisations(organisationId);

            return View(new AddAnOrganisationModel(organisation, availableOrganisations));
        }

        [HttpPost("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(AddAnOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.AddRelatedOrganisations(model.Organisation.OrganisationId, model.SelectedOrganisation);

            return RedirectToAction(
                nameof(Details),
                typeof(OrganisationsController).ControllerName(),
                new { id = model.Organisation.OrganisationId });
        }

        [HttpGet("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(Guid organisationId, Guid relatedOrganisationId)
        {
            var relatedOrganisation = await organisationsService.GetOrganisation(relatedOrganisationId);

            return View(new RemoveAnOrganisationModel(organisationId, relatedOrganisation));
        }

        [HttpPost("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(Guid organisationId, Guid relatedOrganisationId, RemoveAnOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.RemoveRelatedOrganisations(organisationId, relatedOrganisationId);
            return RedirectToAction(
                nameof(Details),
                typeof(OrganisationsController).ControllerName(),
                new { id = organisationId });
        }
    }
}
