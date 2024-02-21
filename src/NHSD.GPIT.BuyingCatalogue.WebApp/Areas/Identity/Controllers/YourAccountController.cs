using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/YourAccount")]
    [Authorize]
    public sealed class YourAccountController : Controller
    {
        private readonly IOrganisationsService organisationsService;

        public YourAccountController(
        IOrganisationsService organisationsService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var model = new YourAccountModel(organisation)
            {
                Title = "Your account",
                Caption = User.GetUserDisplayName(),
            };

            return View(model);
        }
    }
}
