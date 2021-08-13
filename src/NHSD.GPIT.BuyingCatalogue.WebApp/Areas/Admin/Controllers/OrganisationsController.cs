using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{organisationId}")]
        public async Task<IActionResult> Details(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var users = await userService.GetAllUsersForOrganisation(organisationId);
            var relatedOrganisations = await organisationsService.GetRelatedOrganisations(organisationId);

            return View(new DetailsModel(organisation, users, relatedOrganisations));
        }

        [HttpGet("{organisationId}/edit")]
        public async Task<IActionResult> EditOrganisation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            return View(new EditOrganisationModel(organisation));
        }

        [HttpPost("{organisationId}/edit")]
        public async Task<IActionResult> EditOrganisation(int organisationId, EditOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.UpdateCatalogueAgreementSigned(organisationId, model.CatalogueAgreementSigned);

            return RedirectToAction(
                nameof(EditConfirmation),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }

        [HttpGet("{organisationId}/edit/confirmation")]
        public async Task<IActionResult> EditConfirmation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            return View(new EditConfirmationModel(organisation.Name, organisationId));
        }

        [HttpGet("find")]
        public IActionResult Find(string ods)
        {
            return View(new FindOrganisationModel(ods));
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find(FindOrganisationModel model)
        {
            var (organisation, error) = await odsService.GetOrganisationByOdsCode(model.OdsCode);

            if (organisation is null)
                ModelState.AddModelError(nameof(model.OdsCode), error);

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(Select),
                typeof(OrganisationsController).ControllerName(),
                new { ods = model.OdsCode });
        }

        [HttpGet("find/select")]
        public async Task<IActionResult> Select(string ods)
        {
            var (organisation, _) = await odsService.GetOrganisationByOdsCode(ods);

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
            (var organisation, _) = await odsService.GetOrganisationByOdsCode(ods);

            return View(new CreateOrganisationModel(organisation));
        }

        [HttpPost("find/select/create")]
        public async Task<IActionResult> Create(CreateOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            (OdsOrganisation organisation, _) = await odsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode);

            var (orgId, error) = await organisationsService.AddOdsOrganisation(organisation, model.CatalogueAgreementSigned);

            if (orgId == 0)
            {
                return RedirectToAction(
                    nameof(Error),
                    typeof(OrganisationsController).ControllerName(),
                    new { model.OdsOrganisation.OdsCode, error });
            }

            return RedirectToAction(
                nameof(Confirmation),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId = orgId });
        }

        [HttpGet("find/select/create/error")]
        public IActionResult Error(string odsCode, string error)
        {
            return View(new AddAnOrganisationErrorModel(odsCode, error));
        }

        [HttpGet("find/select/create/confirmation")]
        public async Task<IActionResult> Confirmation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            return View(new ConfirmationModel(organisation.Name));
        }

        [HttpGet("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            return View(new AddUserModel(organisation));
        }

        [HttpPost("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(int organisationId, AddUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO - Check result
            var result = await createBuyerService.Create(organisationId, model.FirstName, model.LastName, model.TelephoneNumber, model.EmailAddress);

            return RedirectToAction(
                nameof(AddUserConfirmation),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId, userId = result.Value });
        }

        [HttpGet("{organisationId}/adduser/confirmation/{userId}")]
        public async Task<IActionResult> AddUserConfirmation(Guid organisationId, Guid userId)
        {
            var user = await userService.GetUser(userId);

            return View(new AddUserConfirmationModel(user.GetDisplayName(), organisationId));
        }

        [HttpGet("{organisationId}/{userId}")]
        public async Task<IActionResult> UserDetails(int organisationId, Guid userId)
        {
            var user = await userService.GetUser(userId);
            var organisation = await organisationsService.GetOrganisation(organisationId);

            return View(new UserDetailsModel(organisation, user));
        }

        [HttpPost("{organisationId}/{userId}")]
        public async Task<IActionResult> UserDetails(int organisationId, string userId, UserDetailsModel model)
        {
            await userService.EnableOrDisableUser(model.User.Id, !model.User.Disabled);

            if (model.User.Disabled)
            {
                return RedirectToAction(
                    nameof(UserEnabled),
                    typeof(OrganisationsController).ControllerName(),
                    new { organisationId, userId = model.User.Id });
            }

            return RedirectToAction(
                nameof(UserDisabled),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId, userId = model.User.Id });
        }

        [HttpGet("{organisationId}/{userId}/disable")]
        public async Task<IActionResult> UserDisabled(int organisationId, Guid userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            return View(new UserEnablingModel(organisation, user));
        }

        [HttpGet("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(int organisationId, Guid userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            return View(new UserEnablingModel(organisation, user));
        }

        [HttpGet("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var availableOrganisations = await organisationsService.GetUnrelatedOrganisations(organisationId);

            return View(new AddAnOrganisationModel(organisation, availableOrganisations));
        }

        [HttpPost("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(AddAnOrganisationModel model, int organisationId)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.AddRelatedOrganisations(organisationId, model.SelectedOrganisation);

            return RedirectToAction(
                nameof(Details),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }

        [HttpGet("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(int organisationId, int relatedOrganisationId)
        {
            var relatedOrganisation = await organisationsService.GetOrganisation(relatedOrganisationId);

            return View(new RemoveAnOrganisationModel(organisationId, relatedOrganisation));
        }

        [HttpPost("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(int organisationId, int relatedOrganisationId, RemoveAnOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.RemoveRelatedOrganisations(organisationId, relatedOrganisationId);
            return RedirectToAction(
                nameof(Details),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }
    }
}
