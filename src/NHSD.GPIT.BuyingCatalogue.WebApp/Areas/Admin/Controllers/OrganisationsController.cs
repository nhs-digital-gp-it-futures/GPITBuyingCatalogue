using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/organisations")]
    public sealed class OrganisationsController : OrganisationBaseController
    {
        public OrganisationsController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService,
            AccountManagementSettings accountManagementSettings)
            : base(organisationsService, odsService, createBuyerService, userService, accountManagementSettings)
        {
        }

        protected override string ControllerName => typeof(OrganisationsController).ControllerName();

        protected override string HomeLink => Url.Action(
            nameof(HomeController.Index),
            typeof(HomeController).ControllerName());

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

        [HttpGet("{organisationId}/edit")]
        public async Task<IActionResult> EditOrganisation(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

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

            await OrganisationsService.UpdateCatalogueAgreementSigned(organisationId, model.CatalogueAgreementSigned);

            return RedirectToAction(
                nameof(EditConfirmation),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }

        [HttpGet("{organisationId}/edit/confirmation")]
        public async Task<IActionResult> EditConfirmation(int organisationId)
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

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

            var (organisation, error) = await OdsService.GetOrganisationByOdsCode(model.OdsCode);

            if (organisation is null)
            {
                ModelState.AddModelError(nameof(model.OdsCode), error);
            }
            else if (await OrganisationsService.OrganisationExists(organisation))
            {
                ModelState.AddModelError(nameof(model.OdsCode), $"The organisation with ODS code {organisation.OdsCode} already exists.");
            }

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
            var (organisation, _) = await OdsService.GetOrganisationByOdsCode(ods);

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
            (var organisation, _) = await OdsService.GetOrganisationByOdsCode(ods);

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

            (OdsOrganisation organisation, _) = await OdsService.GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode);

            var (orgId, error) = await OrganisationsService.AddOrganisation(organisation, model.CatalogueAgreementSigned);

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
            var organisation = await OrganisationsService.GetOrganisation(organisationId);

            var model = new ConfirmationModel(organisation.Name)
            {
                BackLink = Url.Action(nameof(Index)),
            };

            return View(model);
        }

        private async Task<IEnumerable<Organisation>> GetFilteredOrganisations(string search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? await OrganisationsService.GetAllOrganisations()
                : await OrganisationsService.GetOrganisationsBySearchTerm(search);
        }
    }
}
