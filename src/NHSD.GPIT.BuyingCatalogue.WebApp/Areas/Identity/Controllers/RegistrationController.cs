using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("registration")]
    public class RegistrationController(IAccountRequestsService accountRequestsService) : Controller
    {
        private readonly IAccountRequestsService accountRequestsService = accountRequestsService ?? throw new ArgumentNullException(nameof(accountRequestsService));

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

            var confirmationRoute = new RoutingResult
            {
                ControllerName = typeof(RegistrationController).ControllerName(),
                ActionName = nameof(ConfirmRegistration),
                AreaName = typeof(RegistrationController).AreaName(),
            };

            await accountRequestsService.SubmitAccountRequest(
                new AccountRequest(
                    viewModel.FirstName,
                    viewModel.LastName,
                    viewModel.EmailAddress,
                    viewModel.OdsCode,
                    viewModel.Justification,
                    viewModel.HasGivenUserResearchConsent),
                confirmationRoute);

            return RedirectToAction(nameof(Confirmation));
        }

        [HttpGet("{requestId}/confirm-registration")]
        public async Task<IActionResult> ConfirmRegistration(Guid requestId, string email)
        {
            await accountRequestsService.ConfirmAccountRequest(requestId, email);

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
