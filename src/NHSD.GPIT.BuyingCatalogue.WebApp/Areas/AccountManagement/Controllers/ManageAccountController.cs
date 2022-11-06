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
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers
{
    [Authorize(Policy = "AccountManager")]
    [Area("AccountManagement")]
    [Route("accountmanagement/manageaccount")]
    public sealed class ManageAccountController : OrganisationBaseController
    {
        public ManageAccountController(
            IOrganisationsService organisationsService,
            IOdsService odsService,
            ICreateUserService createBuyerService,
            IUsersService userService)
            : base(organisationsService, odsService, createBuyerService, userService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await OrganisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            return RedirectToAction(
                nameof(ManageAccountController.Details),
                typeof(ManageAccountController).ControllerName(),
                new { organisationId = organisation.Id });
        }

        protected override string GetHomeLink()
        {
            return Url.Action(
                nameof(HomeController.Index),
                typeof(HomeController).ControllerName());
        }
    }
}
