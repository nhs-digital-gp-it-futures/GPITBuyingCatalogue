﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.Identity
{
    /// <summary>
    /// Provides password services.
    /// </summary>
    public sealed class PasswordService : IPasswordService
    {
        public const string InvalidTokenCode = "InvalidToken";

        private readonly IEmailService emailService;
        private readonly IdentityOptions identityOptions = new();
        private readonly PasswordResetSettings settings;
        private readonly UserManager<AspNetUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordService"/> class using
        /// the provided <paramref name="emailService"/> and <paramref name="settings"/>.
        /// </summary>
        /// <param name="emailService">The service to use to send e-mails.</param>
        /// <param name="settings">The configured password reset settings.</param>
        /// <param name="userManager">The Identity framework user manager.</param>
        /// <exception cref="ArgumentNullException"><paramref name="emailService"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="userManager"/> is <see langref="null"/>.</exception>
        public PasswordService(
            IEmailService emailService,
            PasswordResetSettings settings,
            UserManager<AspNetUser> userManager)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Generates a password reset token for the user with the
        /// provided <paramref name="emailAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The e-mail of the user to generate the password
        /// reset token for.</param>
        /// <returns>A <see cref="PasswordResetToken"/> if the user was found;
        /// otherwise, <see langref="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="emailAddress"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="emailAddress"/> is empty or white space.</exception>
        public async Task<PasswordResetToken> GeneratePasswordResetTokenAsync(string emailAddress)
        {
            emailAddress.ValidateNotNullOrWhiteSpace(nameof(emailAddress));

            var user = await userManager.FindByEmailAsync(emailAddress);

            return user is null
                ? null
                : new PasswordResetToken(await userManager.GeneratePasswordResetTokenAsync(user), user);
        }

        /// <summary>
        /// Sends a password reset e-mail to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to send the e-mail to.</param>
        /// <param name="callback">The callback URL to handle the password reset.</param>
        /// <returns>An asynchronous task context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <see langref="null"/>.</exception>
        public async Task SendResetEmailAsync(AspNetUser user, Uri callback)
        {
            user.ValidateNotNull(nameof(user));
            callback.ValidateNotNull(nameof(callback));

            await emailService.SendEmailAsync(
                settings.EmailMessageTemplate,
                new EmailAddress(user.Email, $"{user.FirstName} {user.LastName}"),
                callback);
        }

        /// <summary>
        /// Returns true if the <paramref name="token"/> is valid.
        /// </summary>
        /// <param name="emailAddress">The e-mail address of the user.</param>
        /// <param name="token">The password reset token.</param>
        /// <returns><see langref="true"/> if the token is valid; otherwise <see langref="false"/>.</returns>
        public async Task<bool> IsValidPasswordResetTokenAsync(string emailAddress, string token)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                return false;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            var user = await userManager.FindByEmailAsync(emailAddress);
            if (user is null)
                return false;

            return await userManager.VerifyUserTokenAsync(
                user,
                identityOptions.Tokens.PasswordResetTokenProvider,
                UserManager<IdentityUser>.ResetPasswordTokenPurpose,
                token);
        }

        /// <summary>
        /// Resets the password of the user with the specified <paramref name="emailAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="token">The validation token for authorizing the password reset.</param>
        /// <param name="newPassword">The value of the new password.</param>
        /// <returns>The result of the password reset operation.</returns>
        public async Task<IdentityResult> ResetPasswordAsync(string emailAddress, string token, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(emailAddress);
            return await userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}
