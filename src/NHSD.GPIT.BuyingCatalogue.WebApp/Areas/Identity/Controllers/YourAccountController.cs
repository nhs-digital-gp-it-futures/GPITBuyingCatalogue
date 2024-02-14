using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/YourAccount")]
    [Authorize(Policy = "Buyer")]
    public sealed class YourAccountController : Controller
    {
        public IActionResult Index()
        {
            var organisation = await OrganisationsService.GetOrganisation(organisationId);
            var model = new YourAccountModel(organisation)
            {
                Title = "Your Account",
                Caption = User.Identity.Name,
            };

            return View(model);
        }
    }
}
