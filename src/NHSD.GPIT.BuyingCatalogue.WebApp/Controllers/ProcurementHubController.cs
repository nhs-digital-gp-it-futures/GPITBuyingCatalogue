using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.ProcurementHub;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("contact-procurement-hub")]
    public class ProcurementHubController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IProcurementHubService procurementHubService;
        private readonly IUsersService usersService;

        public ProcurementHubController(
            IOrganisationsService organisationsService,
            IProcurementHubService procurementHubService,
            IUsersService usersService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.procurementHubService = procurementHubService ?? throw new ArgumentNullException(nameof(procurementHubService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            AspNetUser user;
            Organisation organisation;

            try
            {
                user = await usersService.GetUser(User.UserId());
                organisation = await organisationsService.GetOrganisation(user.PrimaryOrganisationId);
            }
            catch
            {
                user = null;
                organisation = null;
            }

            return View(new ProcurementHubDetailsModel
            {
                BackLink = string.IsNullOrWhiteSpace(returnUrl)
                    ? Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName())
                    : returnUrl,
                FullName = user?.FullName,
                EmailAddress = user?.Email,
                OrganisationName = organisation?.Name,
                OdsCode = organisation?.OdsCode,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProcurementHubDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await procurementHubService.ContactProcurementHub(new ProcurementHubRequest
            {
                FullName = model.FullName,
                Email = model.EmailAddress,
                OrganisationName = model.OrganisationName,
                OdsCode = model.OdsCode,
                Query = model.Query,
            });

            return RedirectToAction(
                nameof(Confirmation),
                new { returnUrl = model.BackLink });
        }

        [HttpGet("confirmation")]
        public IActionResult Confirmation(string returnUrl = null)
        {
            var model = new NavBaseModel
            {
                BackLink = string.IsNullOrWhiteSpace(returnUrl)
                    ? Url.Action(nameof(HomeController.Index), typeof(HomeController).ControllerName())
                    : returnUrl,
                BackLinkText = string.IsNullOrWhiteSpace(returnUrl)
                    ? "Go back to homepage"
                    : "Go back",
            };

            return View(model);
        }
    }
}
