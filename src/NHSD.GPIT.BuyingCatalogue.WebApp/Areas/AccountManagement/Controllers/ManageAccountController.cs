using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers
{
    [Authorize(Policy = "AccountManager")]
    [Area("AccountManagement")]
    [Route("account-management/manage-account")]
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

        protected override string ControllerName => typeof(ManageAccountController).ControllerName();

        protected override string HomeLink => Url.Action(
            nameof(HomeController.Index),
            typeof(HomeController).ControllerName());

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await OrganisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            return RedirectToAction(
                nameof(Details),
                typeof(ManageAccountController).ControllerName(),
                new { organisationId = organisation.Id });
        }
    }
}
