// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Pages.Account;

public class LoginWith2faModel : PageModel
{
    private readonly SignInManager<AspNetUser> signInManager;
    private readonly UserManager<AspNetUser> userManager;
    private readonly IOdsService odsService;
    private readonly ILogger<LoginWith2faModel> logger;

    public LoginWith2faModel(
        SignInManager<AspNetUser> signInManager,
        UserManager<AspNetUser> userManager,
        IOdsService odsService,
        ILogger<LoginWith2faModel> logger)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.odsService = odsService;
        this.logger = logger;
    }

    /// <summary>
    ///     Gets or sets this API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    /// <summary>
    ///     Gets or sets a value indicating whether this API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    ///     Gets or sets this API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     Gets or sets this API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; } = default!;

        /// <summary>
        ///     Gets or sets a value indicating whether this API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        returnUrl = returnUrl ?? Url.Content("~/");

        // This user comes from Identity's temporary two-factor cookie.
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");

        var authenticatorCode = Input.TwoFactorCode
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

        var userId = await userManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            logger.LogInformation(
                "User with ID '{UserId}' logged in with 2FA.",
                userId);

            // Perform the same post-login work as the ordinary login path.
            user.LoginEvents.Add(
                new AspNetUserLoginEvent(DateTime.UtcNow));

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                logger.LogError(
                    "Unable to record the login event for user '{UserId}'. " +
                    "Errors: {Errors}",
                    userId,
                    string.Join(
                        "; ",
                        updateResult.Errors.Select(error =>
                            $"{error.Code}: {error.Description}")));
            }

            await odsService.UpdateOrganisationDetails(
                user.PrimaryOrganisation.ExternalIdentifier);

            logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
            return LocalRedirect(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
            return RedirectToPage("./Lockout");
        }
        else
        {
            logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }
}
