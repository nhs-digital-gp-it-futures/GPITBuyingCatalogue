using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/organisations")]
    public sealed class OrganisationsController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IOdsService odsService;
        private readonly ICreateUserService createBuyerService;
        private readonly IUsersService userService;

        public OrganisationsController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.createBuyerService = createBuyerService ?? throw new ArgumentNullException(nameof(createBuyerService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string search = null)
        {
            var organisations = await GetFilteredOrganisations(search);

            var organisationModel = organisations
                .Select(o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.ExternalIdentifier,
                })
                .ToList();

            return View(new IndexModel(search, organisationModel));
        }

        [HttpGet("search-results")]
        public async Task<IActionResult> SearchResults([FromQuery] string search)
        {
            var results = (await GetFilteredOrganisations(search)).Take(15);

            return Json(results.Select(x => new SuggestionSearchResult
            {
                Title = x.Name,
                Category = x.ExternalIdentifier,
                Url = Url.Action(nameof(Details), new { organisationId = $"{x.Id}" }),
            }));
        }

        [HttpGet("{organisationId}")]
        public async Task<IActionResult> Details(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var users = await userService.GetAllUsersForOrganisation(organisationId);
            var relatedOrganisations = await organisationsService.GetRelatedOrganisations(organisationId);

            var model = new DetailsModel(organisation, users, relatedOrganisations)
            {
                BackLink = Url.Action(nameof(Index)),
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
                BackLink = Url.Action(nameof(Index)),
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

            var (orgId, error) = await organisationsService.AddCcgOrganisation(organisation, model.CatalogueAgreementSigned);

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
                BackLink = Url.Action(nameof(Index)),
            };

            return View(model);
        }

        [HttpGet("{organisationId}/users")]
        public async Task<IActionResult> Users(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var users = await userService.GetAllUsersForOrganisation(organisationId);

            var model = new UsersModel
            {
                BackLink = Url.Action(nameof(Details), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                Users = users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName),
            };

            return View(model);
        }

        [HttpGet("{organisationId}/users/add")]
        public async Task<IActionResult> AddUser(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            var model = new AddUserModel(organisation)
            {
                BackLink = Url.Action(nameof(Users), new { organisationId }),
            };

            return View(model);
        }

        [HttpPost("{organisationId}/users/add")]
        public async Task<IActionResult> AddUser(int organisationId, AddUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await createBuyerService.Create(
                organisationId,
                model.FirstName,
                model.LastName,
                model.EmailAddress,
                OrganisationFunction.Buyer.Name);

            return RedirectToAction(
                nameof(Users),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }

        [HttpGet("{organisationId}/users/{userId}/status")]
        public async Task<IActionResult> UserStatus(int organisationId, int userId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var user = await userService.GetUser(userId);

            var model = new UserStatusModel
            {
                BackLink = Url.Action(nameof(Users), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                UserId = user.Id,
                UserEmail = user.Email,
                IsActive = !user.Disabled,
            };

            return View(model);
        }

        [HttpPost("{organisationId}/users/{userId}/status")]
        public async Task<IActionResult> UserStatus(int organisationId, int userId, UserStatusModel model)
        {
            await userService.EnableOrDisableUser(userId, model.IsActive);

            return RedirectToAction(nameof(Users), new { organisationId });
        }

        [HttpGet("{organisationId}/related")]
        public async Task<IActionResult> RelatedOrganisations(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var relatedOrganisations = await organisationsService.GetRelatedOrganisations(organisationId);

            var model = new RelatedOrganisationsModel
            {
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                RelatedOrganisations = relatedOrganisations,
            };

            return View(model);
        }

        [HttpGet("{organisationId}/related/{relatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveRelatedOrganisation(int organisationId, int relatedOrganisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var relatedOrganisation = await organisationsService.GetOrganisation(relatedOrganisationId);

            var model = new RemoveRelatedOrganisationModel
            {
                BackLink = Url.Action(nameof(RelatedOrganisations), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                RelatedOrganisationId = relatedOrganisationId,
                RelatedOrganisationName = relatedOrganisation.Name,
            };

            return View(model);
        }

        [HttpPost("{organisationId}/related/{relatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveRelatedOrganisation(int organisationId, int relatedOrganisationId, RemoveRelatedOrganisationModel model)
        {
            await organisationsService.RemoveRelatedOrganisations(organisationId, relatedOrganisationId);

            return RedirectToAction(nameof(RelatedOrganisations), new { organisationId });
        }

        [HttpGet("{organisationId}/nominated")]
        public async Task<IActionResult> NominatedOrganisations(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var nominatedOrganisations = await organisationsService.GetNominatedOrganisations(organisationId);

            var model = new NominatedOrganisationsModel
            {
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                NominatedOrganisations = nominatedOrganisations,
            };

            return View(model);
        }

        [HttpGet("{organisationId}/nominated/add")]
        public async Task<IActionResult> AddNominatedOrganisation(int organisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);

            var model = new AddNominatedOrganisationModel
            {
                BackLink = Url.Action(nameof(NominatedOrganisations), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                PotentialOrganisations = await GetPotentialOrganisations(organisationId),
            };

            return View(model);
        }

        [HttpPost("{organisationId}/nominated/add")]
        public async Task<IActionResult> AddNominatedOrganisation(int organisationId, AddNominatedOrganisationModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PotentialOrganisations = await GetPotentialOrganisations(organisationId);

                return View(model);
            }

            await organisationsService.AddNominatedOrganisation(organisationId, int.Parse(model.SelectedOrganisationId));

            return RedirectToAction(nameof(NominatedOrganisations), new { organisationId });
        }

        [HttpGet("{organisationId}/nominated/{nominatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveNominatedOrganisation(int organisationId, int nominatedOrganisationId)
        {
            var organisation = await organisationsService.GetOrganisation(organisationId);
            var nominatedOrganisation = await organisationsService.GetOrganisation(nominatedOrganisationId);

            var model = new RemoveNominatedOrganisationModel
            {
                BackLink = Url.Action(nameof(NominatedOrganisations), new { organisationId }),
                OrganisationId = organisationId,
                OrganisationName = organisation.Name,
                NominatedOrganisationId = nominatedOrganisationId,
                NominatedOrganisationName = nominatedOrganisation.Name,
            };

            return View(model);
        }

        [HttpPost("{organisationId}/nominated/{nominatedOrganisationId}/remove")]
        public async Task<IActionResult> RemoveNominatedOrganisation(int organisationId, int nominatedOrganisationId, RemoveNominatedOrganisationModel model)
        {
            await organisationsService.RemoveNominatedOrganisation(organisationId, nominatedOrganisationId);

            return RedirectToAction(nameof(NominatedOrganisations), new { organisationId });
        }

        private async Task<IEnumerable<Organisation>> GetFilteredOrganisations(string search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? await organisationsService.GetAllOrganisations()
                : await organisationsService.GetOrganisationsBySearchTerm(search);
        }

        private async Task<IEnumerable<SelectListItem>> GetPotentialOrganisations(int organisationId)
        {
            var allOrganisations = await organisationsService.GetAllOrganisations();
            var nominatedOrganisations = await organisationsService.GetNominatedOrganisations(organisationId);

            var excludedOrganisationIds = nominatedOrganisations
                .Select(x => x.Id)
                .Union(new[] { organisationId });

            return allOrganisations
                .Where(x => !excludedOrganisationIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, $"{x.Id}"));
        }
    }
}
