using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    /// <summary>
    /// Password services.
    /// </summary>
    public interface IPasswordService
    {
        public const string InvalidTokenCode = "InvalidToken";

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
        Task<PasswordResetToken> GeneratePasswordResetTokenAsync(string emailAddress);

        /// <summary>
        /// Sends a password reset e-mail to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to send the e-mail to.</param>
        /// <param name="callback">The callback URL to handle the password reset.</param>
        /// <returns>An asynchronous task context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <see langref="null"/>.</exception>
        Task SendResetEmailAsync(AspNetUser user, Uri callback);

        /// <summary>
        /// Resets the password of the user with the specified <paramref name="emailAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="token">The validation token for authorizing the password reset.</param>
        /// <param name="newPassword">The value of the new password.</param>
        /// <returns>The result of the password reset operation.</returns>
        public Task<IdentityResult> ResetPasswordAsync(string emailAddress, string token, string newPassword);

        /// <summary>
        /// Returns true if the <paramref name="token"/> is valid.
        /// </summary>
        /// <param name="emailAddress">The e-mail address of the user.</param>
        /// <param name="token">The password reset token.</param>
        /// <returns><see langref="true"/> if the token is valid; otherwise <see langref="false"/>.</returns>
        Task<bool> IsValidPasswordResetTokenAsync(string emailAddress, string token);
    }
}
