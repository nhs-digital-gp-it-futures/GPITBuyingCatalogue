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
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/organisations")]
    public sealed class OrganisationsController : OrganisationController
    {
        public OrganisationsController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService)
            : base(organisationsService, odsService, createBuyerService, userService)
        {
        }

        // This method is required by the base class
        public override Task<IActionResult> Index()
        {
            return Index(null);
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string search)
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
            var organisation = await GetOrganisationsService().GetOrganisation(organisationId);

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

            await GetOrganisationsService().UpdateCatalogueAgreementSigned(organisationId, model.CatalogueAgreementSigned);

            return RedirectToAction(
                nameof(EditConfirmation),
                typeof(OrganisationsController).ControllerName(),
                new { organisationId });
        }

        [HttpGet("{organisationId}/edit/confirmation")]
        public async Task<IActionResult> EditConfirmation(int organisationId)
        {
            var organisation = await GetOrganisationsService().GetOrganisation(organisationId);

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

            var (organisation, error) = await GetOdsService().GetOrganisationByOdsCode(model.OdsCode);

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
            var (organisation, _) = await GetOdsService().GetOrganisationByOdsCode(ods);

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
            (var organisation, _) = await GetOdsService().GetOrganisationByOdsCode(ods);

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

            (OdsOrganisation organisation, _) = await GetOdsService().GetOrganisationByOdsCode(model.OdsOrganisation.OdsCode);

            var (orgId, error) = await GetOrganisationsService().AddCcgOrganisation(organisation, model.CatalogueAgreementSigned);

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
            var organisation = await GetOrganisationsService().GetOrganisation(organisationId);

            var model = new ConfirmationModel(organisation.Name)
            {
                BackLink = Url.Action(nameof(Index)),
            };

            return View(model);
        }

        private async Task<IEnumerable<Organisation>> GetFilteredOrganisations(string search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? await GetOrganisationsService().GetAllOrganisations()
                : await GetOrganisationsService().GetOrganisationsBySearchTerm(search);
        }
    }
}
