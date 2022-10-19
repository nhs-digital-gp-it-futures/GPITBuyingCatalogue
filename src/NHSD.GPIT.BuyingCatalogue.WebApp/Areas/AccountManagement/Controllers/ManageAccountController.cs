using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Models;
//using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers
{
    [Authorize(Policy = "AccountManager")]
    [Area("AccountManagement")]
    [Route("accountmanagement/manageaccount")]
    public sealed class ManageAccountController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IOdsService odsService;
        private readonly ICreateUserService createBuyerService;
        private readonly IUsersService userService;

        public ManageAccountController(
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
        public async Task<IActionResult> Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var users = await userService.GetAllUsersForOrganisation(organisation.Id);
            var relatedOrganisations = await organisationsService.GetRelatedOrganisations(organisation.Id);

            var model = new ManageAccountModel(organisation, users, relatedOrganisations);

            return View(model);
        }

        [HttpGet("{organisationId}/users")]
        public async Task<IActionResult> Users(int organisationId)
        {
            //var organisation = await organisationsService.GetOrganisation(organisationId);
            //var users = await userService.GetAllUsersForOrganisation(organisationId);

            //var model = new UsersModel
            //{
            //    BackLink = Url.Action(nameof(Details), new { organisationId }),
            //    OrganisationId = organisationId,
            //    OrganisationName = organisation.Name,
            //    Users = users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName),
            //};

            return View();
        }

        [HttpGet("{organisationId}/related")]
        public async Task<IActionResult> RelatedOrganisations(int organisationId)
        {
            //var organisation = await organisationsService.GetOrganisation(organisationId);
            //var relatedOrganisations = await organisationsService.GetRelatedOrganisations(organisationId);

            //var model = new RelatedOrganisationsModel
            //{
            //    OrganisationId = organisationId,
            //    OrganisationName = organisation.Name,
            //    RelatedOrganisations = relatedOrganisations,
            //};

            return View();
        }

        [HttpGet("{organisationId}/nominated")]
        public async Task<IActionResult> NominatedOrganisations(int organisationId)
        {
            //var organisation = await organisationsService.GetOrganisation(organisationId);
            //var nominatedOrganisations = await organisationsService.GetNominatedOrganisations(organisationId);

            //var model = new NominatedOrganisationsModel
            //{
            //    OrganisationId = organisationId,
            //    OrganisationName = organisation.Name,
            //    NominatedOrganisations = nominatedOrganisations,
            //};

            return View();
        }
    }
}
