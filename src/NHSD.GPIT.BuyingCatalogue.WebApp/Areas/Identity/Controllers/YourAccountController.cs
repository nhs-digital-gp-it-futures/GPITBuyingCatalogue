using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/YourAccount")]
    [Authorize]
    public sealed class YourAccountController : Controller
    {
        public const string YourAccountTitle = "Your account";
        public const string ManageEmailNotificationsTitle = "Manage email notifications";

        private readonly IOrganisationsService organisationsService;
        private readonly IEmailPreferenceService emailPreferenceService;

        public YourAccountController(
            IOrganisationsService organisationsService,
            IEmailPreferenceService emailPreferenceService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.emailPreferenceService = emailPreferenceService ?? throw new ArgumentNullException(nameof(emailPreferenceService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var model = new YourAccountModel(organisation)
            {
                Title = YourAccountTitle,
                Caption = User.GetUserDisplayName(),
            };

            return View(model);
        }

        [HttpGet("ManageEmailNotifications")]
        public async Task<IActionResult> ManageEmailNotifications(bool saved = false)
        {
            var preferences = await emailPreferenceService.Get(User.UserId());

            var model = new ManageEmailPreferencesModel()
            {
                Title = ManageEmailNotificationsTitle,
                Caption = User.GetUserDisplayName(),
                EmailPreferences = preferences,
                Saved = saved,
            };

            return View(model);
        }

        [HttpPost("ManageEmailNotifications")]
        public async Task<IActionResult> ManageEmailNotifications(ManageEmailPreferencesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await emailPreferenceService.Save(User.UserId(), model.EmailPreferences);

            const bool saved = true;

            return RedirectToAction(
                nameof(ManageEmailNotifications),
                typeof(YourAccountController).ControllerName(),
                new { saved });
        }
    }
}
