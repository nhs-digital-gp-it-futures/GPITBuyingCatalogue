using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("registration")]
    public class RegistrationController : Controller
    {
        private readonly IRequestAccountService requestAccountService;

        public RegistrationController(IRequestAccountService requestAccountService)
        {
            this.requestAccountService = requestAccountService ?? throw new ArgumentNullException(nameof(requestAccountService));
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new NavBaseModel
            {
                BackLink = Url.Action(
                    nameof(HomeController.Index),
                    typeof(HomeController).ControllerName()),
                BackLinkText = "Go back to homepage",
            };

            return View(model);
        }

        [HttpGet("details")]
        public IActionResult Details()
        {
            var model = new RegistrationDetailsModel
            {
                BackLink = Url.Action(nameof(Index)),
                BackLinkText = "Go back",
            };

            return View(model);
        }

        [ValidateRecaptcha]
        [HttpPost("details")]
        public async Task<IActionResult> Details(RegistrationDetailsModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await requestAccountService.RequestAccount(new NewAccountDetails
            {
                FullName = viewModel.FullName,
                EmailAddress = viewModel.EmailAddress,
                OrganisationName = viewModel.OrganisationName,
                OdsCode = viewModel.OdsCode,
                HasGivenUserResearchConsent = viewModel.HasGivenUserResearchConsent,
            });

            return RedirectToAction(nameof(Confirmation));
        }

        [HttpGet("confirmation")]
        public IActionResult Confirmation()
        {
            var model = new NavBaseModel
            {
                BackLink = Url.Action(
                    nameof(HomeController.Index),
                    typeof(HomeController).ControllerName()),
                BackLinkText = "Go back to homepage",
            };

            return View(model);
        }
    }
}
