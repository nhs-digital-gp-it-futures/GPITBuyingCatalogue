using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<AspNetUser> _signInManager;

        public AccountController(ILogger<AccountController> logger, SignInManager<AspNetUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
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

            // MJRTODO
            //var resetToken = await passwordService.GeneratePasswordResetTokenAsync(viewModel.EmailAddress);
            //if (resetToken is null)
            //    return RedirectToAction(nameof(ForgotPasswordLinkSent));

            //await passwordService.SendResetEmailAsync(
            //    resetToken.User,
            //    passwordResetCallback.GetPasswordResetCallback(resetToken));

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
            // MJRTODO
            //var isValid = await passwordService.IsValidPasswordResetTokenAsync(email, token);

            //if (!isValid)
            //{
            //    return RedirectToAction(nameof(ResetPasswordExpired));
            //}

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

            // MJRTODO
            //var res = await passwordService.ResetPasswordAsync(viewModel.Email, viewModel.Token, viewModel.Password);
            //if (res.Succeeded)
            //{
            //    return RedirectToAction(nameof(ResetPasswordConfirmation));
            //}

            //var invalidPasswordError = res.Errors.FirstOrDefault(error => error.Code == PasswordValidator.InvalidPasswordCode);
            //if (invalidPasswordError is not null)
            //{
            //    ModelState.AddModelError(nameof(ResetPasswordViewModel.Password), invalidPasswordError.Description);
            //    return View(viewModel);
            //}

            //var invalidTokenError = res.Errors.FirstOrDefault(error => error.Code == PasswordService.InvalidTokenCode);
            //if (invalidTokenError is not null)
            //{
            //    return RedirectToAction(nameof(ResetPasswordExpired));
            //}

            //throw new InvalidOperationException(
            //    $"Unexpected errors whilst resetting password: {string.Join(" & ", res.Errors.Select(error => error.Description))}");

            return RedirectToAction(nameof(ResetPasswordConfirmation)); // MJRTODO - Remove once above is done

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
