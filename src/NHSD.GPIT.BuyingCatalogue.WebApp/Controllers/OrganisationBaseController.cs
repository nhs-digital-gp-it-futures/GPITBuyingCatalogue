using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public abstract class OrganisationBaseController : Controller
    {
        internal const string ViewBaseName = "OrganisationBase";
        private readonly AccountManagementSettings accountManagementSettings;

        protected OrganisationBaseController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService,
            AccountManagementSettings accountManagementSettings)
        {
            OrganisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            OdsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            CreateBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.accountManagementSettings = accountManagementSettings ?? throw new ArgumentNullException(nameof(accountManagementSettings));
        }

        protected IOrganisationsService OrganisationsService { get; }

        protected IOdsService OdsService { get; }

        protected ICreateUserService CreateBuyerService { get; }

        protected IUsersService UserService { get; }

        protected abstract string ControllerName { get; }

        protected abstract string HomeLink { get; }

        [HttpGet("{organisationId}")]
        public async Task<IActionResult> Details(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var users = await UserService.GetAllUsersForOrganisation(organisationId);
            var relatedOrganisations = await OrganisationsService.GetRelatedOrganisations(organisationId);

            var model = new DetailsModel(organisation, users, relatedOrganisations)
            {
                BackLink = Url.Action(nameof(Index)),
                HomeLink = HomeLink,
                ControllerName = ControllerName,
            };

            return View($"{ViewBaseName}/Details", model);
        }

        [HttpGet("{organisationId}/users")]
        public async Task<IActionResult> Users(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var users = await UserService.GetAllUsersForOrganisation(organisationId);

            var model = new UsersModel
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                Users = users,
                HomeLink = HomeLink,
                ControllerName = ControllerName,
            };

            return View($"{ViewBaseName}/Users", model);
        }

        [HttpGet("{organisationId}/users/add")]
        public async Task<IActionResult> AddUser(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var isLimit = await UserService.IsAccountManagerLimit(organisationId);

            var model = new UserDetailsModel(organisation)
            {
                BackLink = Url.Action(nameof(Users), new { organisationId }),
                ControllerName = ControllerName,
                IsDefaultAccountType = isLimit,
                MaxNumberOfAccountManagers = accountManagementSettings.MaximumNumberOfAccountManagers,
            };

            return View($"{ViewBaseName}/UserDetails", model);
        }

        [HttpPost("{organisationId}/users/add")]
        public async Task<IActionResult> AddUser(int organisationId, UserDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View($"{ViewBaseName}/UserDetails", model);

            await CreateBuyerService.Create(
                organisationId,
                model.FirstName,
                model.LastName,
                model.EmailAddress,
                model.SelectedAccountType,
                model.IsActive!.Value);

            return RedirectToAction(
                nameof(Users),
                new { organisationId });
        }

        [HttpGet("{organisationId}/users/{userId}/edit")]
        public async Task<IActionResult> EditUser(int organisationId, int userId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var user = await UserService.GetUser(userId);

            if (user == null || organisation == null || organisation.Id != user.PrimaryOrganisationId)
                return NotFound();

            var isLimit = await UserService.IsAccountManagerLimit(organisationId, userId);

            var model = new UserDetailsModel(organisation, user)
            {
                BackLink = Url.Action(nameof(Users), new { organisationId }),
                ControllerName = ControllerName,
                IsDefaultAccountType = isLimit && !user.Disabled,
                MaxNumberOfAccountManagers = accountManagementSettings.MaximumNumberOfAccountManagers,
            };

            return View($"{ViewBaseName}/UserDetails", model);
        }

        [HttpPost("{organisationId}/users/{userId}/edit")]
        public async Task<IActionResult> EditUser(int organisationId, int userId, UserDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View($"{ViewBaseName}/UserDetails", model);

            var user = await UserService.GetUser(userId);

            if (user.PrimaryOrganisationId != organisationId)
                return BadRequest();

            await UserService.UpdateUser(
                userId,
                model.FirstName,
                model.LastName,
                model.EmailAddress,
                !model.IsActive!.Value,
                model.SelectedAccountType,
                organisationId);

            return RedirectToAction(
                nameof(Users),
                new { organisationId });
        }

        [HttpGet("{organisationId}/related")]
        public async Task<IActionResult> RelatedOrganisations(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

            if (organisation.OrganisationType == OrganisationType.GP)
                return BadRequest();

            var relatedOrganisations = await OrganisationsService.GetRelatedOrganisations(organisationId);

            var model = new RelatedOrganisationsModel
            {
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                RelatedOrganisations = relatedOrganisations,
                HomeLink = HomeLink,
                ControllerName = ControllerName,
            };

            return View($"{ViewBaseName}/RelatedOrganisations", model);
        }

        [HttpGet("{organisationId}/related/{relatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveRelatedOrganisation(int organisationId, int relatedOrganisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var relatedOrganisation = await OrganisationsService.GetOrganisation(relatedOrganisationId);

            var model = new RemoveRelatedOrganisationModel
            {
                BackLink = Url.Action(nameof(RelatedOrganisations), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                RelatedOrganisationId = relatedOrganisationId,
                RelatedOrganisationName = relatedOrganisation.Name,
                ControllerName = ControllerName,
            };

            return View($"{ViewBaseName}/RemoveRelatedOrganisation", model);
        }

        [HttpPost("{organisationId}/related/{relatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveRelatedOrganisation(int organisationId, int relatedOrganisationId, RemoveRelatedOrganisationModel model)
        {
            await OrganisationsService.RemoveRelatedOrganisations(organisationId, relatedOrganisationId);

            return RedirectToAction(nameof(RelatedOrganisations), new { organisationId });
        }

        [HttpGet("{organisationId}/nominated")]
        public async Task<IActionResult> NominatedOrganisations(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

            if (organisation.OrganisationType == OrganisationType.GP)
                return BadRequest();

            var nominatedOrganisations = await OrganisationsService.GetNominatedOrganisations(organisationId);
            var model = new NominatedOrganisationsModel
            {
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                NominatedOrganisations = nominatedOrganisations,
                HomeLink = HomeLink,
                ControllerName = ControllerName,
            };

            return View($"{ViewBaseName}/NominatedOrganisations", model);
        }

        [HttpGet("{organisationId}/nominated/add")]
        public async Task<IActionResult> AddNominatedOrganisation(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

            var model = new AddNominatedOrganisationModel
            {
                BackLink = Url.Action(nameof(NominatedOrganisations), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                PotentialOrganisations = await GetPotentialOrganisations(organisationId),
            };

            return View($"{ViewBaseName}/AddNominatedOrganisation", model);
        }

        [HttpPost("{organisationId}/nominated/add")]
        public async Task<IActionResult> AddNominatedOrganisation(int organisationId, AddNominatedOrganisationModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PotentialOrganisations = await GetPotentialOrganisations(organisationId);

                return View($"{ViewBaseName}/AddNominatedOrganisation", model);
            }

            await OrganisationsService.AddNominatedOrganisation(organisationId, int.Parse(model.SelectedOrganisationId));

            return RedirectToAction(nameof(NominatedOrganisations), new { organisationId });
        }

        [HttpGet("{organisationId}/nominated/{nominatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveNominatedOrganisation(int organisationId, int nominatedOrganisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var nominatedOrganisation = await OrganisationsService.GetOrganisation(nominatedOrganisationId);

            var model = new RemoveNominatedOrganisationModel
            {
                BackLink = Url.Action(nameof(NominatedOrganisations), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                NominatedOrganisationId = nominatedOrganisationId,
                NominatedOrganisationName = nominatedOrganisation.Name,
                ControllerName = ControllerName,
            };

            return View($"{ViewBaseName}/RemoveNominatedOrganisation", model);
        }

        [HttpPost("{organisationId}/nominated/{nominatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveNominatedOrganisation(int organisationId, int nominatedOrganisationId, RemoveNominatedOrganisationModel model)
        {
            await OrganisationsService.RemoveNominatedOrganisation(organisationId, nominatedOrganisationId);

            return RedirectToAction(nameof(NominatedOrganisations), new { organisationId });
        }

        private async Task<IEnumerable<SelectOption<string>>> GetPotentialOrganisations(int organisationId)
        {
            var allOrganisations = await OrganisationsService.GetAllOrganisations();
            var nominatedOrganisations = await OrganisationsService.GetNominatedOrganisations(organisationId);

            var excludedOrganisationIds = nominatedOrganisations
                .Select(x => x.Id)
                .Union(new[] { organisationId });

            return allOrganisations
                .Where(x => !excludedOrganisationIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SelectOption<string>(x.Name, $"{x.Id}"));
        }
    }
}
