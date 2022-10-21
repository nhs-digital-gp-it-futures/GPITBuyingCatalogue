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

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers
{
    [Authorize(Policy = "AccountManager")]
    [Area("AccountManagement")]
    [Route("accountmanagement/manageaccount")]
    public sealed class ManageAccountController : OrganisationController
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
        public override async Task<IActionResult> Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await GetOrganisationsService().GetOrganisationByInternalIdentifier(internalOrgId);
            return RedirectToAction(
                nameof(ManageAccountController.Details),
                typeof(ManageAccountController).ControllerName(),
                new { organisationId = organisation.Id });
        }
    }
}
