using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Account")]
    public sealed class AccountController : Controller
    {
        public const string UserDisabledErrorMessageTemplate = @"There is a problem accessing your account.
                Contact the account administrator at: {0} or call {1}";

        private readonly SignInManager<AspNetUser> signInManager;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IOdsService odsService;
        private readonly IUsersService userServices;
        private readonly IPasswordService passwordService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly DisabledErrorMessageSettings disabledErrorMessageSettings;

        public AccountController(
            SignInManager<AspNetUser> signInManager,
            UserManager<AspNetUser> userManager,
            IOdsService odsService,
            IUsersService userServices,
            IPasswordService passwordService,
            IPasswordResetCallback passwordResetCallback,
            DisabledErrorMessageSettings disabledErrorMessageSettings)
        {
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.userServices = userServices ?? throw new ArgumentNullException(nameof(userServices));
            this.passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            this.passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            this.disabledErrorMessageSettings = disabledErrorMessageSettings ?? throw new ArgumentNullException(nameof(disabledErrorMessageSettings));
        }

        [HttpGet("Login")]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            IActionResult BadLogin()
            {
                const string message = "The username or password were not recognised. Please try again.";

                ModelState.AddModelError(nameof(LoginViewModel.EmailAddress), message);
                ModelState.AddModelError(nameof(LoginViewModel.Password), message);

                return View(viewModel);
            }

            var user = await userManager.FindByNameAsync(viewModel.EmailAddress);
            if (user is null)
                return BadLogin();

            if (user.Disabled)
            {
                var disabledErrorFormat = string.Format(
                    CultureInfo.CurrentCulture,
                    UserDisabledErrorMessageTemplate,
                    disabledErrorMessageSettings.EmailAddress,
                    disabledErrorMessageSettings.PhoneNumber);

                ModelState.AddModelError(nameof(LoginViewModel.DisabledError), disabledErrorFormat);

                return View(viewModel);
            }

            var signinResult = await signInManager.PasswordSignInAsync(user, viewModel.Password, false, true);

            if (signinResult.Succeeded)
            {
                await odsService.UpdateOrganisationDetails(user.PrimaryOrganisation.ExternalIdentifier);
                return Redirect(string.IsNullOrWhiteSpace(viewModel.ReturnUrl) ? "~/" : viewModel.ReturnUrl);
            }

            return signinResult.IsLockedOut ? RedirectToAction(nameof(LockedAccount)) : BadLogin();
        }

        [HttpGet("LockedAccount")]
        public IActionResult LockedAccount()
        {
            var model = new NavBaseModel
            {
                BackLink = Url.Action(nameof(Login)),
                BackLinkText = "Go back",
            };
            return View(model);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
                await signInManager.SignOutAsync().ConfigureAwait(false);

            return LocalRedirect("~/");
        }

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var resetToken = await passwordService.GeneratePasswordResetTokenAsync(viewModel.EmailAddress);
            if (resetToken is null)
                return RedirectToAction(nameof(ForgotPasswordLinkSent));

            await passwordService.SendResetEmailAsync(
                resetToken.User,
                passwordResetCallback.GetPasswordResetCallback(resetToken));

            return RedirectToAction(nameof(ForgotPasswordLinkSent));
        }

        [HttpGet("ForgotPasswordLinkSent")]
        public IActionResult ForgotPasswordLinkSent()
        {
            return View();
        }

        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            var isValid = await passwordService.IsValidPasswordResetTokenAsync(email, token);

            if (!isValid)
                return RedirectToAction(nameof(ResetPasswordExpired));

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var user = await userManager.FindByEmailAsync(viewModel.Email);
            var usedPassword = await userServices.IsPasswordValid(user, viewModel.Password);
            if (usedPassword)
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), ResetPasswordViewModel.ErrorMessages.PasswordPreviouslyUsed);
                return View(viewModel);
            }

            var res = await passwordService.ResetPasswordAsync(viewModel.Email, viewModel.Token, viewModel.Password);
            if (res.Succeeded)
            {
                await passwordService.UpdatePasswordChangedDate(viewModel.Email);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var invalidPasswordError = res.Errors.FirstOrDefault(error => error.Code == PasswordValidator.InvalidPasswordCode);

            if (invalidPasswordError is not null)
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), invalidPasswordError.Description);
                return View(viewModel);
            }

            var invalidTokenError = res.Errors.FirstOrDefault(error => error.Code == IPasswordService.InvalidTokenCode);

            if (invalidTokenError is not null)
                return RedirectToAction(nameof(ResetPasswordExpired));

            throw new InvalidOperationException(
                $"Unexpected errors whilst resetting password: {string.Join(" & ", res.Errors.Select(error => error.Description))}");
        }

        [HttpGet("ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPasswordExpired()
        {
            return View();
        }

        [HttpGet("UpdatePassword")]
        public IActionResult UpdatePassword()
        {
            return View();
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var res = await passwordService.ChangePasswordAsync(User.Identity.Name, viewModel.CurrentPassword, viewModel.NewPassword);

            if (res.Succeeded)
            {
                await passwordService.UpdatePasswordChangedDate(User.Identity.Name);
                await signInManager.SignOutAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Login));
            }

            var incorrectPasswordError = res.Errors.FirstOrDefault(error => error.Code == UpdatePasswordViewModelValidator.CurrentPasswordMismatchCode);
            var invalidPasswordError = res.Errors.FirstOrDefault(error => error.Code == PasswordValidator.InvalidPasswordCode);

            if (incorrectPasswordError is null && invalidPasswordError is null)
                throw new InvalidOperationException($"Unexpected errors whilst updating password: {string.Join(" & ", res.Errors.Select(error => error.Description))}");

            if (incorrectPasswordError is not null)
                ModelState.AddModelError(nameof(UpdatePasswordViewModel.CurrentPassword), UpdatePasswordViewModelValidator.CurrentPasswordIncorrect);

            if (invalidPasswordError is not null)
                ModelState.AddModelError(nameof(UpdatePasswordViewModel.NewPassword), invalidPasswordError.Description);

            return View(viewModel);
        }
    }
}
