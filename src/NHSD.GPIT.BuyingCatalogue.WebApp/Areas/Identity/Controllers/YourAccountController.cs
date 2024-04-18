using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
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
        public const string ManagePasswordTitle = "Manage password";

        private readonly IOrganisationsService organisationsService;
        private readonly IEmailPreferenceService emailPreferenceService;
        private readonly IPasswordService passwordService;

        public YourAccountController(
            IOrganisationsService organisationsService,
            IEmailPreferenceService emailPreferenceService,
            IPasswordService passwordService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.emailPreferenceService = emailPreferenceService ?? throw new ArgumentNullException(nameof(emailPreferenceService));
            this.passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
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

            var saved = true;

            return RedirectToAction(
                nameof(ManageEmailNotifications),
                typeof(YourAccountController).ControllerName(),
                new { saved });
        }

        [HttpGet("ManagePassword")]
        public IActionResult ManagePassword(bool saved = false)
        {
            var model = new ManagePasswordModel()
            {
                Title = ManagePasswordTitle,
                Caption = User.GetUserDisplayName(),
                Saved = saved,
            };

            return View(model);
        }

        [HttpPost("ManagePassword")]
        public async Task<IActionResult> ManagePassword(ManagePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatePasswordViewModel.IdentityResult = await passwordService.ChangePasswordAsync(User.Identity.Name, model.UpdatePasswordViewModel.CurrentPassword, model.UpdatePasswordViewModel.NewPassword);
                if (TryValidateModel(model))
                {
                    if (!model.UpdatePasswordViewModel.IdentityResult.Succeeded)
                    {
                        // it's not succeeded and the validator hasn't handled it
                        throw new InvalidOperationException($"Unexpected errors whilst updating password: {string.Join(" & ", model.UpdatePasswordViewModel.IdentityResult.Errors.Select(error => error.Description))}");
                    }

                    await passwordService.UpdatePasswordChangedDate(User.Identity.Name);
                    var saved = true;

                    return RedirectToAction(
                        nameof(ManagePassword),
                        typeof(YourAccountController).ControllerName(),
                        new { saved });
                }
            }

            return View(model);
        }
    }
}
