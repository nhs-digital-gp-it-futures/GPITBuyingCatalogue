using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

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

            var model = new DetailsModel(organisation, users, relatedOrganisations)
            {
                BackLink = Url.Action(nameof(HomeController.BuyerOrganisations), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }

        [HttpGet("{organisationId}/edit")]
        public async Task<IActionResult> EditOrganisation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            var model = new EditOrganisationModel(organisation)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
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

            var model = new EditConfirmationModel(organisation.Name, organisationId)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
        }

        [HttpGet("find")]
        public IActionResult Find(string ods)
        {
            var model = new FindOrganisationModel(ods)
            {
                BackLink = Url.Action(nameof(HomeController.BuyerOrganisations), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find(FindOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

            var model = new SelectOrganisationModel(organisation)
            {
                BackLink = Url.Action(nameof(Find)),
            };

            return View(model);
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

            var model = new CreateOrganisationModel(organisation)
            {
                BackLink = Url.Action(nameof(Select), new { ods }),
            };

            return View(model);
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
            var model = new AddAnOrganisationErrorModel(odsCode, error)
            {
                BackLink = Url.Action(nameof(Find)),
            };

            return View(model);
        }

        [HttpGet("find/select/create/confirmation")]
        public async Task<IActionResult> Confirmation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            var model = new ConfirmationModel(organisation.Name)
            {
                BackLink = Url.Action(nameof(HomeController.BuyerOrganisations), typeof(HomeController).ControllerName()),
            };

            return View(model);
        }

        [HttpGet("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            var model = new AddUserModel(organisation)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
        }

        [HttpPost("{organisationId}/adduser")]
        public async Task<IActionResult> AddUser(int organisationId, AddUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await createBuyerService.Create(organisationId, model.FirstName, model.LastName, model.TelephoneNumber, model.EmailAddress);

            return RedirectToAction(
                nameof(AddUserConfirmation),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId, userId = user.Id });
        }

        [HttpGet("{organisationId}/adduser/confirmation/{userId}")]
        public async Task<IActionResult> AddUserConfirmation(int organisationId, int userId)
        {
            var user = await userService.GetUser(userId);

            var model = new AddUserConfirmationModel(user.GetDisplayName(), organisationId)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
        }

        [HttpGet("{organisationId}/{userId}")]
        public async Task<IActionResult> UserDetails(int organisationId, int userId)
        {
            var user = await userService.GetUser(userId);
            var organisation = await organisationsService.GetOrganisation(organisationId);

            var model = new UserDetailsModel(organisation, user)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
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
        public async Task<IActionResult> UserDisabled(int organisationId, int userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            var model = new UserEnablingModel(organisation, user)
            {
                BackLink = Url.Action(nameof(UserDetails), new { organisationId, userId }),
            };

            return View(model);
        }

        [HttpGet("{organisationId}/{userId}/enable")]
        public async Task<IActionResult> UserEnabled(int organisationId, int userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            var model = new UserEnablingModel(organisation, user)
            {
                BackLink = Url.Action(nameof(UserDetails), new { organisationId, userId }),
            };

            return View(model);
        }

        [HttpGet("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            if (organisation is null)
                return BadRequest($"No Organisation found for Id: {organisationId}");

            var availableOrganisations = await organisationsService.GetUnrelatedOrganisations(organisationId);

            var model = new AddAnOrganisationModel(organisation, availableOrganisations)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
        }

        [HttpPost("proxy/{organisationId}")]
        public async Task<IActionResult> AddAnOrganisation(AddAnOrganisationModel model, int organisationId)
        {
            if (!ModelState.IsValid)
                return View(model);

            await organisationsService.AddRelatedOrganisations(organisationId, model.SelectedOrganisation!.Value);

            return RedirectToAction(
                nameof(Details),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }

        [HttpGet("removeproxy/{organisationId}/{relatedOrganisationId}")]
        public async Task<IActionResult> RemoveAnOrganisation(int organisationId, int relatedOrganisationId)
        {
            var currentOrganisation = await organisationsService.GetOrganisation(organisationId);

            if (currentOrganisation is null)
                return BadRequest($"No Organisation found for Id: {organisationId}");

            var relatedOrganisation = await organisationsService.GetOrganisation(relatedOrganisationId);

            if (relatedOrganisation is null)
                return BadRequest($"No Organisation found for Id: {relatedOrganisationId}");

            var model = new RemoveAnOrganisationModel(currentOrganisation, relatedOrganisation)
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
            };

            return View(model);
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
