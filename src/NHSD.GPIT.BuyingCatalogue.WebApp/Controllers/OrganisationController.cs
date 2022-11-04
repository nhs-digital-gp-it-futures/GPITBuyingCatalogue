using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using static System.Net.Mime.MediaTypeNames;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public abstract class OrganisationController : Controller
    {
        protected OrganisationController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService)
        {
            OrganisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            OdsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            CreateBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        protected IOrganisationsService OrganisationsService { get; }

        protected IOdsService OdsService { get; }

        protected ICreateUserService CreateBuyerService { get; }

        protected IUsersService UserService { get; }


        [HttpGet("{organisationId}")]
        public async Task<IActionResult> Details(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var users = await UserService.GetAllUsersForOrganisation(organisationId);
            var relatedOrganisations = await OrganisationsService.GetRelatedOrganisations(organisationId);

            var model = new DetailsModel(organisation, users, relatedOrganisations)
            {
                BackLink = Url.Action(nameof(Index)),
                HomeLink = GetHomeLink(),
                ManageOrgsLink = GetManageOrgsLink(),
                ControllerName = GetControllerName(),
            };

            return View("Organisation/Details", model);
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
                Users = users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName),
                HomeLink = GetHomeLink(),
                ManageOrgsLink = GetManageOrgsLink(),
                ControllerName = GetControllerName(),
            };

            return View("Organisation/Users", model);
        }

        [HttpGet("{organisationId}/users/add")]
        public async Task<IActionResult> AddUser(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

            var model = new AddUserModel(organisation)
            {
                BackLink = Url.Action(nameof(Users), new { organisationId }),
                ControllerName = GetControllerName(),
            };

            return View("Organisation/AddUser", model);
        }

        [HttpPost("{organisationId}/users/add")]
        public async Task<IActionResult> AddUser(int organisationId, AddUserModel model)
        {
            if (!ModelState.IsValid)
                return View("Organisation/AddUser", model);

            await CreateBuyerService.Create(
                organisationId,
                model.FirstName,
                model.LastName,
                model.EmailAddress,
                OrganisationFunction.Buyer.Name);

            return RedirectToAction(
                nameof(Users),
                new { organisationId });
        }

        [HttpGet("{organisationId}/users/{userId}/status")]
        public async Task<IActionResult> UserStatus(int organisationId, int userId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var user = await UserService.GetUser(userId);

            var model = new UserStatusModel
            {
                BackLink = Url.Action(nameof(Users), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                UserId = user.Id,
                UserEmail = user.Email,
                IsActive = !user.Disabled,
                ControllerName = GetControllerName(),
            };

            return View("Organisation/UserStatus", model);
        }

        [HttpPost("{organisationId}/users/{userId}/status")]
        public async Task<IActionResult> UserStatus(int organisationId, int userId, UserStatusModel model)
        {
            await UserService.EnableOrDisableUser(userId, model.IsActive);

            return RedirectToAction(nameof(Users), new { organisationId });
        }

        [HttpGet("{organisationId}/related")]
        public async Task<IActionResult> RelatedOrganisations(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var relatedOrganisations = await OrganisationsService.GetRelatedOrganisations(organisationId);

            var model = new RelatedOrganisationsModel
            {
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                RelatedOrganisations = relatedOrganisations,
                HomeLink = GetHomeLink(),
                ManageOrgsLink = GetManageOrgsLink(),
                ControllerName = GetControllerName(),
            };

            return View("Organisation/RelatedOrganisations", model);
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
                ControllerName = GetControllerName(),
            };

            return View("Organisation/RemoveRelatedOrganisation", model);
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
            var nominatedOrganisations = await OrganisationsService.GetNominatedOrganisations(organisationId);
            var model = new NominatedOrganisationsModel
            {
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                NominatedOrganisations = nominatedOrganisations,
                HomeLink = GetHomeLink(),
                ManageOrgsLink = GetManageOrgsLink(),
                ControllerName = GetControllerName(),
            };

            return View("Organisation/NominatedOrganisations", model);
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

            return View("Organisation/AddNominatedOrganisation", model);
        }

        [HttpPost("{organisationId}/nominated/add")]
        public async Task<IActionResult> AddNominatedOrganisation(int organisationId, AddNominatedOrganisationModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PotentialOrganisations = await GetPotentialOrganisations(organisationId);

                return View("Organisation/AddNominatedOrganisation", model);
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
                ControllerName = GetControllerName(),
            };

            return View("Organisation/RemoveNominatedOrganisation", model);
        }

        [HttpPost("{organisationId}/nominated/{nominatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveNominatedOrganisation(int organisationId, int nominatedOrganisationId, RemoveNominatedOrganisationModel model)
        {
            await OrganisationsService.RemoveNominatedOrganisation(organisationId, nominatedOrganisationId);

            return RedirectToAction(nameof(NominatedOrganisations), new { organisationId });
        }

        protected abstract string GetHomeLink();

        protected virtual string GetManageOrgsLink()
        {
            return string.Empty;
        }

        private async Task<IEnumerable<SelectListItem>> GetPotentialOrganisations(int organisationId)
        {
            var allOrganisations = await OrganisationsService.GetAllOrganisations();
            var nominatedOrganisations = await OrganisationsService.GetNominatedOrganisations(organisationId);

            var excludedOrganisationIds = nominatedOrganisations
                .Select(x => x.Id)
                .Union(new[] { organisationId });

            return allOrganisations
                .Where(x => !excludedOrganisationIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, $"{x.Id}"));
        }

        private string GetControllerName()
        {
            return ControllerContext.RouteData.Values["controller"]?.ToString();
        }
    }
}
