using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Authorize]
    [Route("nominate-organisation")]
    public class NominateOrganisationController : Controller
    {
        private readonly INominateOrganisationService nominateOrganisationService;

        public NominateOrganisationController(
            INominateOrganisationService nominateOrganisationService)
        {
            this.nominateOrganisationService = nominateOrganisationService ?? throw new ArgumentNullException(nameof(nominateOrganisationService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (await nominateOrganisationService.IsGpPractice(User.UserId()))
            {
                return RedirectToAction(
                    nameof(Unavailable));
            }
            return View();
        }

        [HttpGet("details")]
        public IActionResult Details()
        {
            var model = new NominateOrganisationDetailsModel
            {
                BackLink = Url.Action(nameof(Index)),
                BackLinkText = "Go back",
            };

            return View(model);
        }

        [HttpPost("details")]
        public async Task<IActionResult> Details(NominateOrganisationDetailsModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await nominateOrganisationService.NominateOrganisation(User.UserId(), new NominateOrganisationRequest
            {
                OdsCode = viewModel.OdsCode,
                OrganisationName = viewModel.OrganisationName,
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

        [HttpGet("unavailable")]
        public IActionResult Unavailable()
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
