using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Account")]
    public class AccountController : Controller
    {
        public const string SignInErrorMessage = "Enter a valid email address and password";

        public const string UserDisabledErrorMessageTemplate = @"There is a problem accessing your account.
                Contact the account administrator at: {0} or call {1}";

        private readonly ILogWrapper<AccountController> logger;
        private readonly SignInManager<AspNetUser> signInManager;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IPasswordService passwordService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly DisabledErrorMessageSettings disabledErrorMessageSettings;

        public AccountController(ILogWrapper<AccountController> logger, SignInManager<AspNetUser> signInManager, UserManager<AspNetUser> userManager, IPasswordService passwordService, IPasswordResetCallback passwordResetCallback, DisabledErrorMessageSettings disabledErrorMessageSettings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            logger.LogInformation($"Handling post for {nameof(AccountController)}.{nameof(Login)}");

            if (!ModelState.IsValid)
                return View(viewModel);

            var signinResult = await signInManager.PasswordSignInAsync(viewModel.EmailAddress, viewModel.Password, isPersistent: false, lockoutOnFailure: true);

            if (signinResult.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(viewModel.EmailAddress);

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

                if (string.IsNullOrWhiteSpace(viewModel.ReturnUrl))
                    return Redirect("~/");

                return Redirect(viewModel.ReturnUrl);
            }

            viewModel.Error = "The username or password were not recognised. Please try again.";

            return View(viewModel);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(Logout)}");

            if (signInManager.IsSignedIn(User))
                await signInManager.SignOutAsync().ConfigureAwait(false);

            return LocalRedirect("~/");
        }

        [HttpGet("Registration")]
        public IActionResult Registration()
        {
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(Registration)}");

            return View();
        }

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(ForgotPassword)}");

            return View();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            logger.LogInformation($"Handling post for {nameof(AccountController)}.{nameof(ForgotPassword)}");

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
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(ForgotPasswordLinkSent)}");

            return View();
        }

        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(ResetPassword)}");

            var isValid = await passwordService.IsValidPasswordResetTokenAsync(email, token);

            if (!isValid)
                return RedirectToAction(nameof(ResetPasswordExpired));

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            logger.LogInformation($"Handling post for {nameof(AccountController)}.{nameof(ResetPassword)}");

            if (!ModelState.IsValid)
                return View(viewModel);

            var res = await passwordService.ResetPasswordAsync(viewModel.Email, viewModel.Token, viewModel.Password);

            if (res.Succeeded)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

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
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(ResetPasswordConfirmation)}");

            return View();
        }

        public IActionResult ResetPasswordExpired()
        {
            logger.LogInformation($"Taking user to {nameof(AccountController)}.{nameof(ResetPasswordExpired)}");

            return View();
        }
    }
}
