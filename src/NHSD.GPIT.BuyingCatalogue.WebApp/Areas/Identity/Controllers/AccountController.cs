using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        public const string SignInErrorMessage = "Enter a valid email address and password";

        public const string UserDisabledErrorMessageTemplate = @"There is a problem accessing your account.

Contact the account administrator at: {0} or call {1}";



        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IPasswordService _passwordService;
        private readonly IPasswordResetCallback _passwordResetCallback;
        private readonly DisabledErrorMessageSettings _disabledErrorMessageSettings;

        public AccountController(ILogger<AccountController> logger, SignInManager<AspNetUser> signInManager, UserManager<AspNetUser> userManager, IPasswordService passwordService, IPasswordResetCallback passwordResetCallback, DisabledErrorMessageSettings disabledErrorMessageSettings)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _passwordService = passwordService;
            _passwordResetCallback = passwordResetCallback;
            _disabledErrorMessageSettings = disabledErrorMessageSettings;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var loginViewModel = new LoginViewModel { ReturnUrl = returnUrl };

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
                return View(viewModel);

            var signinResult = await _signInManager.PasswordSignInAsync(viewModel.EmailAddress, viewModel.Password, isPersistent: false, lockoutOnFailure: true);

            if (signinResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.EmailAddress);

                if(user.Disabled)
                {
                    var disabledErrorFormat = string.Format(
                        CultureInfo.CurrentCulture,
                        UserDisabledErrorMessageTemplate,
                        _disabledErrorMessageSettings.EmailAddress,
                        _disabledErrorMessageSettings.PhoneNumber);

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

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync().ConfigureAwait(false);

            return LocalRedirect("~/");
        }

        [HttpGet]
        public IActionResult Registration()
        {            
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            var resetToken = await _passwordService.GeneratePasswordResetTokenAsync(viewModel.EmailAddress);
            if (resetToken is null)
                return RedirectToAction(nameof(ForgotPasswordLinkSent));

            await _passwordService.SendResetEmailAsync(
                resetToken.User,
                _passwordResetCallback.GetPasswordResetCallback(resetToken));

            return RedirectToAction(nameof(ForgotPasswordLinkSent));
        }

        [HttpGet]
        public IActionResult ForgotPasswordLinkSent()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {            
            var isValid = await _passwordService.IsValidPasswordResetTokenAsync(email, token);

            if (!isValid)
            {
                return RedirectToAction(nameof(ResetPasswordExpired));
            }

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            
            var res = await _passwordService.ResetPasswordAsync(viewModel.Email, viewModel.Token, viewModel.Password);
            if (res.Succeeded)
            {
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
            {
                return RedirectToAction(nameof(ResetPasswordExpired));
            }

            throw new InvalidOperationException(
                $"Unexpected errors whilst resetting password: {string.Join(" & ", res.Errors.Select(error => error.Description))}");            
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPasswordExpired()
        {
            return View();
        }
    }
}
