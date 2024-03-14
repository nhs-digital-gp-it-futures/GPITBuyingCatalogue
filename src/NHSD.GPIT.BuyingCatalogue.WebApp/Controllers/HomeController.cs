using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactUsService contactUsService;
        private readonly IOrganisationsService organisationsService;
        private readonly IOrderService orderService;
        private readonly IManageFiltersService manageFiltersService;
        private readonly ICompetitionsService competitionsService;

        public HomeController(
            IContactUsService contactUsService,
            IOrganisationsService organisationsService,
            IOrderService orderService,
            IManageFiltersService manageFiltersService,
            ICompetitionsService competitionsService)
        {
            this.contactUsService = contactUsService ?? throw new ArgumentNullException(nameof(contactUsService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
            this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public IActionResult Index() => View();

        [Authorize]
        public async Task<IActionResult> AccountDashboard()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var existingFilters = await manageFiltersService.GetFilters(organisation.Id);
            var competitions = await competitionsService.GetCompetitionsDashboard(internalOrgId);
            var orders = await orderService.GetOrders(organisation.Id);

            var model = new AccountDashboardModel()
            {
                OrganisationName = organisation.Name,
                InternalOrgId = internalOrgId,
                Orders = orders,
                Shortlists = existingFilters,
                Competitions = competitions?.ToList(),
            };
            return View(model);
        }

        [HttpGet("privacy-policy")]
        public IActionResult PrivacyPolicy()
            => View();

        [Authorize]
        [HttpGet("contact-us")]
        public IActionResult ContactUs()
            => View(new ContactUsModel());

        [Authorize]
        [ValidateRecaptcha]
        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs(ContactUsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await contactUsService.SubmitQuery(
                model.FullName,
                model.EmailAddress,
                model.Message);

            return RedirectToAction(nameof(ContactUsConfirmation));
        }

        [HttpGet("contact-us/confirmation")]
        public IActionResult ContactUsConfirmation()
        {
            var model = new ContactUsConfirmationModel() { BackLink = Url.Action(nameof(Index)), };

            return View(model);
        }

        [HttpGet("accessibility-statement")]
        public IActionResult AccessibilityStatement() => View();

        [HttpGet("unauthorized")]
        public IActionResult NotAuthorized() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null, string error = null)
        {
            if (statusCode is 404)
            {
                var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                ViewData["BadUrl"] = $"Incorrect url {feature?.OriginalPath}";
                return View("PageNotFound");
            }

            return View(new ErrorModel(error));
        }
    }
}
